/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEditor;
using UnityEngine;

namespace CookApps.PackageTemplate.Editor
{
    [CustomEditor(typeof(TechPackageAsset))]
    internal class TechPackageAssetDrawer : UnityEditor.Editor
    {
        private TechPackageAsset Asset => target as TechPackageAsset;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if(GUILayout.Button("매뉴얼", GUILayout.Height(30)))
            {
                Application.OpenURL("https://docs.tech.cookapps.com/release/com.cookapps.package.template-helper/manual/getting_started.html");
            }

            if(GUILayout.Button("GitHub CLI 초기화 실행", GUILayout.Height(30)))
            {
                TechPackage.GitHubInitialize();
            }

            if(GUILayout.Button("package.json에서 불러오기", GUILayout.Height(30)))
            {
                TechPackage.LoadAsset(Asset);
            }

            if(GUILayout.Button("저장", GUILayout.Height(30)))
            {
                TechPackage.SaveAsset(Asset);
            }

            if(GUILayout.Button("저장 & GitHub 동기화", GUILayout.Height(30)))
            {
                TechPackage.SaveAsset(Asset);
                TechPackage.GitHubSync(Asset);
            }

            if(GUILayout.Button("GitHub browse 열기", GUILayout.Height(30)))
            {
                ExecuteCommand.RunGitHubCli("gh browse");
            }
        }
    }
}
