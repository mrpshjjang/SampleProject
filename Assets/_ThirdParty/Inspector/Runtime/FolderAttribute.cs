using System;
using System.Diagnostics;
using UnityEngine;

namespace Sample.Inspector
{
    /// <summary>
    /// 폴더 경로 적용 (string필드 적용)
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class FolderAttribute : PropertyAttribute
    {
        public FolderAttribute(bool AbsolutePath = false)
        {
            this.AbsolutePath = AbsolutePath;
        }

        public bool AbsolutePath { get; }
    }
}
