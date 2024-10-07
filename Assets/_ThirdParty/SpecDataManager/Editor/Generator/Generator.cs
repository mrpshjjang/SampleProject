/*
 * Copyright (c) Sample.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sample.SpecData.Editor.Util;
using ExcelDataReader;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sample.SpecData.Editor.Generator
{
    internal class ConvertSheetInfo
    {
        public DataSet DataSet { get; } = new();

        private Dictionary<string, SchemaEnumValue> _dictSchemaEnumValue;

        public IEnumerable<SchemaEnumValue> SchemaEnumValues => _dictSchemaEnumValue.Values;
        public bool HasEnum => _dictSchemaEnumValue != default && _dictSchemaEnumValue.Count > 0;

        public void SetSchemaEnumValue([NotNull] SchemaEnumValue[] schemaEnumValues)
        {
            if (schemaEnumValues == null)
            {
                throw new ArgumentNullException(nameof(schemaEnumValues));
            }

            _dictSchemaEnumValue = new Dictionary<string, SchemaEnumValue>();
            foreach (SchemaEnumValue schemaEnumValue in schemaEnumValues)
            {
                _dictSchemaEnumValue.Add(schemaEnumValue.EnumName, schemaEnumValue);
            }
        }

        public SchemaEnumValue GetSchemaEnumValue(string enumName)
        {
            if (_dictSchemaEnumValue == default)
            {
                return default;
            }

            _dictSchemaEnumValue.TryGetValue(enumName, out SchemaEnumValue value);
            return value;
        }

        public int? GetSchemaEnumValue(string enumName, string enumValue)
        {
            return GetSchemaEnumValue(enumName)?.Get(enumValue);
        }
    }

    internal static class Generator
    {
        public static ConvertSheetInfo ConvertDataSet([NotNull] IExcelDataReader reader)
        {
            return ExcelUtil.ConvertToDataSet(reader);
        }

        public static string ConvertJson(ConvertSheetInfo convertSheetInfo)
        {
            var json = new JObject();
            foreach (SchemaDataTable data in convertSheetInfo.DataSet.Tables.OfType<SchemaDataTable>())
            {
                var array = new JArray();
                List<Schema> schema = data.Schema;
                for (var i = 0; i < data.Rows.Count; i++)
                {
                    var obj = new JObject();
                    DataRow row = data.Rows[i];
                    foreach (Schema s in schema)
                    {
                        object value = row.GetValue(s, convertSheetInfo);
                        JToken token;
                        if (s.IsArray)
                        {
                            token = new JArray(value);
                        }
                        else
                        {
                            token = new JValue(value);
                        }

                        obj[s.VarName] = token;
                    }

                    array.Add(obj);
                }

                json[data.TableName] = array;
            }

            return json.ToString(Formatting.None);
        }
    }
}
