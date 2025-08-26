using System;
using System.Collections.Generic;
using System.IO;
using BlendJson.DataSources;
using BlendJson.SpecificProcessors.Options;
using BlendJson.TypeResolving;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace BlendJson.Serialization
{
    public class ExternalJsonConverter : JsonConverter
    {
        private readonly string _path;
        private readonly LoadMode _mode;
        private readonly SerializationContext _context;
        private readonly JsonSerializer _jsonSerializer;

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
            _jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = context.JsonConverters
            });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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

            var obj = new JObject()
            {
                ["@LoadFrom"] = JObject.FromObject(opt, _jsonSerializer)
            };

            serializer.Serialize(writer, obj);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new InvalidOperationException();
        }
    }
}
