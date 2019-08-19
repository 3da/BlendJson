﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JsonSettingsManager.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace JsonSettingsManager.Serialization
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
