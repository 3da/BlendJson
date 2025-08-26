using System;
using BlendJson.DataSources;

namespace BlendJson.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToExternalAttribute : Attribute
    {
        public string Path { get; set; }

        public LoadMode Mode { get; set; }
    }
}
