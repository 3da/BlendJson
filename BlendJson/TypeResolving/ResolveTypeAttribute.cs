using System;

namespace BlendJson.TypeResolving
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class ResolveTypeAttribute : Attribute
	{
	}
}
