/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using System.Linq;
using static System.Char;

namespace Sample.SpecData.Editor.Util
{
    internal static class StringUtil
    {
        private static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !IsWhiteSpace(c))
                .ToArray());
        }

        private static string[] Trim(this IEnumerable<string> input)
        {
            return input.Select(x => x.Trim()).ToArray();
        }

        public static string[] SplitByColon(this string input)
        {
            return input.RemoveWhitespace().Split(':').Trim();
        }
    }
}
