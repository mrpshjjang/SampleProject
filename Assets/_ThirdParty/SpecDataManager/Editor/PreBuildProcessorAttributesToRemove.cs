/*
 * Copyright (c) Sample.
 */

using Sample.SpecData.Generator;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Sample.SpecData.Editor
{
    // 난독화 시  Attribute 제거
    internal class PreBuildProcessorAttributesToRemove : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
        }
    }
}
