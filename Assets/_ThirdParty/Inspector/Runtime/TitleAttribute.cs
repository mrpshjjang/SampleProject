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
    public class TitleAttribute : PropertyAttribute
    {
        public string Title { get; }
        public bool HorizontalLine { get; }
        public TitleAttribute(string title, bool horizontalLine = true)
        {
            Title = title;
            HorizontalLine = horizontalLine;
        }
    }
}
