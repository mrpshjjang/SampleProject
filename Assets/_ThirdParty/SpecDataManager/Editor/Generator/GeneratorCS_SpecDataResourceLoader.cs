/*
* Copyright (c) Sample.
*/

using System.Text;

namespace Sample.SpecData.Editor.Generator
{
    internal sealed class GeneratorCS_SpecDataResourceLoader : GeneratorCS
    {
        private GeneratorCS_SpecDataResourceLoader(string ns, string encryptKey)
        {
            WriteUsing("System.Linq");
            WriteUsing("System.Security.Cryptography");
            WriteUsing("System.Text");
            WriteUsing("Sample.SpecData.Generator");
            WriteNewLine();
            WriteNamespace(ns);
            WriteLine("[GeneratorSpecDataResource]");
            WriteClass("SpecDataResourceLoader", true, true);
            WriteLine(GenKey(encryptKey));
            AutoClose();
        }

        /// <summary>
        /// encryptKey 암호화 해서 byte[] 생성
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <returns></returns>
        private static string GenKey(string encryptKey)
        {
            var builder = new StringBuilder();

            byte[] byteKey = Encoding.UTF8.GetBytes(encryptKey);
            for (byte i = 0; i < byteKey!.Length; i++)
            {
                byte b = byteKey[i];
                int v = b ^ i;
                byteKey[i] = (byte)v;
            }

            builder.Append("private static readonly byte[] _key = {");
            builder.Append(string.Join(", ", byteKey));
            builder.Append("};");
            return builder.ToString();
        }

        public static void CreateCS(string folder, string ns, string encryptKey)
        {
            var generator = new GeneratorCS_SpecDataResourceLoader(ns, encryptKey);
            generator.Save(folder, "SpecDataResourceLoader");
        }

        private static byte[] GeneratorKey(byte[] key)
        {
            var byteKey = key.Clone() as byte[];
            for (byte i = 0; i < byteKey!.Length; i++)
            {
                byte b = byteKey[i];
                int v = b ^ i;
                byteKey[i] = (byte)v;
            }

            return byteKey;
        }
    }
}
