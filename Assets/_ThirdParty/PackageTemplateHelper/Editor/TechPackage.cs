/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace CookApps.PackageTemplate.Editor
{
    internal static class TechPackage
    {
        // menu item : CookApps/Tech Package
        [MenuItem("CookApps/Tech Package", priority = -1)]
        public static void OpenTechPackageWindow()
        {
            TechPackageAsset asset = TechPackageAsset.GetAssets();
            Selection.activeObject = asset;
        }

        public static void LoadAsset([NotNull] TechPackageAsset asset)
        {
            JObject json = JObject.Parse(File.ReadAllText(Define.PackageJsonPath));
            asset.Name = json["name"]!.Value<string>();
            asset.DisplayName = json["displayName"]!.Value<string>();
            asset.Description = json["description"]!.Value<string>();
            asset.Keywords = json["keywords"]!.ToObject<string[]>();
            asset.SupportUnityVersion = json["unity"]?.Value<string>() ?? Define.MinimumSupportUnityVersion;
            asset.UnityTestRunner = YamlUtil.GetChildrenItems(Define.GithubWorkflowsTestYamlPath, Define.UnityVersionYaml);
            // deprecated 제거
            asset.Description = asset.Description.Replace(Define.StrDeprecated, "").Trim();

            Debug.Log("package.json 로딩 완료");
        }

        public static void SaveAsset([NotNull] TechPackageAsset asset)
        {
            ValidateAsset(asset);
            JObject json = JObject.Parse(File.ReadAllText(Define.PackageJsonPath));
            json["name"] = asset.Name;
            json["displayName"] = asset.GetDisplayName();
            json["description"] = asset.GetDescription();
            json["keywords"] = JArray.FromObject(asset.Keywords);
            json["unity"] = asset.SupportUnityVersion;
            File.WriteAllText(Define.PackageJsonPath, json.ToString(Formatting.Indented));
            SaveReadMe(asset);
            SaveUnityTestRunner(asset);
            Debug.Log("package.json 저장 완료");
        }

        private static void ValidateAsset([NotNull] TechPackageAsset asset)
        {
            if (string.IsNullOrEmpty(asset.Name))
            {
                throw new System.Exception("Name이 비어있습니다.");
            }
            if (asset.Name == "com.cookapps.package.template")
            {
                throw new System.Exception("Name이 com.cookapps.package.template입니다. 변경해주세요.");
            }
            if (string.IsNullOrEmpty(asset.DisplayName))
            {
                throw new System.Exception("DisplayName이 비어있습니다.");
            }
            if (asset.DisplayName == "CookApps Package Template")
            {
                throw new System.Exception("DisplayName이 CookApps Package Template입니다. 변경해주세요.");
            }
            if (string.IsNullOrEmpty(asset.SupportUnityVersion))
            {
                throw new System.Exception("SupportUnityVersion이 비어있습니다.");
            }
            if(asset.UnityTestRunner.Length == 0)
            {
                throw new System.Exception("UnityTestRunner가 비어있습니다.");
            }
        }

        private static void SaveUnityTestRunner([NotNull] TechPackageAsset asset)
        {
            YamlUtil.SetChildrenItems(Define.GithubWorkflowsTestYamlPath, Define.UnityVersionYaml, asset.UnityTestRunner);
        }

        // .chglog/config.yml 파일에 repository_url을 저장
        private static bool SaveChangeLogConfig()
        {
            if (!File.Exists(Define.ChangeLogConfigYamlPath))
            {
                return false;
            }

            // GitHub CLI로 repository_url 가져오기
            (bool success, string output) = ExecuteCommand.RunGitHubCli($"gh repo view --json url");
            if (!success)
            {
                Debug.LogError("GitHub CLI 실행 실패");
                return false;
            }

            JObject jsonUrl = JObject.Parse(output);

            string[] allLines = File.ReadAllLines(Define.ChangeLogConfigYamlPath);
            for (var i = 0; i < allLines.Length; i++)
            {
                if (!allLines[i].Contains("repository_url:"))
                {
                    continue;
                }

                int spaceCount = YamlUtil.GetSpaceCount(allLines[i]);
                // repository_url이 이미 존재하면 수정
                allLines[i] = YamlUtil.CreateStringWithSpace($"repository_url: {jsonUrl["url"]}", spaceCount);
                break;
            }

            File.WriteAllLines(Define.ChangeLogConfigYamlPath, allLines);
            return true;
        }

        // README.md 파일 저장
        private static void SaveReadMe([NotNull] TechPackageAsset asset)
        {
            string content = asset.GetReadmeContent();
            foreach (string path in Define.ReadmePaths)
            {
                File.WriteAllText(path, content);
            }

            Debug.Log("README.md 저장 완료");
        }

        // GitHub 동기화
        public static void GitHubSync([NotNull] TechPackageAsset asset)
        {
            // GitHub description 동기화
            if (!ExecuteCommand.RunGitHubCli($"gh repo edit --description \"{asset.GetDescription()}\"").success)
            {
                return;
            }

            // .chglog/config.yml 파일에 repository_url을 저장
            if (!SaveChangeLogConfig())
            {
                return;
            }

            Debug.Log("GitHub 동기화 완료");
        }

        // GitHub CLI 초기화
        public static void GitHubInitialize()
        {
            string envPath = EditorUtility.OpenFilePanel("GitHub secret env 파일을 선택해주세요", "", "env");
            if(string.IsNullOrEmpty(envPath))
            {
                Debug.Log("secret 파일이 선택되지 않았습니다.");
                return;
            }

            // secret env 설정
            if (!ExecuteCommand.RunGitHubCli($"gh secret set -f \"{envPath}\"").success)
            {
                return;
            }


            // cookapps-unity-package topic 추가
            if (!ExecuteCommand.RunGitHubCli($"gh repo edit --add-topic \"{Define.TopicCookappsUnityPackage}\"").success)
            {
                return;
            }

            // team permission 설정
            if (!SetTeamPermission())
            {
                return;
            }

            Debug.Log("GitHub CLI 초기화 완료");
        }

        // team permission 설정
        private static bool SetTeamPermission()
        {
            (bool success, string output) = ExecuteCommand.RunGitHubCli($"gh repo view --json name");
            if (!success)
            {
                Debug.LogError("GitHub CLI 실행 실패");
                return false;
            }

            JObject json = JObject.Parse(output);
            var repo = json["name"]!.Value<string>();
            string org = "cookapps-devops";
            string team_id = "tech-support";

            // https://docs.github.com/en/rest/teams/teams?apiVersion=2022-11-28#add-or-update-team-repository-permissions
            var cmd = $"gh api --method PUT -H \"Accept: application/vnd.github+json\" -H \"X-GitHub-Api-Version: 2022-11-28\" /orgs/{org}/teams/{team_id}/repos/{org}/{repo} -f permission=\"admin\"";

            if (!ExecuteCommand.RunGitHubCli(cmd).success)
            {
                Debug.LogError("GitHub CLI 실행 실패");
                return false;
            }

            return true;
        }
    }
}
