using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.TypeResolving;
using Newtonsoft.Json;

namespace BlendJson.Serialization
{
    public class SettingsSerializer
    {
        public async Task SaveZipAsync(object obj, string path)
        {
            await using var stream = File.Create(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
            var context = new SerializationContext(new ZipEntryWriter(archive, new Uri(Directory.GetCurrentDirectory() + "\\")))
            {
                GlobalContext = new GlobalContext()
            };

            await context.SaveExternalAsync(context, new FileInfo("Main.json").FullName, LoadMode.Json, obj);
        }


        public async Task SaveJsonAsync(object obj, string path)
        {
            var context = new SerializationContext(new FileSystemWriter())
            {
                GlobalContext = new GlobalContext()
            };

            await context.SaveExternalAsync(context, path, LoadMode.Json, obj);
        }

        public string SaveJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new JsonImplConverter(null)
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });
        }
    }
}
