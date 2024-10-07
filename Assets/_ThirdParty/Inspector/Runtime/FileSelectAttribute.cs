/*
* Copyright (c) Sample.
*/

using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 파일 선택
    /// EditorUtility.OpenFilePanelWithFilters 파라메타 참고
    /// </summary>
    /// <example>
    /// <code language="cs">
    /// [FileSelect(new[]{"All files", "*"})]
    /// [FileSelect(new[] {"Keystore", "keystore,jks,ks", "All files", "*"})]
    /// </code>
    /// </example>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class FileSelectAttribute : PropertyAttribute
    {
        public readonly string[] Filters;

        public FileSelectAttribute(string[] filters)
        {
            Filters = filters.Length > 0 ? filters : new[] {"All files", "*"};
        }
    }
}
