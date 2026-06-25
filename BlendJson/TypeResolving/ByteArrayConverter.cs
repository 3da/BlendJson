using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlendJson.TypeResolving
{
    public class ByteArrayConverter : JsonConverter<byte[][]>
    {
        public override byte[][] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Пробуем определить, является ли первый элемент числом (байтом)
                var readerCopy = reader;
                readerCopy.Read();

                if (readerCopy.TokenType == JsonTokenType.Number)
                {
                    // Это массив байтов - читаем как byte[]
                    var byteArray = JsonSerializer.Deserialize<byte[]>(ref reader, options);
                    return new byte[][] { byteArray };
                }

                // Это массив массивов - читаем как byte[][]
                return JsonSerializer.Deserialize<byte[][]>(ref reader, options);
            }

            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, byte[][] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}