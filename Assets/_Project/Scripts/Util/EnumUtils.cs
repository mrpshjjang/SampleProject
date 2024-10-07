using System;
using System.Collections.Generic;


public static class EnumUtils
{
    private static class EnumStringCache<TEnum>
    {
        private static Dictionary<TEnum, string> Dict = null;

        public static string GetString(TEnum value)
        {
            if (Dict == null)
            {
                Dict = new Dictionary<TEnum, string>();
            }

            if (Dict.TryGetValue(value, out string result))
            {
                return result;
            }

            result = value.ToString();
            Dict.Add(value, result);

            return result;
        }
    }

    public static string GetEnumString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return EnumStringCache<TEnum>.GetString(value);
    }

    public static string ToEnumString<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return GetEnumString(value);
    }

    public static string ToEnumStringToLower<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return GetEnumString(value).ToLower();
    }
}
