using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CookApps.PackageTemplate.Editor
{
    public class TechPackageAsset : ScriptableObject
    {
        [Header("패키지 정보 (package.json)")]
        [Tooltip("패키지 이름")]
        public string Name;
        [Tooltip("패키지 표시 이름")]
        public string DisplayName;
        [Tooltip("패키지 & GitHub Description")]
        public string Description;
        [Tooltip("유니티 최소 지원 버전")]
        public string SupportUnityVersion;
        [Tooltip("키워드")]
        public string[] Keywords;

        [Header("README.md")]
        [Multiline(10)]
        public string Content =
            "- **[사용 방법 및 API](https://cookapps.atlassian.net/wiki/spaces/TST/pages/25344114772)**\n\n문의 사항 (<tech@cookapps.com>)";

        [Header("Unity")]
        [Tooltip("Unity Test Runner 버전 1개 이상 필수")]
        public string[] UnityTestRunner;

        [Header("기타")]
        public bool Deprecated = false;

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public string GetDescription()
        {
            string result = Description.Replace(Define.StrDeprecated, "").Trim();
            return Deprecated ? $"{result} {Define.StrDeprecated}" : result;
        }

        // README.md 파일 내용 생성
        public string GetReadmeContent()
        {
            var stringBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(DisplayName))
            {
                throw new Exception("DisplayName이 비어있습니다.");
            }

            stringBuilder.AppendLine($"# {DisplayName}");
            stringBuilder.AppendLine();

            if (Deprecated)
            {
                stringBuilder.AppendLine("> [!IMPORTANT]");
                stringBuilder.AppendLine("> 이 저장소는 더 이상 사용하지 않습니다.");
                stringBuilder.AppendLine();
            }

            if (!string.IsNullOrEmpty(Description))
            {

                stringBuilder.AppendLine($"{Description}");
                stringBuilder.AppendLine();
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine("---");
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(Content);
            string content = stringBuilder.ToString().Trim();
            content += Environment.NewLine;
            return content;
        }

        public static TechPackageAsset GetAssets()
        {
            const string path = Define.AssetPath;
            var asset = AssetDatabase.LoadAssetAtPath<TechPackageAsset>(path);
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

            asset = CreateInstance<TechPackageAsset>();
            TechPackage.LoadAsset(asset);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}
