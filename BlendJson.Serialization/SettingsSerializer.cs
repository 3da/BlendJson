using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.TypeResolving;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlendJson.Serialization
{
    public class SettingsSerializer
    {
        private readonly List<Type> _dataSourceTypes = SettingsLoader.GetDefaultDataSources();

        private List<JsonConverter> GetJsonConverters() =>
            new List<JsonConverter>()
            {
                new StringEnumConverter(),
                new JsonImplConverter(null)
                {
                    Interface = typeof(IDataSource),
                    Implementations = _dataSourceTypes.ToArray()
                },
                new JsonImplConverter(null)
            };

        public async Task SaveZipAsync(object obj, string path)
        {
            await using var stream = File.Create(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            var context = new SerializationContext(new ZipEntryWriter(archive, Directory.GetCurrentDirectory()))
            {
                GlobalContext = new GlobalContext(),
                JsonConverters = GetJsonConverters()
            };

            await context.SaveExternalAsync(context, new FileInfo("Main.json").FullName, LoadMode.Json, obj);
        }


        public async Task SaveJsonAsync(object obj, string path)
        {
            var context = new SerializationContext(new FileSystemWriter())
            {
                GlobalContext = new GlobalContext(),
                JsonConverters = GetJsonConverters()
            };

            await context.SaveExternalAsync(context, path, LoadMode.Json, obj);
        }

        public string SaveJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = GetJsonConverters(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });
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
