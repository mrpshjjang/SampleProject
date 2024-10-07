/*
* Copyright (c) Sample.
*/

using Sample.SpecData.Editor.Asset;

namespace Sample.SpecData.Editor.Generator
{
    internal sealed class GeneratorCS_SpecDataManager : GeneratorCS
    {
        private GeneratorCS_SpecDataManager(string ns)
        {
            WriteUsing("Sample.SpecData.Generator");
            WriteNewLine();
            WriteNamespace(ns);

            WriteClass("SpecDataManager", true, false);
            IndentClose();

            WriteNewLine();

            WriteClass("SpecDataValidManager", true, false);
            IndentClose();

            AutoClose();
        }
        public static void CreateCS(string folder, string ns)
        {
            var generator = new GeneratorCS_SpecDataManager(ns);
            generator.Save(folder, "SpecDataManager");
        }
    }
}
