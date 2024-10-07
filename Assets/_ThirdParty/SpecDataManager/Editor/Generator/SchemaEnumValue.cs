/*
* Copyright (c) Sample.
*/

using System.Collections.Generic;
using System.Linq;

namespace Sample.SpecData.Editor.Generator
{
    internal class SchemaEnumValue
    {
        private readonly Dictionary<string, int> _dict = new();
        public string EnumName { get; set; }
        public string Desc { get; set; }

        public void Add(string @enum, int value)
        {
            _dict.Add(@enum, value);
        }

        public int? Get(string @enum)
        {
            if (_dict.TryGetValue(@enum, out int value))
            {
                return value;
            }

            return null;
        }

        public IEnumerable<(string name, int value)> GetAllValue()
        {
            return _dict.Select(x => (x.Key, x.Value)).OrderBy(x => x.Value);
        }
    }
}
