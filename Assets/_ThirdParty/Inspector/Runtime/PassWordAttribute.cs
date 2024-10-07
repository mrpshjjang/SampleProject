/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// PassWord *** 표시
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class PassWordAttribute : PropertyAttribute
    {
    }
}
