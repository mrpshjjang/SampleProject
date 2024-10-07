using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sample.SpecData.Generator
{
    [Obfuscation(Exclude = false, ApplyToMembers = true)]
    public static class SpecDataEnumHelper
    {
        /// <summary>
        ///  enum 케싱용
        /// </summary>
        /// <typeparam name = "TEnum"></typeparam>
        private static class EnumCache<TEnum>
        {
            public static readonly Dictionary<string, TEnum> DictEnum;
            static EnumCache()
            {
                Array arrayEnum = Enum.GetValues(typeof(TEnum));
                DictEnum = new Dictionary<string, TEnum>(arrayEnum.Length);
                foreach (TEnum @enum in arrayEnum.Cast<TEnum>())
                {
                    DictEnum.Add(@enum.ToString(), @enum);
                }
            }
        }

        /// <summary>
        /// string to enum
        /// </summary>
        /// <param name = "value"></param>
        /// <typeparam name = "TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum? Parse<TEnum>(string value)
            where TEnum : struct, Enum
        {
            if (EnumCache<TEnum>.DictEnum.TryGetValue(value, out TEnum result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// IEnumerable string to enums
        /// </summary>
        /// <param name = "values"></param>
        /// <typeparam name = "TEnum"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnum> Parse<TEnum>(IEnumerable<string> values)
            where TEnum : struct, Enum
        {
            foreach (string value in values)
            {
                if (EnumCache<TEnum>.DictEnum.TryGetValue(value, out TEnum result))
                {
                    yield return result;
                }
            }
        }
    }
}
