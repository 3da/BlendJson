using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BlendJson.TypeResolving
{
    public class JsonImplConverter : JsonConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public JsonImplConverter(IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
        }

        public Type Interface { get; init; }
        public Type[] Implementations { get; init; }

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            var typeName = type.Name;

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(type);

            var resultObj = new JObject
            {
                ["@Name"] = typeName
            };



            foreach (var prop in contract.Properties)
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue != null)
                {
                    if (prop.Converter != null)
                    {
                        var writer1 = resultObj.CreateWriter();
                        writer1.WritePropertyName(prop.PropertyName);
                        prop.Converter.WriteJson(writer1, propValue, serializer);
                    }
                    else
                    {
                        resultObj[prop.PropertyName] = JToken.FromObject(propValue, serializer);
                    }

                }
            }

            serializer.Serialize(writer, resultObj);
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
                    return Enumerable.Empty<Type>();
                }
            }).ToArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var types = LoadPossibleImplementations(objectType);

            Type type;

            var token = JToken.Load(reader);

            if (!(token is JObject jObject))
                return null;

            if (types.Length == 0)
            {
                throw new SettingsException($"No classes implement {objectType.Name}");
            }
            else
            {
                var classNameProp = jObject.Property("@Name");

                if (classNameProp == null)
                    throw new SettingsException($"Cannot find property @Name")
                    {
                        JToken = jObject
                    };

                classNameProp.Remove();

                var className = classNameProp.Value.Value<string>();

                ResolveTypeAttribute attribute = objectType.GetCustomAttribute(typeof(ResolveTypeAttribute), true) as ResolveTypeAttribute;
                if (attribute == null)
                {
                    attribute = objectType.GetInterfaces().Select(i => i.GetCustomAttribute(typeof(ResolveTypeAttribute)))
                        .FirstOrDefault(q => q != null) as ResolveTypeAttribute;
                }

                if (attribute != null)
                    className = attribute.TypePrefix + className + attribute.TypePostfix;

                type = types.FirstOrDefault(q => q.Name.Equals(className));

                if (type == null)
                    throw new SettingsException($"Cannot find implementation '{className}' of '{objectType.Name}'");
            }

            object result = null;
            if (_serviceProvider != null)
                result = ActivatorUtilities.CreateInstance(_serviceProvider, type);

            if (result == null)
                result = Activator.CreateInstance(type);

            serializer.Populate(jObject.CreateReader(), result);

            return result;
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

        public override bool CanConvert(Type objectType)
        {
            if (Interface != null)
            {
                return objectType == Interface || Implementations.Contains(objectType);
            }

            return objectType.GetCustomAttribute(typeof(ResolveTypeAttribute), true) != null
                || objectType.GetInterfaces().Any(i => i.GetCustomAttribute(typeof(ResolveTypeAttribute)) != null);
        }
    }
}
