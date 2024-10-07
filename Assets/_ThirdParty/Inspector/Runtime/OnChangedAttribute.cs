/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 값이 바뀌면 호출
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class OnChangedAttribute : PropertyAttribute
    {
        internal readonly string methodName;

        public OnChangedAttribute(string methodNameNoArguments)
        {
            methodName = methodNameNoArguments;
        }
    }
}
