using System;

namespace BlendJson.DocumentationLib
{
    public interface IDescriptionProvider
    {
        string GetForMember(Type type, string memberName);
        string GetForType(Type type);
        string GetForEnumValue(Type type, object enumValue);
    }
}
