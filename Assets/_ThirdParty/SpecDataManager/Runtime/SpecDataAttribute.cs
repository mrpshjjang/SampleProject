using System;
using System.Reflection;

namespace Sample.SpecData.Generator
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GeneratorIdAttribute : Attribute
    {
        public string IDName { get; }

        public string IDType { get; }

        public GeneratorIdAttribute(string idName, Type idType)
        {
            IDName = idName;
            IDType = idType.FullName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GeneratorSpecDataAttribute : Attribute
    {
    }

    [Flags]
    [Obfuscation(Exclude = false, ApplyToMembers = true)]
    public enum EnumSpecDataManagerType
    {
        /// <summary>
        /// Unity JsonUtility 사용 (메모리 사용량 적음, 로딩 빠름, Obfuscator 지원 안됨)
        /// </summary>
        JSON_UNITY = 1 << 0,

        /// <summary>
        /// Newtonsoft.Json 사용 (JSON_UNITY 보다 메모리 많이 사용, 로딩 느림, Obfuscator 지원)
        /// </summary>
        JSON_NET = 1 << 1,

        /// <summary>
        /// 검증기 지원
        /// </summary>
        VALIDATOR = 1 << 2,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GeneratorSpecDataManagerAttribute : Attribute
    {
        public EnumSpecDataManagerType Flag { get; }

        public GeneratorSpecDataManagerAttribute(EnumSpecDataManagerType flag)
        {
            Flag = flag;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class GeneratorIdToDataAttribute : Attribute
    {
        public GeneratorIdToDataAttribute(string idName, Type targetType)
        {
            IDName = idName;
            TargetType = targetType.FullName;
        }

        public string IDName { get; }

        public string TargetType { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class GeneratorIdToEnumAttribute : Attribute
    {
        public GeneratorIdToEnumAttribute(string idName, Type enumType)
        {
            IDName = idName;
            EnumType = enumType.FullName;
        }

        public string IDName { get; }

        public string EnumType { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class GeneratorSpecDataResourceAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class GeneratorDateTimeAttribute : Attribute
    {
        public string IDName { get; }

        public GeneratorDateTimeAttribute(string idName)
        {
            IDName = idName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GeneratorSpecDataGroupByFieldNameAttribute : Attribute
    {
        public string GroupFieldName { get; }

        public GeneratorSpecDataGroupByFieldNameAttribute(string groupFieldName)
        {
            GroupFieldName = groupFieldName;
        }
    }
}
