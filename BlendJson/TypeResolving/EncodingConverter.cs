using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlendJson.TypeResolving
{
    internal class EncodingConverter : JsonConverter<Encoding?>
    {
        public override Encoding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return str == null ? null : Encoding.GetEncoding(str);
        }

        public override void Write(Utf8JsonWriter writer, Encoding? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.WebName);
        }
    }
}
