/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace CookApps.PackageTemplate.Editor
{
    internal static class Define
    {
        public const string AssetPath = "Assets/CookApps/Editor/TechPackageAsset.asset";
        public const string PackageJsonPath = "Assets/Package/package.json";
        public const string MinimumSupportUnityVersion = "2021.3";
        public const string ChangeLogConfigYamlPath = ".chglog/config.yml";
        public static readonly string[] ReadmePaths = {
            "Assets/Package/README.md",
            "README.md",
        };

        public const string GithubWorkflowsTestYamlPath = "./.github/workflows/test.yml";
        public const string UnityVersionYaml = "unityVersion:";
        public const string StrDeprecated = "(deprecated)";
        public const string TopicCookappsUnityPackage = "cookapps-unity-package";
    }
}
