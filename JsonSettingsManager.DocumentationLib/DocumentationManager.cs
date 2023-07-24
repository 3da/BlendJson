﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonSettingsManager.TypeResolving;
using Newtonsoft.Json.Serialization;

namespace JsonSettingsManager.DocumentationLib
{
    public class DocumentationManager
    {
        private readonly IContractResolver _contractResolver = new DefaultContractResolver();

        public IDescriptionProvider DescriptionProvider { get; set; }

        public IList<MemberInfo> GenerateForTypes(params Type[] types)
        {

            var context = new Context()
            {
                ProcessedTypes = new List<Type>()
            };
            var result = new List<MemberInfo>();
            foreach (var type in types)
            {
                result.Add(ProcessMember(type.Name, type, context));
            }
            return result;
        }

        private bool ShouldImpl(Type objectType)
        {
            return objectType.GetCustomAttribute(typeof(ResolveTypeAttribute), true) != null
                   || objectType.GetInterfaces().Any(i => i.GetCustomAttribute(typeof(ResolveTypeAttribute)) != null);
        }

        private MemberInfo ProcessMember(string name, Type type, Context context)
        {
            if (!context.ProcessedTypes.Contains(type))
            {
                context = context.Clone();
                context.ProcessedTypes.Add(type);
                var contract = _contractResolver.ResolveContract(type);

                if (contract is JsonPrimitiveContract jsonPrimitiveContract)
                {
                    if (jsonPrimitiveContract.UnderlyingType.IsEnum)
                        return ProcessEnum(name, jsonPrimitiveContract.UnderlyingType, context);

                    return new MemberInfo()
                    {
                        MemberType = MemberType.Primitive,
                        Type = GetTypeFriendlyName(jsonPrimitiveContract.UnderlyingType),
                        Name = name
                    };
                }


                if (contract is JsonArrayContract jsonArrayContract)
                {
                    var elType = jsonArrayContract.CollectionItemType;


                    return new MemberInfo()
                    {
                        MemberType = MemberType.Array,
                        Type = GetTypeFriendlyName(type),
                        Name = name,
                        Children = new List<MemberInfo>()
                    {
                        ProcessMember("Item", elType, context)
                    }

                    };
                }

                if (contract is JsonObjectContract jsonObjectContract)
                    return ProcessClass(name, jsonObjectContract, context);

            }

            return new MemberInfo()
            {
                MemberType = MemberType.Primitive,
                Type = GetTypeFriendlyName(type),
                Name = name
            };
        }

        private string GetTypeFriendlyName(Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(long))
                return "long";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type == typeof(char))
                return "char";

            if (type.IsGenericType)
            {
                var gt = type.GetGenericTypeDefinition();

                if (gt == typeof(Nullable<>))
                    return $"{GetTypeFriendlyName(type.GenericTypeArguments[0])}?";

                if (gt == typeof(IList<>) || gt == typeof(List<>))
                    return $"{GetTypeFriendlyName(type.GenericTypeArguments[0])}[]";

                return $"{gt.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(GetTypeFriendlyName))}>";
            }

            if (type.IsArray)
            {
                return $"{GetTypeFriendlyName(type.GetElementType())}[]";
            }

            return type.Name;
        }

        private MemberInfo ProcessEnum(string name, Type type, Context context)
        {
            return new MemberInfo()
            {
                MemberType = MemberType.Enum,
                Name = name,
                Type = GetTypeFriendlyName(type),
                Implementations = Enum.GetValues(type).Cast<object>().Select(q => new MemberInfo()
                {
                    Name = Enum.GetName(type, q),
                    Type = GetTypeFriendlyName(type.GetEnumUnderlyingType()),
                    Value = Convert.ChangeType(q, type.GetEnumUnderlyingType()).ToString(),
                    MemberType = MemberType.Primitive,
                    Description = DescriptionProvider?.GetForEnumValue(type, q)
                }).ToArray(),
                Description = DescriptionProvider?.GetForType(type)
            };
        }


        private MemberInfo ProcessClass(string name, JsonObjectContract contract, Context context)
        {

            var objectType = contract.UnderlyingType;

            IList<MemberInfo> implementations = null;

            if (ShouldImpl(objectType))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var allTypes = assemblies.SelectMany(q =>
                 {
                     try
                     {
                         return q.GetTypes();
                     }
                     catch
                     {
                         return Enumerable.Empty<Type>();
                     }
                 }).Where(q => !q.IsInterface && !q.IsAbstract).ToArray();


                var types = (objectType.IsInterface
                    ? allTypes.Where(p => objectType.IsAssignableFrom(p))
                    : allTypes.Where(t => t.IsSubclassOf(objectType))).ToArray();

                implementations = types.Any()
                    ? types.Select(q => ProcessMember(q.Name, q, context)).ToArray()
                    : null;
            }

            return new MemberInfo()
            {
                MemberType = MemberType.Class,
                Name = name,
                Type = GetTypeFriendlyName(contract.UnderlyingType),
                Children = contract.Properties.Select(q =>
                {
                    var childInfo = ProcessMember(q.PropertyName, q.PropertyType, context);

                    childInfo.Description = DescriptionProvider?.GetForMember(contract.UnderlyingType, q.PropertyName);

                    return childInfo;
                }).Where(q => q != null).ToArray(),
                Implementations = implementations,
                Description = DescriptionProvider?.GetForType(contract.UnderlyingType)
            };
        }

    }
}
