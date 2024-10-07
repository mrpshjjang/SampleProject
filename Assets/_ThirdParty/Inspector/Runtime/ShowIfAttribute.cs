/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 조건 bool (Property) true값이면 show
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        internal string Condition { get; }
        internal bool Value { get; }

        public ShowIfAttribute(string condition, bool value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
