using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson.Configuration
{
    public sealed class JsonConfigurationFileParser
    {
        private JsonConfigurationFileParser() { }

        private readonly Dictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _paths = new();

        public static IDictionary<string, string?> Parse(Stream input)
            => new JsonConfigurationFileParser().ParseStream(input);

        public static IDictionary<string, string?> Parse(string input)
            => new JsonConfigurationFileParser().ParseString(input);

        public static IDictionary<string, string?> Parse(JsonElement element)
            => new JsonConfigurationFileParser().ParseJson(element);

        public static IDictionary<string, string?> Parse(JObject jToken)
            => new JsonConfigurationFileParser().ParseJson(jToken);

        private Dictionary<string, string?> ParseStream(Stream input)
        {
            using var reader = new StreamReader(input);
            return ParseString(reader.ReadToEnd());
        }

        private Dictionary<string, string?> ParseString(string input)
        {
            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (var doc = JsonDocument.Parse(input, jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException("Invalid topLevelJSONElement: " + doc.RootElement.ValueKind);
                }
                VisitObjectElement(doc.RootElement);
            }

            return _data;
        }

        private Dictionary<string, string?> ParseJson(JsonElement jsonElement)
        {
            VisitObjectElement(jsonElement);

            return _data;
        }

        private Dictionary<string, string?> ParseJson(JObject jToken)
        {
            VisitObjectElement(jToken);

            return _data;
        }

        private void VisitObjectElement(JsonElement element)
        {
            var isEmpty = true;

            foreach (var property in element.EnumerateObject())
            {
                isEmpty = false;
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }

            SetNullIfElementIsEmpty(isEmpty);
        }

        private void VisitObjectElement(JObject element)
        {
            var isEmpty = true;

            foreach (var property in element.Properties())
            {
                isEmpty = false;
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }

            SetNullIfElementIsEmpty(isEmpty);
        }

        private void VisitArrayElement(JsonElement element)
        {
            var index = 0;

            foreach (var arrayElement in element.EnumerateArray())
            {
                EnterContext(index.ToString());
                VisitValue(arrayElement);
                ExitContext();
                index++;
            }

            SetEmptyIfElementIsEmpty(isEmpty: index == 0);
        }

        private void VisitArrayElement(JArray element)
        {
            var index = 0;

            foreach (var arrayElement in element)
            {
                EnterContext(index.ToString());
                VisitValue(arrayElement);
                ExitContext();
                index++;
            }

            SetEmptyIfElementIsEmpty(isEmpty: index == 0);
        }

        private void SetNullIfElementIsEmpty(bool isEmpty)
        {
            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null;
            }
        }

        private void SetEmptyIfElementIsEmpty(bool isEmpty)
        {
            if (isEmpty && _paths.Count > 0)
            {
                _data[_paths.Peek()] = string.Empty;
            }
        }

        private void VisitValue(JsonElement value)
        {
            Debug.Assert(_paths.Count > 0);

            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitObjectElement(value);
                    break;

                case JsonValueKind.Array:
                    VisitArrayElement(value);
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    string key = _paths.Peek();
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException("Duplicated key: " + key);
                    }
                    _data[key] = value.ValueKind == JsonValueKind.Null ? null : value.ToString();
                    break;

                default:
                    throw new FormatException("Unsupported value kind: " + value.ValueKind);
            }
        }

        private void VisitValue(JToken value)
        {
            Debug.Assert(_paths.Count > 0);

            switch (value)
            {
                case JObject obj:
                    VisitObjectElement(obj);
                    break;
                case JArray arr:
                    VisitArrayElement(arr);
                    break;
               default:
                    string key = _paths.Peek();
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException("Duplicated key: " + key);
                    }
                    _data[key] = value.ToString();
                   break;
            }
        }

        private void EnterContext(string context) =>
            _paths.Push(_paths.Count > 0 ?
                _paths.Peek() + ConfigurationPath.KeyDelimiter + context :
                context);

        private void ExitContext() => _paths.Pop();
    }
}
