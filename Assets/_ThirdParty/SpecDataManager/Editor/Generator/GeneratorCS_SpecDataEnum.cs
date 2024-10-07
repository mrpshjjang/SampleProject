/*
* Copyright (c) Sample.
*/

using System.Collections.Generic;

namespace Sample.SpecData.Editor.Generator
{
    internal class GeneratorCS_SpecDataEnum : GeneratorCS
    {
        private GeneratorCS_SpecDataEnum(string ns, IEnumerable<SchemaEnumValue> listEnum)
        {
            WriteNamespace(ns);
            foreach (SchemaEnumValue schemaEnumValue in listEnum)
            {
                WriteNewLine();
                WriteSchema(schemaEnumValue);
            }
            AutoClose();
        }

        private void WriteSchema(SchemaEnumValue table)
        {
            if(!string.IsNullOrEmpty(table.Desc))
                WriteLine($"/// {table.Desc}");
            WriteLine($"public enum {table.EnumName}");
            IndentOpen();
            foreach ((string name, int value) valueTuple in table.GetAllValue())
            {
                WriteLine($"{valueTuple.name} = {valueTuple.value},");
            }
            IndentClose();
        }

        public static void CreateCS(string folder, string ns, IEnumerable<SchemaEnumValue> listEnum)
        {
            var generator = new GeneratorCS_SpecDataEnum(ns, listEnum);
            generator.Save(folder, "SpecEnums");
        }
    }
}
