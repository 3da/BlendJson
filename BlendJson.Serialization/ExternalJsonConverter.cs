using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BlendJson.DataSources;
using BlendJson.SpecificProcessors.Options;

namespace BlendJson.Serialization
{
    public class ExternalJsonConverter : JsonConverter<object>
    {
        private readonly string _path;
        private readonly LoadMode _mode;
        private readonly SerializationContext _context;
        private readonly JsonSerializerOptions _jsonSerializer;

        public ExternalJsonConverter(string externalAttrPath,
            LoadMode mode,
            SerializationContext context)
        {
            _path = externalAttrPath;
            var fi = new FileInfo(_path);
            if (fi.Extension.Length == 0)
            {
                switch (mode)
                {
                    case LoadMode.Json:
                        _path += ".json";
                        break;
                    case LoadMode.Text:
                        _path += ".txt";
                        break;
                    case LoadMode.Bin:
                    case LoadMode.LargeBin:
                        _path += ".bin";
                        break;
                    case LoadMode.Lines:
                        _path += ".txt";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }

            }

            _mode = mode;
            _context = context;
            _jsonSerializer = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            foreach (var converter in _context.JsonConverters)
            {
                _jsonSerializer.Converters.Add(converter);
            }
        }

    

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            string path;
            if (_path.Contains("{id}"))
            {
                var id = _context.GlobalContext.GetFileId();
                path = _path.Replace("{id}", id.ToString("0000"));
            }
            else
            {
                path = _path;
            }


            _context.SaveExternalAsync(_context, path, _mode, value).Wait();


            var opt = new LoadOptions()
            {
                Mode = _mode,
                DataSource = _context.Writer.CreateDataSource(path)
            };

            var obj = new JsonObject()
            {
                ["@LoadFrom"] = JsonSerializer.SerializeToNode(opt, _jsonSerializer)
            };

            obj.WriteTo(writer, options);
        }
    }
}
