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
    public class RequiredAttribute : PropertyAttribute
    {
        public override string ToString()
        {
            return "[Required]";
        }
    }
}
