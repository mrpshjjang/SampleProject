/*
 * Copyright (c) Sample.
 */

using System;
using System.Collections.Generic;
using Sample.SpecData.Editor.Asset;
using Sample.SpecData.Editor.Util;

namespace Sample.SpecData.Editor.Generator
{
    internal class Schema
    {
        public enum EStyle
        {
            Normal,
            Enum,
            DataReference,
            Id,
            DateTime,
            ClassDataGroupByFieldName,
        }

        public string TypeName { get; private set; }
        public Type Type { get; private set; }
        public EStyle Style { get; private set; } = EStyle.Normal;
        public bool IsArray { get; private set; }
        public string ReferenceType { get; private set; }
        public string VarName { get; private set; }
        public string SheetName { get; private set; }
        public string Desc { get; private set; }

        /// 최종 타입
        public string GetFinalTypeName()
        {
            return TypeName;
        }

        //------------------------------------------------------------------

        private static readonly List<Schema> _listSchema = new();

        public static IEnumerable<Schema> Parse(string str, string varName, string sheetName, int column, string desc)
        {
            _listSchema.Clear();
            var schema = new Schema();
            string[] vars = varName.SplitByColon();
            string[] types = str.SplitByColon();
            schema.TypeName = GetTypeName(types[0]);
            schema.IsArray = CheckIsArray(types[0]);
            schema.VarName = vars[0];
            schema.SheetName = sheetName;
            schema.Desc = desc;

            // 0컬럼은 항상 id
            if (column == 0)
            {
                schema.Style = EStyle.Id;
            }

            // typename enum이면 string 처리
            if (schema.TypeName.Equals("enum", StringComparison.OrdinalIgnoreCase))
            {
                if (types.Length < 2)
                {
                    throw new Exception($"{sheetName}, {varName} enum: 정의가 없습니다.");
                }

                schema.TypeName = "string";
                schema.Style = EStyle.Enum;
            }

            if (schema.TypeName.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                schema.TypeName = "string";
                schema.Style = EStyle.DateTime;
            }

            if (types.Length > 1)
            {
                schema.ReferenceType = types[1];

                // enum이 아니고 Reference참조하고 있다면 다른 Data Sheet 참조다
                if (schema.Style == EStyle.Normal)
                {
                    schema.Style = EStyle.DataReference;
                }
            }

            if (vars.Length > 1)
            {
                // varName:GROUP
                if (vars[1].Equals("GROUP", StringComparison.OrdinalIgnoreCase))
                {
                    var schemaGroup = new Schema
                    {
                        Style = EStyle.ClassDataGroupByFieldName,
                        VarName = vars[0],
                        SheetName = sheetName,
                        Desc = desc,
                    };
                    _listSchema.Add(schemaGroup);
                }
            }

            schema.Type = GetType(schema.TypeName);
            _listSchema.Add(schema);
            return _listSchema;
        }

        private static string GetTypeName(string value)
        {
            int findArray = value.IndexOf('[');
            return findArray == -1 ? value : value[..findArray];
        }

        private static bool CheckIsArray(string value)
        {
            return value.Contains("[]");
        }

        public static bool IsClassSchema(Schema schema)
        {
            return schema.Style == EStyle.ClassDataGroupByFieldName;
        }

        private static Type GetType(string typeName)
        {
            return typeName switch
            {
                "bool" => typeof(bool),
                "int" => typeof(int),
                "long" => typeof(long),
                "float" => typeof(float),
                "double" => typeof(double),
                "string" => typeof(string),
                _ => throw new SchemeNotFoundException(typeName),
            };
        }
    }
}
