/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;

namespace Sample.Inspector
{
    /// <summary>
    /// 버튼
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public readonly string Name;

        public ButtonAttribute() { }
        public ButtonAttribute(string name) => Name = name;
    }
}
