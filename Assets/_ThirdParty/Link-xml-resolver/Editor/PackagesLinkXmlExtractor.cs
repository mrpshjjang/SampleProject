/*
* Copyright (c) Sample.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEngine;

namespace Sample.LinkXmlResolver.Editor
{
    /// <summary>
    /// Package내부의 link.xml 빌드시 자동 생성 & 머지
    /// 빌드 이후 자동 삭제
    /// </summary>
    internal class PackagesLinkXmlExtractor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private class AsmdefInfo
        {
            public string name;
        }

        private static readonly string SamplePackage = "com.sample";
        private static string TemporaryFolder => $"{Application.dataPath}/Sample/";
        private static string LinkFilePath => $"{TemporaryFolder}link.xml";

        private static readonly string RelativeLinkFilePath = "Assets/Sample/link.xml";

        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            CreateMergedLinkFromPackages();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            // empty
        }

#if UNITY_2021_1_OR_NEWER
        private static void CreateMergedLinkFromPackages()
        {
            var packageInfos = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            ProcessPackages(packageInfos);
        }
#else
        private static void CreateMergedLinkFromPackages()
        {
            UnityEditor.PackageManager.Requests.ListRequest request = UnityEditor.PackageManager.Client.List();
            do
            {
                // 완료시까지 대기
            } while (!request.IsCompleted);

            if (request.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                UnityEditor.PackageManager.PackageInfo[] packageInfos = request.Result.ToArray();
                ProcessPackages(packageInfos);
            }
            else if (request.Status >= UnityEditor.PackageManager.StatusCode.Failure)
            {
                Debug.LogError(request.Error.message);
            }
        }
#endif

        private static void ProcessPackages(UnityEditor.PackageManager.PackageInfo[] allPackageInfos)
        {
            if (!allPackageInfos.Any())
            {
                return;
            }

            // Play때만 사용하는 Assemblies 모두 얻기
            List<string> playerAssemblies = CompilationPipeline
                .GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
                .Select(x => x.name).ToList();

            IEnumerable<UnityEditor.PackageManager.PackageInfo> samplePackages =
                allPackageInfos.Where(x => x.name.StartsWith(SamplePackage));
            var xmlPathList = new List<string>();
            var asmdefList = new List<string>();
            foreach (UnityEditor.PackageManager.PackageInfo package in samplePackages)
            {
                string path = package.resolvedPath;
                xmlPathList.AddRange(Directory.EnumerateFiles(path, "link.xml", SearchOption.AllDirectories).ToList());

                List<string> asmdef = Directory.EnumerateFiles(path, "*.asmdef", SearchOption.AllDirectories).ToList();
                foreach (string def in asmdef)
                {
                    string json = File.ReadAllText(def);
                    var asmdefInfo = JsonUtility.FromJson<AsmdefInfo>(json);
                    // 해당 Assemblies이름이 Play때 사용하는가?
                    if (playerAssemblies.Contains(asmdefInfo.name))
                    {
                        asmdefList.Add(asmdefInfo.name);
                    }
                }
            }

            CreateMergedLinkFromPackages(xmlPathList, asmdefList);
        }

        private static void CreateMergedLinkFromPackages(IList<string> linkPathList, IList<string> asmdefNameList)
        {
            var doc = new XDocument();
            var root = new XElement("linker");
            doc.Add(root);
            doc.AddFirst(new XComment("Sample 패키지 자동 생성 (수정하지 마세요!!)"));

            if (asmdefNameList.Any())
            {
                root.Add(new XComment("Sample 내부 패키지"));
                foreach (string asmdef in asmdefNameList)
                {
                    var element = new XElement("assembly",
                        new XAttribute("fullname", asmdef),
                        new XAttribute("preserve", "all")
                    );
                    root.Add(element);
                }
            }

            if (linkPathList.Any())
            {
                root.Add(new XComment("Sample 외부 패키지"));
                XDocument[] xmlList = linkPathList.Select(XDocument.Load).ToArray();
                foreach (XDocument xDocument in xmlList)
                {
                    if (xDocument.Root != null)
                    {
                        root.Add(xDocument.Root.Elements());
                    }
                }
            }

            if (!Directory.Exists(TemporaryFolder))
            {
                Directory.CreateDirectory(TemporaryFolder);
            }

            doc.Save(LinkFilePath, SaveOptions.None);
            AssetDatabase.ImportAsset(RelativeLinkFilePath);
        }
    }
}
