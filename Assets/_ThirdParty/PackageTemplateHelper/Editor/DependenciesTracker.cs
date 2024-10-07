using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CookApps.PackageTemplate.Editor
{
    /// <summary>
    /// PackageManager에서 패키지를 추가/제거/업데이트를 할 경우 Assets/Package/package.json의 dependencies에 값을 변경해줍니다.
    /// </summary>
    internal class DependenciesTracker : IPackageManagerExtension
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        //------------------- Inspector ------------------//

        //------------------- public ------------------//

        //------------------- protected ------------------//

        //------------------- private ------------------//
        private static readonly string DEPENDENCIES = "dependencies";
        private static readonly string PathPackageJson = "Assets/Package/package.json";

        //--------------------------------------------------------------------------------//
        //------------------------------------PROPERTY------------------------------------//
        //--------------------------------------------------------------------------------//

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        //───────────────────────────────────────────────────────────────────────────────────────
        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            IPackageManagerExtension dt = new DependenciesTracker();
            PackageManagerExtensions.RegisterExtension(dt);
        }

        public VisualElement CreateExtensionUI()
        {
            return null;
        }

        public void OnPackageSelectionChange(PackageInfo packageInfo)
        {
            // empty
        }

        public void OnPackageAddedOrUpdated(PackageInfo packageInfo)
        {
            ConformAddOrUpdated(packageInfo.name, packageInfo.version);
        }

        public void OnPackageRemoved(PackageInfo packageInfo)
        {
            ConformRemovePackage(packageInfo.name);
        }

        private static void ConformAddOrUpdated(string packageName, string version)
        {
            if (EditorUtility.DisplayDialog("알림",
                    $"package.json의 dependencies에 {packageName}의 version : {version}을 갱신하시겠습니까?", "확인", "취소") ==
                false)
            {
                return;
            }

            AddOrUpdated(packageName, version);
        }

        private static void ConformRemovePackage(string packageName)
        {
            string version = GetPackageVersion(packageName);
            if (string.IsNullOrEmpty(version))
            {
                return;
            }

            if (EditorUtility.DisplayDialog("알림",
                    $"package.json의 dependencies에 {packageName}을 제거하시겠습니까?", "확인", "취소") ==
                false)
            {
                return;
            }
            RemovePackage(packageName);
        }

        private static void RemovePackage(string packageName)
        {
            JObject jObject = LoadPackage();
            var dependencies = jObject[DEPENDENCIES] as JObject;
            dependencies?.Remove(packageName);
            SavePackage(jObject);
        }

        private static void AddOrUpdated(string packageName, string version)
        {
            JObject jObject = LoadPackage();
            var dependencies = jObject[DEPENDENCIES] as JObject;
            if (dependencies == null)
            {
                dependencies = new JObject();
                jObject[DEPENDENCIES] = dependencies;
            }
            dependencies[packageName] = version;
            SavePackage(jObject);
        }

        private static string GetPackageVersion(string packageName)
        {
            JObject jObject = LoadPackage();
            JToken dependencies = jObject[DEPENDENCIES];
            JToken package = dependencies?[packageName];
            return package?.Value<string>();
        }

        private static JObject LoadPackage()
        {
            Assert.IsTrue(File.Exists(PathPackageJson));
            string json = File.ReadAllText(PathPackageJson);
            JObject jObject = JObject.Parse(json);
            Assert.IsNotNull(jObject);
            return jObject;
        }

        private static void SavePackage(JObject jObject)
        {
            var json = jObject.ToString(Formatting.Indented);
            File.WriteAllText(PathPackageJson, json);
        }
    }
}
