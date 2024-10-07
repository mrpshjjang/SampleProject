/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using System.IO;

namespace Sample.SpecData.Editor.Generator
{
    internal sealed class GeneratorCS_SpecDataValid : GeneratorCS
    {
        private GeneratorCS_SpecDataValid(string ns, string nsEnum, SchemaDataTable table) : base(false)
        {
            WriteLine("#if UNITY_EDITOR");
            WriteLine("// \"Custom 검증 기능 추가\" 참조 해주세요.");
            WriteNewLine();
            WriteUsing("Sample.SpecData");
            WriteNewLine();
            WriteNamespaceValid(ns);
            WriteSchema(nsEnum, table);
            IndentClose();
            AutoClose();
            WriteLine("#endif");
        }

        private void WriteSchema(string nsEnum, SchemaDataTable table)
        {
            WriteLine($"internal class Valid{table.TableName} : IValidator<{table.TableName}>");
            IndentOpen();
            WriteLine($"public bool Validate({table.TableName} data, ISpecDataManager manager)");
            IndentOpen();
            WriteLine("// 검증 코드를 추가해 주세요");
            WriteLine("return true;");
            IndentClose();
            IndentClose();
        }

        public static void CreateCS(string folderRoot, string ns, string nsEnum, IEnumerable<SchemaDataTable> listTable)
        {
            string folder = Path.Combine(folderRoot, "Valid");
            foreach (SchemaDataTable table in listTable)
            {
                var generator = new GeneratorCS_SpecDataValid(ns, nsEnum, table);
                if (generator.Exist(folder, $"Valid{table.TableName}"))
                {
                    continue;
                }
                generator.Save(folder, $"Valid{table.TableName}");
            }
        }
    }
}
