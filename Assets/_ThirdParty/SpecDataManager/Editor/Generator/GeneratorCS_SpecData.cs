/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using System.Linq;

namespace Sample.SpecData.Editor.Generator
{
    internal sealed class GeneratorCS_SpecData : GeneratorCS
    {
        private GeneratorCS_SpecData(string ns, string nsEnum, IEnumerable<SchemaDataTable> listTable)
        {
            WriteUsing("System");
            WriteUsing("Sample.SpecData.Generator");
            WriteNewLine();
            WriteNamespace(ns);
            foreach (SchemaDataTable table in listTable)
            {
                WriteNewLine();
                WriteSchema(nsEnum, table);
                IndentClose();
            }

            AutoClose();
        }

        private void WriteSchema(string nsEnum, SchemaDataTable table)
        {
            WriteLine("[Serializable]");

            foreach (Schema schema in table.SchemaClass)
            {
                if (schema.Style == Schema.EStyle.ClassDataGroupByFieldName)
                {
                    WriteLine($"[GeneratorSpecDataGroupByFieldName(nameof({schema.VarName}))]");
                }
            }

            WriteClass(table.TableName, true, false);

            foreach (Schema schema in table.Schema)
            {
                if (!string.IsNullOrEmpty(schema.Desc))
                {
                    WriteLine($"/// {schema.Desc}");
                }

                if (schema.Style == Schema.EStyle.Id)
                {
                    WriteLine($"[GeneratorId(nameof({schema.VarName}), typeof({schema.GetFinalTypeName()}))]");
                }

                if (schema.Style == Schema.EStyle.DataReference)
                {
                    WriteLine($"[GeneratorIdToData(nameof({schema.VarName}), typeof({schema.ReferenceType}))]");
                }

                if (schema.Style == Schema.EStyle.DateTime)
                {
                    WriteLine($"[GeneratorDateTime(nameof({schema.VarName}))]");
                }

                if (schema.Style == Schema.EStyle.Enum)
                {
                    string ns = !string.IsNullOrEmpty(nsEnum) ? $"{nsEnum}." : string.Empty;
                    string array = schema.IsArray ? "[]" : string.Empty;
                    if (schema.IsArray)
                    {
                        WriteLine(
                            $"public global::{ns}{schema.ReferenceType}{array} {schema.VarName} = global::System.Array.Empty<global::{ns}{schema.ReferenceType}>();");
                    }
                    else
                    {
                        WriteLine($"public global::{ns}{schema.ReferenceType}{array} {schema.VarName};");
                    }
                }
                else
                {
                    string array = schema.IsArray ? "[]" : string.Empty;
                    if (schema.IsArray)
                    {
                        WriteLine(
                            $"public {schema.GetFinalTypeName()}{array} {schema.VarName} = global::System.Array.Empty<{schema.GetFinalTypeName()}>();");
                    }
                    else
                    {
                        WriteLine($"public {schema.GetFinalTypeName()}{array} {schema.VarName};");
                    }
                }
            }
        }

        public static void CreateCS(string folder, string ns, string nsEnum, IEnumerable<SchemaDataTable> listTable)
        {
            var generator = new GeneratorCS_SpecData(ns, nsEnum, listTable);
            generator.Save(folder, "SpecDatas");
        }
    }
}
