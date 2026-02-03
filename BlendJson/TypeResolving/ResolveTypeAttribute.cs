using System;

namespace BlendJson.TypeResolving
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class ResolveTypeAttribute : Attribute
    {
        public string TypePrefix { get; set; }
        public string TypePostfix { get; set; }

        public string TrimClassName(string name)
        {
            if (TypePrefix != null && !name.StartsWith(TypePrefix))
                throw new SettingsException($"Type Prefix \"{name}\" doesn't match {nameof(ResolveTypeAttribute)}");

            if (TypePostfix != null && !name.EndsWith(TypePostfix))
                throw new SettingsException($"Type Postfix \"{name}\" doesn't match {nameof(ResolveTypeAttribute)}");

            return name[(TypePrefix?.Length ?? 0)..(name.Length - (TypePostfix?.Length ?? 0))];
        }
    }
}
