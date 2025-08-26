using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.SpecificProcessors;
using BlendJson.TypeResolving;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson
{
    public class SettingsManager
    {
        private readonly List<ISpecificProcessor> _specialProcessors;

        public IServiceProvider ServiceProvider { get; set; }

        public IFsProvider FsProvider { get; set; } = new FsProvider();

        public SettingsManager(params ISpecificProcessor[] processors)
        {
            _specialProcessors = new List<ISpecificProcessor>
            {
                new LoadProcessor(),
                new MergeProcessor(),
                new MergeArrayProcessor(),
                new ConcatArrayProcessor(),
                new UnionArrayProcessor(),
                new ExceptArrayProcessor(),

            };

            _specialProcessors.AddRange(processors);
        }

        private List<JsonConverter> Converters
        {
            get
            {
                var result = new List<JsonConverter>(3)
                {
                    new JsonImplConverter(ServiceProvider),
                    new ByteArrayConverter()
                };

                if (ServiceProvider != null)
                    result.Add(new DependencyInjectionActivator(ServiceProvider));

                return result;
            }
        }

        internal async Task<JToken> LoadSettingsAsync(IDataSource dataSource, ParseContext context, LoadMode mode, CancellationToken token)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            (var jToken, dataSource) = await dataSource.LoadAsync(context.DataSource, mode, context, token);

            return await ProcessJTokenAsync(jToken, new ParseContext
            {
                DataSource = dataSource,
                Manager = this,
                DisableProcessors = context.DisableProcessors,
                Serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = Converters
                }),
                Parameters = context.Parameters,
                FsProvider = context.FsProvider
            }, token);
        }

        public async Task<JToken> LoadSettingsAsync(IDataSource dataSource, CancellationToken token = default)
        {
            var parseContext = new ParseContext()
            {
                Manager = this,
                DataSource = dataSource,
                FsProvider = FsProvider
            };

            return await LoadSettingsAsync(dataSource, parseContext, LoadMode.Json, token);
        }

        public async Task<JToken> LoadSettingsAsync(string path, string workDir = null, CancellationToken token = default)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return await LoadSettingsZipAsync(path, token);
            else
                return await LoadSettingsJsonAsync(path, workDir, token);
        }

        public async Task<T> LoadSettingsAsync<T>(string path, string workDir = null, CancellationToken token = default)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return await LoadSettingsZipAsync<T>(path, token);
            else
                return await LoadSettingsJsonAsync<T>(path, workDir, token);
        }

        public async Task<JToken> LoadSettingsJsonAsync(string path, string workDir = null, CancellationToken token = default)
        {
            return await LoadSettingsAsync(new FileDataSource() { Path = path, WorkDir = workDir }, token);
        }

        public async Task<T> LoadSettingsJsonAsync<T>(string path, string workDir = null, CancellationToken token = default)
        {
            var jToken = await LoadSettingsJsonAsync(path, workDir, token);

            return LoadSettings<T>(jToken);
        }

        public T LoadSettings<T>(JToken token)
        {
            return token.ToObject<T>(JsonSerializer.Create(new JsonSerializerSettings()
            {
                Converters = Converters
            }));
        }

        public async Task<JToken> LoadSettingsZipAsync(string path, CancellationToken token = default)
        {
            await using var stream = File.OpenRead(path);
            using var archive = new ZipArchive(stream);
            return await LoadSettingsAsync(new ZipDataSource() { Path = "Main.json", ZipArchive = archive }, token);
        }

        public async Task<T> LoadSettingsZipAsync<T>(string path, CancellationToken token = default)
        {
            var jToken = await LoadSettingsZipAsync(path, token);

            return LoadSettings<T>(jToken);
        }

        private async Task<JToken> ProcessJTokenAsync(JToken jToken, ParseContext context, CancellationToken token)
        {
            try
            {
                if (jToken is JObject jObject)
                    return await ProcessJObjectAsync(jObject, context, token);
                if (jToken is JArray jArray)
                    return await ProcessJArrayAsync(jArray, context, token);

                return jToken;
            }
            catch (Exception e)
            {
                throw new SettingsException($"Error procesing JToken: {jToken.Path}", e)
                {
                    JToken = jToken
                };
            }

        }

        private async Task<JToken> ProcessJArrayAsync(JArray jArray, ParseContext context, CancellationToken token)
        {
            var anotherArray = new JArray();
            foreach (var jToken in jArray.Children())
            {
                context.MergeArray = false;
                JToken p;
                p = await ProcessJTokenAsync(jToken, context, token);

                if (p == null)
                    continue;

                if (context.MergeArray)
                {
                    foreach (var child in p.Children())
                    {
                        anotherArray.Add(child);
                    }
                }
                else
                    anotherArray.Add(p);
            }

            return anotherArray;
        }

        private async Task<JToken> ProcessJObjectAsync(JObject jObject, ParseContext context, CancellationToken token)
        {
            var specialProperties = jObject.Properties().Where(q => q.Name.StartsWith("@"))
                .Select(q => new { Name = q.Name.Substring(1), Token = q })
                .Select(q => new
                {
                    q.Name,
                    q.Token,
                    Processor = _specialProcessors.FirstOrDefault(proc =>
                        proc.IsPrefix
                            ? q.Name.StartsWith(proc.KeyWord)
                            : q.Name.Equals(proc.KeyWord, StringComparison.OrdinalIgnoreCase))
                })
                .Where(q => q.Processor != null).ToList();


            foreach (var specialProperty in specialProperties.Select(q => q.Token))
            {
                specialProperty.Remove();
            }

            JToken result = jObject;

            if (context.DisableProcessors == false)
            {
                foreach (var specialProperty in specialProperties)
                {
                    if (result is not JObject jobj)
                        break;

                    var processedOptions = await ProcessJTokenAsync(specialProperty.Token.Value, context, token);

                    try
                    {
                        result = await specialProperty.Processor!.DoAsync(context, processedOptions, jobj, specialProperty.Name, token);
                    }
                    catch (Exception e)
                    {
                        throw new SettingsException($"Error processing {specialProperty.Name} ({jobj.Path})", e)
                        {
                            JToken = specialProperty.Token.Value
                        };
                    }


                }
            }


            if (result is JObject jobj2)
            {
                foreach (var property in jobj2.Properties())
                {
                    property.Value = await ProcessJTokenAsync(property.Value, context, token);
                }
            }

            return result;
        }

    }
}
