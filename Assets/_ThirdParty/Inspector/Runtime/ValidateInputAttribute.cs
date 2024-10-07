/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 메소드 조건으로 검증
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class ValidateInputAttribute : PropertyAttribute
    {
        internal string Message { get; private set; }
        internal string MethodName { get; private set; }
        public ValidateInputAttribute(string methodName, string message = null)
        {
            MethodName = methodName;
            Message = message;
        }
    }
}
