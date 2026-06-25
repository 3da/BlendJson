using BlendJson.Serialization.Attributes;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace BlendJson.Serialization
{
    public class MyResolver : DefaultJsonTypeInfoResolver
    {
        private readonly SerializationContext _context;

        public MyResolver(SerializationContext context)
        {
            _context = context;
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var typeInfo = base.GetTypeInfo(type, options);

            if (typeInfo.Kind == JsonTypeInfoKind.Object)
            {
                foreach (var property in typeInfo.Properties)
                {
                    var externalAttr = property.AttributeProvider
                        ?.GetCustomAttributes(typeof(ToExternalAttribute), true)
                        ?.FirstOrDefault() as ToExternalAttribute;

                    if (externalAttr != null)
                    {
                        property.CustomConverter = new ExternalJsonConverter(
                            externalAttr.Path,
                            externalAttr.Mode,
                            _context
                        );
                    }
                }
            }

            return typeInfo;
        }
    }
}