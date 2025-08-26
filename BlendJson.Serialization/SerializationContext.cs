using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.TypeResolving;
using Newtonsoft.Json;

namespace BlendJson.Serialization
{
    public class SerializationContext
    {
        public SerializationContext(IWriter writer)
        {
            Writer = writer;
        }

        public string WorkDir { get; set; }

        public GlobalContext GlobalContext { get; set; }

        public IWriter Writer { get; }
        public List<JsonConverter> JsonConverters { get; set; }

        public async Task SaveExternalAsync(SerializationContext context, string path, LoadMode mode, object value)
        {
            var newPath = path;

            if (!string.IsNullOrEmpty(WorkDir))
                newPath = Path.Combine(WorkDir, newPath);

            var fi = new FileInfo(newPath);

            var dir = fi.Directory;

            if (!dir.Exists)
                dir.Create();

            var newContext = new SerializationContext(context.Writer)
            {
                WorkDir = WorkDir ?? dir.FullName,
                GlobalContext = context.GlobalContext,
                JsonConverters = context.JsonConverters
            };


            var serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                ContractResolver = new MySolver(newContext),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = JsonConverters,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });

            switch (mode)
            {
                case LoadMode.Json:
                    using (var memoryStream = new MemoryStream())
                    {
                        var streamWriter = new StreamWriter(memoryStream);
                        serializer.Serialize(streamWriter, value);
                        await streamWriter.FlushAsync();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Writer.Write(newPath, memoryStream);
                    }

                    break;
                case LoadMode.Text:
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes((string)value)))
                        context.Writer.Write(newPath, stream);
                    break;
                case LoadMode.Bin:
                    using (var stream = new MemoryStream((byte[])value))
                        context.Writer.Write(newPath, stream);
                    break;
                case LoadMode.LargeBin:
                    var bytes = (byte[][])value;
                    var streams = bytes.Select(b => (Stream)new MemoryStream(b)).ToArray();
                    context.Writer.Write(newPath, streams);
                    foreach (var stream in streams)
                    {
                        await stream.DisposeAsync();
                    }

                    break;
                case LoadMode.Lines:
                    using (var memoryStream = new MemoryStream())
                    {
                        var streamWriter = new StreamWriter(memoryStream);
                        foreach (string s in (IEnumerable<string>)value)
                        {
                            await streamWriter.WriteLineAsync(s);
                        }

                        await streamWriter.FlushAsync();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        context.Writer.Write(newPath, memoryStream);

                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }


        }
    }
}
