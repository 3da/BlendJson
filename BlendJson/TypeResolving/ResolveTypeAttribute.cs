using System;

namespace BlendJson.TypeResolving
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class ResolveTypeAttribute : Attribute
    {
        public string TypePrefix { get; set; }
        public string TypePostfix { get; set; }
    }
}
