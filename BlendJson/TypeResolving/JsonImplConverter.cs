using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BlendJson.TypeResolving
{
    public class JsonImplConverter : JsonConverter<object>
    {
        public Type? Interface { get; init; }
        public Type[]? Implementations { get; init; }

        private ConcurrentDictionary<object, object> _visitedObjects = new ConcurrentDictionary<object, object>();

        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters = { new EncodingConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (_visitedObjects.TryRemove(value, out _))
            {
                JsonSerializer.Serialize(writer, value, _jsonSerializerOptions);
                return;
            }
            _visitedObjects.TryAdd(value, null);

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var type = value.GetType();

            var attribute = type.GetCustomAttribute<ResolveTypeAttribute>(true);
            if (attribute == null)
            {
                attribute = type.GetInterfaces()
                    .Select(i => i.GetCustomAttribute<ResolveTypeAttribute>(true))
                    .FirstOrDefault(a => a != null);
            }

            var typeName = type.Name;

            if (attribute != null)
                typeName = attribute.TrimClassName(typeName);

            if (JsonSerializer.SerializeToNode(value, options) is JsonObject resultObject)
            {
                resultObject["@Name"] = typeName;
                resultObject.WriteTo(writer);
            }
        }

        private static IList<Type> GetTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies.SelectMany(q =>
            {
                try
                {
                    return q.GetTypes();
                }
                catch
                {
                    return [];
                }
            }).ToArray();
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var types = LoadPossibleImplementations(typeToConvert);

            if (types.Length == 0)
            {
                throw new SettingsException($"No classes implement {typeToConvert.Name}");
            }

            var rootElement = JsonNode.Parse(ref reader);

            if (rootElement is not JsonObject jsonObject)
                return null;

            if (!jsonObject.TryGetPropertyValue("@Name", out var classNameElement))
            {
                throw new SettingsException($"Cannot find property @Name")
                {
                    JToken = jsonObject
                };
            }

            jsonObject.Remove("@Name");

            var className = classNameElement.GetValue<string>();

            var attribute = typeToConvert.GetCustomAttribute<ResolveTypeAttribute>(true);
            if (attribute == null)
            {
                attribute = typeToConvert.GetInterfaces()
                    .Select(i => i.GetCustomAttribute<ResolveTypeAttribute>(true))
                    .FirstOrDefault(a => a != null);
            }

            if (attribute != null)
                className = attribute.TypePrefix + className + attribute.TypePostfix;

            var type = types.FirstOrDefault(q => q.Name.Equals(className));

            if (type == null)
                throw new SettingsException($"Cannot find implementation '{className}' of '{typeToConvert.Name}'");

            object? result = null;


            return rootElement.Deserialize(type, new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                Converters = { new EncodingConverter() }
            });
        }



        private Type[] LoadPossibleImplementations(Type objectType)
        {
            if (Implementations != null)
                return Implementations;

            return (objectType.IsInterface
                ? GetTypes()
                    .Where(p => objectType.IsAssignableFrom(p))
                : GetTypes()
                    .Where(t => t.IsSubclassOf(objectType) || objectType == t)).ToArray();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (Interface != null)
            {
                return typeToConvert == Interface || Implementations!.Contains(typeToConvert);
            }

            return typeToConvert.GetCustomAttribute<ResolveTypeAttribute>(true) != null
                || typeToConvert.GetInterfaces().Any(i => i.GetCustomAttribute<ResolveTypeAttribute>(true) != null);
        }
    }
}
