/*
 * Copyright (c) Sample.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Sample.SpecData.Editor.Util;
using ExcelDataReader;
using UnityEngine;

namespace Sample.SpecData.Editor.Generator
{
    internal static class ExcelUtil
    {
        /// '#' 시작하는 sheet, col 무시하자
        private const string IgnoreChar = "#";

        private const string IgnoreCharRow = "IGNORE_ROW";
        private const string IgnoreCharRowCol = "#IGNORE_ROW";

        /// enum 시트
        private const string EnumSheet = "#enum";

        /// enum 시트 value 시작 prefix
        private const string EnumValue = "value:";

        ///
        private const string DateTime = "DateTime";

        public static ConvertSheetInfo ConvertToDataSet(IExcelDataReader excelReader)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                if (excelReader == null)
                {
                    Debug.unityLogger.LogError(Define.Tag, $"{nameof(excelReader)} == null");
                    return default;
                }

                var sheetInfo = new ConvertSheetInfo();
                do
                {
                    // enum 시트
                    if (excelReader.Name.Equals(EnumSheet, StringComparison.OrdinalIgnoreCase))
                    {
                        SchemaEnumValue[] schemaEnumValues = GetExcelSheetEnum(excelReader);
                        sheetInfo.SetSchemaEnumValue(schemaEnumValues);
                        continue;
                    }

                    // 시트
                    DataTable table = GetExcelSheetData(excelReader, excelReader.Name);
                    if (table != null)
                    {
                        sheetInfo.DataSet.Tables.Add(table);
                    }
                } while (excelReader.NextResult()); // 다음 시트 읽자

                return sheetInfo;
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }

        // ReSharper disable once CognitiveComplexity
        private static SchemaEnumValue[] GetExcelSheetEnum(IExcelDataReader excelReader)
        {
            // enum:ENUM_TYPE enum value 시작 형식

            // 설명
            var listDesc = new List<string>();
            var table = new DataTable();
            var setIgnoreCol = new HashSet<int>();

            var dictSchemaEnumValue = new Dictionary<string, SchemaEnumValue>();
            var dictDataColumn = new Dictionary<int, DataColumn>();

            while (excelReader.Read())
            {
                DataRow row = table.NewRow();
                var rowIsEmpty = true;

                for (var i = 0; i < excelReader.FieldCount; i++)
                {
                    // Depth 이면 설명이다.
                    var value = "";
                    if (excelReader.Depth == 0)
                    {
                        value = excelReader.IsDBNull(i) ? "" : Convert.ToString(excelReader.GetValue(i));
                        value = value.Trim();
                        listDesc.Add(value);
                        continue;
                    }

                    if (setIgnoreCol.Contains(i))
                    {
                        continue;
                    }

                    value = excelReader.IsDBNull(i) ? "" : Convert.ToString(excelReader.GetValue(i));
                    value = value.Trim();

                    if (value.StartsWith(IgnoreChar))
                    {
                        setIgnoreCol.Add(i);
                        continue;
                    }

                    if (excelReader.Depth == 1)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            setIgnoreCol.Add(i);
                            continue;
                        }

                        if (table.Columns.Contains(value))
                        {
                            Debug.unityLogger.LogError(Define.Tag,
                                $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} enum:{value} 중복");
                            throw new Exception();
                        }

                        DataColumn column = table.Columns.Add(value);
                        dictDataColumn.Add(i, column);

                        if (!value.StartsWith(EnumValue))
                        {
                            var enumValue = new SchemaEnumValue
                            {
                                Desc = listDesc[i],
                                EnumName = value,
                            };
                            dictSchemaEnumValue.Add(value, enumValue);
                        }

                        continue;
                    }

                    try
                    {
                        if (dictDataColumn.TryGetValue(i, out DataColumn column))
                        {
                            row[column] = value;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.unityLogger.LogError(Define.Tag,
                            $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} 오류 : {e}");
                        throw;
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        rowIsEmpty = false;
                    }
                }

                if (excelReader.Depth > 1 && !rowIsEmpty)
                {
                    table.Rows.Add(row);
                }
            }

            // enum 추출
            foreach (KeyValuePair<string, SchemaEnumValue> pair in dictSchemaEnumValue)
            {
                SchemaEnumValue schemaEnumValue = pair.Value;
                int iEnum = table.Columns.IndexOf(pair.Key);
                int iEnumValue = table.Columns.IndexOf(EnumValue + pair.Key);
                DataColumn columnEnum = table.Columns[iEnum];
                DataColumn columnEnumValue = table.Columns[iEnumValue];

                foreach (DataRow row in table.Rows)
                {
                    var enumName = (string) row[columnEnum];
                    var enumValue = (string) row[columnEnumValue];
                    if (!string.IsNullOrEmpty(enumName) && string.IsNullOrEmpty(enumValue))
                    {
                        throw new Exception($"enum {schemaEnumValue.EnumName} name : ({enumName}) 존재하지만 값이 없습니다.");
                    }

                    if (string.IsNullOrEmpty(enumName) && !string.IsNullOrEmpty(enumValue))
                    {
                        throw new Exception($"enum {schemaEnumValue.EnumName} value : ({enumValue}) 존재하지만 이름이 없습니다.");
                    }

                    if (string.IsNullOrEmpty(enumName) && string.IsNullOrEmpty(enumValue))
                    {
                        continue;
                    }

                    schemaEnumValue.Add(enumName, int.Parse(enumValue!));
                }
            }

            return dictSchemaEnumValue.Values.ToArray();
        }

        // ReSharper disable once CognitiveComplexity
        private static SchemaDataTable GetExcelSheetData(IExcelDataReader excelReader, string strSheetName)
        {
            if (strSheetName.StartsWith(IgnoreChar))
            {
                return default;
            }

            var table = new SchemaDataTable(strSheetName);
            table.Clear();

            var setIgnoreCol = new HashSet<int>();
            var dictDataColumn = new Dictionary<int, DataColumn>();
            var dictColumnOrgName = new Dictionary<string, string>();
            int? ignoreRowCol = null;
            var setIgnoreRow = new HashSet<int>();

            // 설명
            var listDesc = new List<string>(excelReader.FieldCount);
            // id(식별자) columns
            int idColumns = -1;

            while (excelReader.Read())
            {
                DataRow row = default;
                var schemaColIndex = 0;

                for (var i = 0; i < excelReader.FieldCount; i++)
                {
                    string value;

                    // Depth 0 이면 설명이다.
                    if (excelReader.Depth == 0)
                    {
                        value = excelReader.IsDBNull(i) ? "" : Convert.ToString(excelReader.GetValue(i));
                        value = value.Trim();
                        listDesc.Add(value);
                        continue;
                    }

                    if (excelReader.IsDBNull(i) &&
                        (excelReader.Depth == 0 || i > table.Columns.Count - 1))
                    {
                        continue;
                    }

                    value = excelReader.IsDBNull(i) ? "" : Convert.ToString(excelReader.GetValue(i));
                    value = value.Trim();

                    // id col 비어있으면 무시
                    if (i == idColumns && string.IsNullOrEmpty(value))
                    {
                        break;
                    }

                    if (excelReader.Depth == 0 || setIgnoreCol.Contains(i))
                    {
                        continue;
                    }

                    if (value.Equals(IgnoreCharRowCol))
                    {
                        ignoreRowCol = i;
                        continue;
                    }

                    if (value.StartsWith(IgnoreChar) && !value.Equals(IgnoreCharRowCol))
                    {
                        setIgnoreCol.Add(i);
                        continue;
                    }

                    if (setIgnoreRow.Contains(excelReader.Depth))
                    {
                        break;
                    }

                    switch (excelReader.Depth)
                    {
                        case 1: // 이름
                        {
                            try
                            {
                                if (ignoreRowCol == i)
                                {
                                    continue;
                                }

                                string columnId = value.SplitByColon()[0];
                                DataColumn column = table.Columns.Add(columnId);
                                dictDataColumn.Add(i, column);
                                dictColumnOrgName.Add(columnId, value);
                                if (idColumns == -1)
                                {
                                    idColumns = i;
                                }

                                continue;
                            }
                            catch (DuplicateNameException e)
                            {
                                Debug.unityLogger.LogError(Define.Tag,
                                    $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} 중복 이름 오류 : {e}");
                                throw;
                            }
                        }
                        case 2: // 타입
                        {
                            try
                            {
                                if (ignoreRowCol == i)
                                {
                                    continue;
                                }

                                foreach (Schema schema in Schema.Parse(value,
                                             dictColumnOrgName[dictDataColumn[i].ColumnName],
                                             strSheetName,
                                             schemaColIndex, listDesc[i]))
                                {
                                    if (Schema.IsClassSchema(schema))
                                    {
                                        table.SchemaClass.Add(schema);
                                    }
                                    else
                                    {
                                        table.Schema.Add(schema);
                                    }
                                }

                                schemaColIndex++;
                                continue;
                            }
                            catch (KeyNotFoundException)
                            {
                                Debug.unityLogger.LogError(Define.Tag,
                                    $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} 이름이 없습니다.");
                                throw;
                            }
                            catch (SchemeNotFoundException e)
                            {
                                Debug.unityLogger.LogError(Define.Tag,
                                    $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} '{e.SchemeType}' 타입 정의 오류 (name:{e.SchemeType} 형식을 사용해 주세요)");
                                throw;
                            }
                        }
                        default: // value
                        {
                            try
                            {
                                if (ignoreRowCol.HasValue)
                                {
                                    string ignore = excelReader.IsDBNull(i)
                                        ? ""
                                        : Convert.ToString(excelReader.GetValue(i));
                                    ignore = ignore.Trim();
                                    if (ignore.Equals(IgnoreCharRow))
                                    {
                                        setIgnoreRow.Add(excelReader.Depth);
                                        break;
                                    }
                                }

                                // ignore row 있으면 0번 col 무시
                                if (ignoreRowCol.HasValue && i == 0)
                                {
                                    continue;
                                }

                                row ??= table.NewRow();
                                DataColumn column = dictDataColumn[i];
                                row[column] = value;
                            }
                            catch (Exception e)
                            {
                                Debug.unityLogger.LogError(Define.Tag,
                                    $"{excelReader.Name}시트 column:{GetExcelColumnName(i + 1)}, row:{excelReader.Depth + 1} 오류 : {e}");
                                throw;
                            }

                            break;
                        }
                    }
                }

                // row 값 있으면 추가
                if (row != default)
                {
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        /// <summary>
        /// 정수를 시트 컬럼명 (A, B, AA)으로 변경,  1 부터 시작
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private static string GetExcelColumnName(int columnNumber)
        {
            var columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
    }
}
