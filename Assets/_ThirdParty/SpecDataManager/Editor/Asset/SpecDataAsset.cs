/*
 * Copyright (c) Sample.
 */

using System.IO;
using Sample.Inspector;
using Sample.SpecData.Editor.Inspector;
using UnityEditor;
using UnityEngine;

namespace Sample.SpecData.Editor.Asset
{
    internal class SpecDataAsset : ScriptableObject
    {
        [Header("구글 설정")]
        [Tooltip("Sheet 제목 필터링")]
        public string FilterTitle;

        public string SheetId;

        [ReadOnly]
        public string SheetName;

        [Space]
        [Header("cs 생성")]
        [Tooltip("생성 cs파일 namespace")]
        public string SourceNamespace;

        [Tooltip("생성 enum cs파일 namespace")]
        public string SourceEnumNamespace;

        [Tooltip("생성 cs파일 폴더 위치")]
        [Folder]
        public string SourceFolder;

        [Tooltip("원본 Json 생성 폴더 위치")]
        [Folder]
        public string OriginalJsonFolder;

        [Tooltip("검증 cs 생성")]
        public bool CreateValidFile;

        [Space]
        [Header("Build 설정")]
        [Tooltip("빌드 전 검증")]
        public bool PreBuildValid;

        [Space]
        [Header("Json 설정")]
        [ButtonEncryptKey]
        public string EncryptKey;

        public static SpecDataAsset GetAssets()
        {
            string path = Define.AssetPath;
            var asset = AssetDatabase.LoadAssetAtPath<SpecDataAsset>(path);
            if (asset != null)
            {
                return asset;
            }

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
                AssetDatabase.Refresh();
            }

            asset = CreateInstance<SpecDataAsset>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.ForceReserializeAssets(new[] {Define.AssetPath});
            return asset;
        }
    }
}
