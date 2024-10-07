/*
 * Copyright (c) Sample.
 */

using System;

namespace Sample.SpecData.Editor.Generator
{
    internal class SchemeNotFoundException : Exception
    {
        public readonly string SchemeType;

        public SchemeNotFoundException(string schemeType)
        {
            SchemeType = schemeType;
        }
    }
}
