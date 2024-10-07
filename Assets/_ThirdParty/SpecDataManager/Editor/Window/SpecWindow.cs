/*
 * Copyright (c) Sample.
 */

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Sample.GoogleApis.Editor;
using Sample.SpecData.Editor.Asset;
using Sample.SpecData.Editor.Generator;
using Sample.SpecData.Editor.Util;
using ExcelDataReader;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sample.SpecData.Editor.Window
{
    [InitializeOnLoad]
    internal sealed class SpecWindow : UnityEditor.Editor
    {
        private static SpecWindow instance;

        static SpecWindow()
        {
            Package.Window.Editor.SamplePackageWindow.Add("SpecData",
                () => EditorCoroutineUtility.StartCoroutineOwnerless(InitCoroutine()));
        }

        private static IEnumerator InitCoroutine()
        {
            SpecWindow[] windows = Resources.FindObjectsOfTypeAll<SpecWindow>();
            foreach (SpecWindow buildWindow in windows)
            {
                DestroyImmediate(buildWindow);
            }

            if (File.Exists(Define.AssetPath))
            {
                ScriptableObject asset;
                do
                {
                    asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(Define.AssetPath);
                    if (asset == null)
                    {
                        yield return null;
                    }
                } while (asset == null);
            }

            UnityEditor.Editor window = CreateEditor(SpecDataAsset.GetAssets(), typeof(SpecWindow));
            Package.Window.Editor.SamplePackageWindow.SetContentVisualElement(window.CreateInspectorGUI());
        }


        private static string GetUIBuilderPath()
        {
            string packagePath = Path.GetFullPath("Packages/com.sample.specdatamanager");
            // 패키지로 배포?
            if (Directory.Exists(packagePath))
            {
                return "Packages/com.sample.specdatamanager/Editor/window.uxml";
            }

            return "Assets/_ThirdParty/SpecDataManager/Editor/window.uxml";
        }

        private IReadOnlyList<GoogleDriveFileInfo> _driveFileList;
        private DropdownField _dropdownField;
        private SpecDataAsset SpecDataAsset => target as SpecDataAsset;
        private VisualElement _rootVisualElement;
        private VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = _rootVisualElement;
            _visualTreeAsset.CloneTree(root);
            root.style.flexGrow = 2;

            _dropdownField = root.Q<DropdownField>("DropDownFileList");
            root.Q<Button>("BtnGetFileList").clicked += AsyncRefreshDriveFileList;
            root.Q<Button>("BtnApplySheetGeneratorCS").clicked += OnBtnApplySheetGeneratorCS;
            root.Q<Button>("BtnGoogleLogout").clicked += OnBtnGoogleLogout;
            root.Q<Button>("BtnSample").clicked += OnBtnSample;
            root.Q<Button>("BtnOpenSelectSheet").clicked += OnBtnOpenSelectSheet;
            root.Q<Button>("BtnValid").clicked += OnBtnValid;
            VisualElement asset = root.Q("Asset");
            var inspectorElement = new InspectorElement(serializedObject);
            inspectorElement.style.flexGrow = 2;
            asset.Add(inspectorElement);

            // 기존의 선택 시트가 있으면
            SpecDataAsset specDataAsset = SpecDataAsset;
            if (specDataAsset != null && !string.IsNullOrEmpty(specDataAsset.SheetName) &&
                !string.IsNullOrEmpty(specDataAsset.SheetId))
            {
                SetDriveFileList(new List<GoogleDriveFileInfo>()
                {
                    new()
                    {
                        Id = specDataAsset.SheetId,
                        Name = specDataAsset.SheetName,
                    },
                });
            }

            return root;
        }

        private static VisualTreeAsset LoadVisualTreeAsset()
        {
            string path = GetUIBuilderPath();
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            return tree;
        }

        private void OnEnable()
        {
            _rootVisualElement ??= new VisualElement();
            _visualTreeAsset ??= LoadVisualTreeAsset();
        }

        private void CheckRefresh()
        {
            if (!GoogleDriveHelper.IsGoogleLogin() || Application.isPlaying)
            {
                return;
            }

            SpecDataAsset specDataAsset = SpecDataAsset;
            if (!string.IsNullOrEmpty(specDataAsset.SheetName) && !string.IsNullOrEmpty(specDataAsset.SheetId))
            {
                DriveFileListFromAsset();
            }
            else
            {
                AsyncRefreshDriveFileList();
            }
        }

        private void OnBtnValid()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            SpecDataValidator.Valid();
        }

        private void OnBtnSample()
        {
            var url = "https://docs.google.com/spreadsheets/d/1uUoUwyM9dfpeiVQAlURo1lnv1n3Ze7pnMNvnoJ1kEos";
            Application.OpenURL(url);
        }

        private void OnBtnOpenSelectSheet()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            int index = _dropdownField.index;
            if (index == -1)
            {
                EditorUtility.DisplayDialog("Error", "Sheet를 선택해 주세요", "확인");
                return;
            }

            GoogleDriveFileInfo select = _driveFileList[index];
            var url = $"https://docs.google.com/spreadsheets/d/{select.Id}";
            Application.OpenURL(url);
        }

        private void OnBtnGoogleLogout()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            GoogleDriveHelper.GoogleLogout();
            _dropdownField.choices = null;
            _dropdownField.index = -1;
        }

        private void OnBtnApplySheetGeneratorCS()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            int index = _dropdownField.index;
            if (index == -1)
            {
                EditorUtility.DisplayDialog("Error", "Sheet를 선택해 주세요", "확인");
                return;
            }

            GoogleDriveFileInfo select = _driveFileList[index];
            ApplySheetGeneratorCS(select);
        }

        private async void AsyncRefreshDriveFileList()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            IEnumerable<GoogleDriveFileInfo> driveFileList =
                await GoogleDriveHelper.AsyncGetDriveSheetFileList(SpecDataAsset.FilterTitle);
            SetDriveFileList(driveFileList);
        }

        private void DriveFileListFromAsset()
        {
            if (!IsEditMode() || !IsCompiling())
            {
                return;
            }

            SpecDataAsset asset = SpecDataAsset;
            List<GoogleDriveFileInfo> listSheetInfo = new()
            {
                new GoogleDriveFileInfo
                {
                    Id = asset.SheetId,
                    Name = asset.SheetName,
                },
            };

            SetDriveFileList(listSheetInfo);
        }

        private void SetDriveFileList(IEnumerable<GoogleDriveFileInfo> list)
        {
            List<GoogleDriveFileInfo> googleDriveFileInfos = list.ToList();
            _driveFileList = googleDriveFileInfos;
            _dropdownField.choices = _driveFileList.Select(x => x.Name).ToList();
            _dropdownField.index = googleDriveFileInfos.Count > 0 ? 0 : -1;
            string selectId = SpecDataAsset.SheetId;
            for (var i = 0; i < _driveFileList.Count; i++)
            {
                if (!_driveFileList[i].Id.Equals(selectId))
                {
                    continue;
                }

                _dropdownField.index = i;
                break;
            }
        }

        private void ApplySheetGeneratorCS(GoogleDriveFileInfo googleDriveFileId)
        {
            if (!HasSourceFolder())
            {
                return;
            }

            IExcelDataReader reader = GoogleDriveHelper.DownloadSheet(googleDriveFileId);
            if (reader == default)
            {
                EditorUtility.DisplayDialog("Error", "Sheet 생성 실패 (excel 다운로드 실패)", "확인");
                return;
            }

            SpecDataAsset asset = SpecDataAsset;
            asset.SheetId = googleDriveFileId.Id;
            asset.SheetName = googleDriveFileId.Name;
            ConvertSheetInfo sheetInfo = Generator.Generator.ConvertDataSet(reader);
            if (sheetInfo == default)
            {
                Debug.unityLogger.LogError(Define.Tag, "<color=green>SpecDataResource 생성 실패</color>");
                return;
            }

            DataSet dataSet = sheetInfo.DataSet;
            string json = Generator.Generator.ConvertJson(sheetInfo);

            if (!string.IsNullOrEmpty(asset.OriginalJsonFolder))
            {
                if (!Directory.Exists(asset.OriginalJsonFolder))
                {
                    Directory.CreateDirectory(asset.OriginalJsonFolder);
                }

                File.WriteAllText(Path.Combine(asset.OriginalJsonFolder, "OriginalSpecData.json"), json);
                Debug.unityLogger.Log(Define.Tag,
                    $"<color=green>원본 Json {asset.OriginalJsonFolder}/OriginalSpecData.json 저장</color>");
            }

            byte[] key = Encoding.UTF8.GetBytes(asset.EncryptKey);
            byte[] bytes = CryptoUtil.EncryptAes128(json, key);
            if (!Directory.Exists(Define.SpecDataResourceDir))
            {
                Directory.CreateDirectory(Define.SpecDataResourceDir);
            }

            File.WriteAllBytes(Define.SpecDataResourcePath, bytes);
            Debug.unityLogger.Log(Define.Tag, "<color=green>SpecDataResource 생성 성공</color>");

            //JsonClassGenerator.GenerateClasses(json, "", asset.SourceFolder);

            // 코드 생성
            GeneratorCS_SpecDataResourceLoader.CreateCS(asset.SourceFolder, asset.SourceNamespace, asset.EncryptKey);

            GeneratorCS_SpecData.CreateCS(asset.SourceFolder, asset.SourceNamespace, asset.SourceEnumNamespace,
                dataSet.Tables.OfType<SchemaDataTable>());
            GeneratorCS_SpecDataManager.CreateCS(asset.SourceFolder, asset.SourceNamespace);
            // 검증 파일 생성
            if (asset.CreateValidFile)
            {
                GeneratorCS_SpecDataValid.CreateCS(asset.SourceFolder, asset.SourceNamespace, asset.SourceEnumNamespace,
                    dataSet.Tables.OfType<SchemaDataTable>());
            }

            if (sheetInfo.HasEnum)
            {
                GeneratorCS_SpecDataEnum.CreateCS(asset.SourceFolder, asset.SourceEnumNamespace,
                    sheetInfo.SchemaEnumValues);
            }

            Debug.unityLogger.Log(Define.Tag, "<color=green>CS 코드 생성 성공</color>");
            AssetDatabase.Refresh();

            // 자동 검증
            SpecDataValidator.AutoValid();
        }

        private bool HasSourceFolder()
        {
            SpecDataAsset asset = SpecDataAsset;
            if (!string.IsNullOrEmpty(asset.SourceFolder))
            {
                return true;
            }

            EditorUtility.DisplayDialog("Error", "SourceFolder 먼저 설정해 주세요", "확인");
            return false;
        }

        private bool IsEditMode()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Play 상태에서 사용 불가능합니다.", "확인");
                return false;
            }

            return true;
        }

        private static bool IsCompiling()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Error", "Compiling 상태에서 사용 불가능합니다.", "확인");
                return false;
            }

            return true;
        }
    }
}
