/*
 * Copyright (c) Sample.
 */

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Sample.SpecData.Editor.Generator;
using UnityEngine;

namespace Sample.SpecData.Editor.Util
{
    internal static class DataUtil
    {
        private const char ArrayDelimiter = '/';

        public static object GetValue(this DataRow dataRow, Schema schema, ConvertSheetInfo sheetInfo)
        {
            var value = dataRow[schema.VarName] as string;
            if (schema.IsArray)
            {
                return GetArrayValue(value, schema, sheetInfo);
            }

            return GetValue(value, schema, sheetInfo);
        }

        private static object GetArrayValue(string str, Schema schema, ConvertSheetInfo sheetInfo)
        {
            try
            {
                Type type = schema.Type;
                // 빈경우
                if (string.IsNullOrEmpty(str))
                {
                    if (type == typeof(bool))
                    {
                        return Array.Empty<int>(); // bool타입은 0, 1 int 형식으로
                    }

                    if (type == typeof(int))
                    {
                        return Array.Empty<int>();
                    }

                    if (type == typeof(long))
                    {
                        return Array.Empty<long>();
                    }

                    if (type == typeof(float))
                    {
                        return Array.Empty<float>();
                    }

                    if (type == typeof(double))
                    {
                        return Array.Empty<double>();
                    }

                    if (type == typeof(string))
                    {
                        return Array.Empty<string>();
                    }

                    throw new AggregateException();
                }

                string[] values = str.Split(ArrayDelimiter).Select(x => x.Trim()).ToArray();

                // enum
                if (schema.Style == Schema.EStyle.Enum)
                {
                    return GetArrValueEnum(values, schema, sheetInfo);
                }

                if (type == typeof(bool))
                {
                    return GetArrValue<int>(values, schema, sheetInfo); // bool타입은 0, 1 int 형식으로
                }

                if (type == typeof(int))
                {
                    return GetArrValue<int>(values, schema, sheetInfo);
                }

                if (type == typeof(long))
                {
                    return GetArrValue<long>(values, schema, sheetInfo);
                }

                if (type == typeof(float))
                {
                    return GetArrValue<float>(values, schema, sheetInfo);
                }

                if (type == typeof(double))
                {
                    return GetArrValue<double>(values, schema, sheetInfo);
                }

                if (type == typeof(string))
                {
                    return values;
                }

                throw new AggregateException();
            }
            catch
            {
                Debug.LogError($"Schema Array Type ({schema.SheetName}, {schema.VarName}, {schema.Type}) 값 오류 : {str}");
                throw;
            }

            T[] GetArrValue<T>(string[] values, Schema schema, ConvertSheetInfo sheetInfo) where T : struct
            {
                var arr = new T[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    var value = (T) GetValue(values[i], schema, sheetInfo);
                    arr[i] = value;
                }

                return arr;
            }

            int[] GetArrValueEnum(string[] values, Schema schema, ConvertSheetInfo sheetInfo)
            {
                var arr = new int[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    var value = (int) GetValue(values[i], schema, sheetInfo);
                    arr[i] = value;
                }

                return arr;
            }
        }

        private static object GetValue(string str, Schema schema, ConvertSheetInfo sheetInfo)
        {
            try
            {
                Type type = schema.Type;

                // enum string -> int value
                if (schema.Style == Schema.EStyle.Enum)
                {
                    int? enumValue = sheetInfo.GetSchemaEnumValue(schema.ReferenceType, str);
                    if (!enumValue.HasValue)
                    {
                        throw new AggregateException(
                            $"{schema.SheetName} 시트 enum ({schema.ReferenceType}) 타입 {str} 없음");
                    }

                    return enumValue.Value;
                }

                if (string.IsNullOrEmpty(str))
                {
                    if (type == typeof(bool))
                    {
                        return default(bool);
                    }

                    if (type == typeof(int))
                    {
                        return default(int);
                    }

                    if (type == typeof(long))
                    {
                        return default(long);
                    }

                    if (type == typeof(float))
                    {
                        return default(float);
                    }

                    if (type == typeof(double))
                    {
                        return default(double);
                    }

                    if (type == typeof(string))
                    {
                        return string.Empty;
                    }

                    throw new AggregateException();
                }

                if (type == typeof(bool))
                {
                    return int.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture); // bool타입은 0, 1 int 형식으로
                }

                if (type == typeof(int))
                {
                    return int.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture);
                }

                if (type == typeof(long))
                {
                    return long.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture);
                }

                if (type == typeof(float))
                {
                    return float.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
                }

                if (type == typeof(double))
                {
                    return double.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
                }

                if (type == typeof(string))
                {
                    return str;
                }

                throw new AggregateException();
            }
            catch
            {
                Debug.LogError($"Schema ({schema.SheetName}, {schema.VarName}, {schema.Type}) 값 오류 : {str}");
                throw;
            }
        }
    }
}
