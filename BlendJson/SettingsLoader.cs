using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.FsProviders;
using BlendJson.ParseModes;
using BlendJson.SpecificProcessors;
using BlendJson.TypeResolving;

namespace BlendJson
{
    public class SettingsLoader
    {
        private readonly List<ISpecificProcessor> _specialProcessors;
        private readonly List<Type> _dataSourceTypes;
        public IFsProvider FsProvider { get; set; } = new FsProvider();

        public SettingsLoader(params ISpecificProcessor[] processors)
        {
            _specialProcessors =
            [
                new LoadProcessor(),
                new MergeProcessor(),
                new MergeArrayProcessor(),
                new ConcatArrayProcessor(),
                new UnionArrayProcessor(),
                new ExceptArrayProcessor()

            ];

            _specialProcessors.AddRange(processors);

            _dataSourceTypes = GetDefaultDataSources();
        }

        public static List<Type> GetDefaultDataSources()
        {
            return [typeof(FileDataSource), typeof(ZipDataSource)];
        }

        private List<JsonConverter> Converters
        {
            get
            {
                var result = new List<JsonConverter>(3)
                {
                    new JsonImplConverter()
                    {
                        Interface = typeof(IDataSource),
                        Implementations = _dataSourceTypes.ToArray()
                    },
                    new JsonImplConverter(),
                    new ByteArrayConverter(),
                    new EncodingConverter(),
                    new JsonStringEnumConverter()
                };

                return result;
            }
        }

        JsonSerializerOptions SerializerOptions
        {
            get
            {
                var options = new JsonSerializerOptions();
                foreach (var jsonConverter in Converters)
                {
                    options.Converters.Add(jsonConverter);
                }

                return options;
            }
        }

        internal async Task<JsonNode?> LoadSettingsAsync(IDataSource dataSource, ParseContext context, LoadMode mode, CancellationToken token)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context is { Depth: > 0, GetRefsMode: { } getRefsMode } && dataSource is FileDataSource fileDataSource)
            {
                var (path, _) = await fileDataSource.GetActualPathAsync(context.DataSource, mode, context, token);
                getRefsMode.AddRef(new JsonReference(fileDataSource.Path, path));
            }

            if (context.MergeMode != null || context.Depth == 0)
            {
                (var jToken, dataSource) = await dataSource.LoadAsync(context.DataSource, mode, context, token);

                return await ProcessJTokenAsync(jToken, new ParseContext
                {
                    DataSource = dataSource,
                    Manager = this,
                    DisableProcessors = context.DisableProcessors,
                    SerializerOptions = SerializerOptions,
                    Parameters = context.Parameters,
                    FsProvider = context.FsProvider,
                    GetRefsMode = context.GetRefsMode,
                    MergeMode = context.MergeMode,
                    Depth = context.Depth + 1,
                    CheckFileExist = context.CheckFileExist
                }, token);
            }

            return null;
        }

        public async Task<JsonNode> LoadSettingsAsync(IDataSource dataSource, CancellationToken token = default)
        {
            var parseContext = new ParseContext()
            {
                Manager = this,
                DataSource = dataSource,
                FsProvider = FsProvider,
                MergeMode = new MergeMode(),
                CheckFileExist = true
            };

            return await LoadSettingsAsync(dataSource, parseContext, LoadMode.Json, token);
        }

        public async Task<(JsonNode, JsonReference[] References)> LoadSettingsWithRefsAsync(IDataSource dataSource, CancellationToken token = default)
        {
            var getRefsMode = new GetRefsMode();
            var parseContext = new ParseContext()
            {
                Manager = this,
                DataSource = dataSource,
                FsProvider = FsProvider,
                MergeMode = new MergeMode(),
                GetRefsMode = getRefsMode,
                CheckFileExist = true
            };

            return (await LoadSettingsAsync(dataSource, parseContext, LoadMode.Json, token), getRefsMode.References);
        }

        public async Task<(JsonReference[] References, JsonNode Json)> LoadRefsAsync(IDataSource dataSource, bool checkFileExist = true, CancellationToken token = default)
        {
            var getRefsMode = new GetRefsMode();
            var parseContext = new ParseContext()
            {
                Manager = this,
                DataSource = dataSource,
                FsProvider = FsProvider,
                GetRefsMode = getRefsMode,
                CheckFileExist = checkFileExist
            };

            var jToken = await LoadSettingsAsync(dataSource, parseContext, LoadMode.Json, token);
            return (getRefsMode.References, jToken);
        }



        public async Task<JsonNode> LoadSettingsAsync(string path, string? workDir = null, CancellationToken token = default)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return await LoadSettingsZipAsync(path, token);
            else
                return await LoadSettingsJsonAsync(path, workDir, token);
        }

        public async Task<T> LoadSettingsAsync<T>(string path, string? workDir = null, CancellationToken token = default)
        {
            var fi = new FileInfo(path);
            if (fi.Extension == ".zip")
                return await LoadSettingsZipAsync<T>(path, token);
            else
                return await LoadSettingsJsonAsync<T>(path, workDir, token);
        }

        public async Task<JsonNode> LoadSettingsJsonAsync(string path, string? workDir = null, CancellationToken token = default)
        {
            return await LoadSettingsAsync(new FileDataSource() { Path = path, WorkDir = workDir }, token);
        }

        public async Task<(JsonReference[] References, JsonNode Json)> LoadRefsAsync(string path, bool checkFileExist = true, string? workDir = null, CancellationToken token = default)
        {
            return await LoadRefsAsync(new FileDataSource() { Path = path, WorkDir = workDir }, checkFileExist, token);
        }

        public async Task<T?> LoadSettingsJsonAsync<T>(string path, string workDir = null, CancellationToken token = default)
        {
            var jToken = await LoadSettingsJsonAsync(path, workDir, token);

            return LoadSettings<T>(jToken);
        }

        public T? LoadSettings<T>(JsonNode token)
        {
            return token.Deserialize<T>(SerializerOptions);
        }

        public async Task<JsonNode> LoadSettingsZipAsync(string path, CancellationToken token = default)
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

        private async Task<JsonNode> ProcessJTokenAsync(JsonNode jToken, ParseContext context, CancellationToken token)
        {
            try
            {
                if (jToken is JsonObject jObject)
                    return await ProcessJObjectAsync(jObject, context, token);
                if (jToken is JsonArray jArray)
                    return await ProcessJArrayAsync(jArray, context, token);

                return jToken;
            }
            catch (Exception e)
            {
                throw new SettingsException($"Error procesing JToken: {jToken.GetPath()}", e)
                {
                    JToken = jToken
                };
            }

        }

        private async Task<JsonArray> ProcessJArrayAsync(JsonArray jArray, ParseContext context, CancellationToken token)
        {
            var anotherArray = new JsonArray();
            foreach (var jToken in jArray)
            {
                context.MergeArray = false;
                JsonNode p;
                p = await ProcessJTokenAsync(jToken, context, token);

                if (p == null)
                    continue;

                if (context.MergeArray && context.MergeMode != null && p is JsonArray jArr2)
                {
                    foreach (var child in jArr2)
                    {
                        anotherArray.Add(child?.DeepClone());
                    }
                }
                else
                    anotherArray.Add(p.DeepClone());
            }

            return anotherArray;
        }

        private async Task<JsonNode> ProcessJObjectAsync(JsonObject jObject, ParseContext context, CancellationToken token)
        {
            var specialProperties = jObject.Where(q => q.Key.StartsWith("@"))
                .Select(q => new { Name = q.Key.Substring(1), Token = q })
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

            if (context.MergeMode != null)
            {
                foreach (var specialProperty in specialProperties.Select(q => q.Token))
                {
                    jObject.Remove(specialProperty.Key);
                }
            }

            JsonNode result = jObject;

            if (context.DisableProcessors == false)
            {
                foreach (var specialProperty in specialProperties)
                {
                    if (result is not JsonObject jobj)
                        break;

                    var processedOptions = await ProcessJTokenAsync(specialProperty.Token.Value, context, token);

                    try
                    {
                        result = await specialProperty.Processor!.DoAsync(context, processedOptions, jobj, specialProperty.Name, token);
                    }
                    catch (Exception e)
                    {
                        throw new SettingsException($"Error processing {specialProperty.Name} ({jobj.GetPath()})", e)
                        {
                            JToken = specialProperty.Token.Value
                        };
                    }


                }
            }

            if (context.MergeMode == null)
            {
                result = jObject;
            }

            if (result is JsonObject jobj2)
            {
                foreach (var property in jobj2)
                {
                    jobj2[property.Key] = await ProcessJTokenAsync(property.Value, context, token);
                }
            }

            return result;
        }

        public void AddDataSource<T>()
        {
            _dataSourceTypes.Add(typeof(T));
        }

        public void AddDataSource(Type t)
        {
            _dataSourceTypes.Add(t);
        }

    }
}
