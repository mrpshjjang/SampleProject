using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CookApps.Package.Report.Editor
{
    internal class GoogleSheetInternal
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        //------------------- Inspector ------------------//


        //------------------- public ------------------//


        //------------------- protected ------------------//


        //------------------- private ------------------//
        private static Dictionary<string, CellIndexData> _notes;
        private static ValueRange _cachedSheetData;
        private static string _projectName;
        private static int _projectRowIndex;

        //--------------------------------------------------------------------------------//
        //------------------------------------PROPERTY------------------------------------//
        //--------------------------------------------------------------------------------//
        internal static string ProjectName => _projectName;
        internal static ValueRange SheetDatas => _cachedSheetData;
        internal static Dictionary<string, CellIndexData> Notes => _notes;

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        //───────────────────────────────────────────────────────────────────────────────────────
        internal static async Task Initialize()
        {
            await GetAllCellValuesFromServer();
            SetProjectInfos();
        }

        internal static async Task CheckAllInstalledPackages()
        {
            ListRequest packageList = Client.List(false, true);
            while (packageList.IsCompleted == false)
            {
                await Task.Yield();
            }

            foreach (PackageInfo package in packageList.Result)
            {
                // if (package.name.Contains("com.cookapps"))
                // {
                //     cellValues.Add(package.displayName);
                // }

                if (package.name.Contains("com.cookapps"))
                {
                    string packageName = PackageEventListener.GetMinimalPackageName(package.name);
                    string version = GetCellData(packageName);
                    if(string.IsNullOrEmpty(version))
                    {
                        AddCellData(packageName, package.version);
                    }
                    //버전이 다르면 버전 업데이트라고 간주
                    else if (version.Equals(package.version) == false)
                    {
                        UpdateCellData(packageName, package.version);
                    }
                }
            }
        }

        private static async Task GetAllCellValuesFromServer()
        {
            _notes ??= new();

            if (_cachedSheetData == null)
            {
                //시트의 모든 cell value를 가져옴
                var response = await TechPackagesReporter.GetCellValue();
                try
                {
                    _cachedSheetData = JsonConvert.DeserializeObject<ValueRange>(response.response);
                }
                catch (JsonReaderException e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        private static string GetCellData(string packageName)
        {
            for (int i = 0; i < _cachedSheetData.Values[0].Count; i++)
            {
                if (_cachedSheetData.Values[0][i].ToString().Equals(packageName))
                {
                    return _cachedSheetData.Values[_projectRowIndex][i].ToString();
                }
            }

            return string.Empty;
        }


        internal static void AddCellData(string packageName, string version)
        {
            if (_cachedSheetData.Values[0].Contains(packageName) == false)
            {
                _cachedSheetData.Values[0].Add(packageName);
            }
            UpdateCellDataInternal(packageName, version);
            TryAddToNote(packageName, version, "Added", _projectRowIndex, GetColumnIndex(packageName));
        }

        internal static void UpdateCellData(string packageName, string version)
        {
            UpdateCellDataInternal(packageName, version);
            TryAddToNote(packageName, version, "Updated", _projectRowIndex, GetColumnIndex(packageName));
        }

        private static void UpdateCellDataInternal(string packageName, string version)
        {
            int columnIndex = GetColumnIndex(packageName);
            if (columnIndex >= 0)
            {
                CheckExistCell(columnIndex);
                _cachedSheetData.Values[_projectRowIndex][columnIndex] = version;
            }
        }

        internal static void RemoveCellData(string packageName, string version)
        {
            int columnIndex = GetColumnIndex(packageName);
            if (columnIndex >= 0)
            {
                CheckExistCell(columnIndex);
                _cachedSheetData.Values[_projectRowIndex][columnIndex] = "Removed";
                TryAddToNote(packageName, version, "Removed", _projectRowIndex, columnIndex);
            }
        }

        private static void TryAddToNote(string packageName, string version, string status, int row, int column)
        {
            var value = $"●{DateTime.Now.ToString("yy.MM.dd-HH:mm:ss")} {status} {version}";
            CellIndexData cellIndexData = new()
            {
                Row = row,
                Column = column,
                Note = value,
            };
            if (_notes.TryAdd(packageName, cellIndexData) == false)
            {
                _notes[packageName] = cellIndexData;
            }
        }

        private static void CheckExistCell(int columnIndex)
        {
            int count = _cachedSheetData.Values[_projectRowIndex].Count;
            if (columnIndex >= count)
            {
                for (int i = count; i < columnIndex + 1; i++)
                {
                    _cachedSheetData.Values[_projectRowIndex].Add("");
                }
            }
        }

        internal static int GetColumnIndex(string packageName)
        {
            for (int i = 0; i < _cachedSheetData.Values[0].Count; i++)
            {
                if (_cachedSheetData.Values[0][i].ToString().Equals(packageName))
                {
                    return i;
                }
            }

            return -1;
        }

        private static void SetProjectInfos()
        {
            _projectName = PlayerSettings.productName;

            //프로젝트가 존재하는지 확인하고, 있다면 row를 저장
            _projectRowIndex = -1;
            if (_cachedSheetData.Values != null)
            {
                for (var i = 0; i < _cachedSheetData.Values.Count; i++)
                {
                    if (_cachedSheetData.Values[i][0].ToString().Equals(_projectName))
                    {
                        _projectRowIndex = i;
                        break;
                    }
                }
            }

            //프로젝트가 없었다면 새로 추가
            if (_projectRowIndex == -1)
            {
                _cachedSheetData.Values.Add(new List<object>{_projectName});
                _projectRowIndex = _cachedSheetData.Values.Count - 1;
            }

            //패키지 갯수만큼 프로젝트의 row에 column을 추가
            int packageCount = _cachedSheetData.Values[0].Count;
            int projectCount = _cachedSheetData.Values.Count;
            for (int i = 0; i < projectCount; i++)
            {
                //1행의 패키지 갯수만큼 각 프로젝트의 열을 늘려준다.
                int diff = packageCount - _cachedSheetData.Values[i].Count;
                for (int j = 0; j < diff; j++)
                {
                    _cachedSheetData.Values[i].Add("");
                }
            }
        }
    }
}
