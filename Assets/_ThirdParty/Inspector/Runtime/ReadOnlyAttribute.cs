/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 읽기 전용
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        internal readonly bool runtimeOnly;

        public ReadOnlyAttribute(bool runtimeOnly = false)
        {
            this.runtimeOnly = runtimeOnly;
        }
    }
}
