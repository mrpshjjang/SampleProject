/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class TextLabelAttribute : PropertyAttribute
    {
        public string Text { get; }
        public TextLabelAttribute(string text)
        {
            Text = text;
        }

    }
}
