/*
 * Copyright (c) Sample.
 */

using System.Collections.Generic;
using System.Data;

namespace Sample.SpecData.Editor.Generator
{
    internal class SchemaDataTable : DataTable
    {
        internal SchemaDataTable(string tableName) : base(tableName)
        {
        }

        public readonly List<Schema> Schema = new();
        public readonly List<Schema> SchemaClass = new();
    }
}
