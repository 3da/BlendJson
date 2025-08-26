using System.Reflection;
using BlendJson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlendJson.Serialization
{
    public class MySolver : DefaultContractResolver
    {
        private readonly SerializationContext _context;

        public MySolver(SerializationContext context)
        {
            _context = context;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            var externalAttr = member.GetCustomAttribute(typeof(ToExternalAttribute)) as ToExternalAttribute;

            if (externalAttr != null)
                prop.Converter = new ExternalJsonConverter(externalAttr.Path, externalAttr.Mode, _context);

            return prop;
        }
    }
}
