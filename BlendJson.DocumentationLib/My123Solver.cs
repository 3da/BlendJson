using System;
using Newtonsoft.Json.Serialization;

namespace BlendJson.DocumentationLib
{
    public class My123Solver:DefaultContractResolver
    {
        protected override JsonArrayContract CreateArrayContract(Type objectType)
        {
            var t2 = base.CreateArrayContract(objectType);
            return t2;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var t1 = base.CreateObjectContract(objectType);
            return t1;
        }

        public override JsonContract ResolveContract(Type type)
        {
            var t1 = base.ResolveContract(type);

            return t1;
        }
    }
}
