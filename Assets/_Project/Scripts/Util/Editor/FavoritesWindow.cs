using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FavoritesWindow : EditorWindow
{
    #region public

    [MenuItem("PxP/Favorites %#&f")]
    public static void ShowWindow()
    {
        GetWindow<FavoritesWindow>("즐겨찾기");
    }

    #endregion


    #region private

    //const
    private const double DOUBLE_CLICK_DELAY = 0.3;      // 더블클릭으로 에셋을 오픈할 때, 더블 클릭 딜레이
    private const int FILE_ICON_SIZE = 32;              // 에셋 아이콘의 크기

    //state
    private Vector2 _scrollPosition;
    private double _lastClickTime;

    //data
    private List<Object> _listFavorite = new();
    private Dictionary<string, bool> _folderToggleStates = new();

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        int removeIndex = -1;
        for (var i = 0; i < _listFavorite.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            //필드 생성
            _listFavorite[i] = EditorGUILayout.ObjectField(_listFavorite[i], typeof(Object), false);

            //즐겨찾기 삭제 버튼
            if (GUILayout.Button("삭제"))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();

            //디렉토리일 경우 하위 디스플레이
            if (_listFavorite[i] != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(_listFavorite[i]);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    EditorGUI.indentLevel++;
                    DisplayFolderContents(assetPath);
                    EditorGUI.indentLevel--;
                }
            }
        }

        //삭제할 필드가 있다면 삭제
        if (removeIndex >= 0)
        {
            _listFavorite.RemoveAt(removeIndex);
        }

        //즐겨찾기 추가 버튼
        if (GUILayout.Button("즐겨찾기 추가"))
        {
            _listFavorite.Add(null);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 디렉토리의 하위 에셋 출력
    /// </summary>
    /// <param name="folderPath"></param>
    private void DisplayFolderContents(string folderPath)
    {
        string[] subFolders = AssetDatabase.GetSubFolders(folderPath);
        string[] files = Directory.GetFiles(folderPath);

        //하위 폴더에 대해 해당 함수 재귀
        foreach (string subFolder in subFolders)
        {
            string folderName = Path.GetFileName(subFolder);
            if (!_folderToggleStates.ContainsKey(subFolder))
                _folderToggleStates[subFolder] = false;

            //접기 & 펴기 추가
            _folderToggleStates[subFolder] = EditorGUILayout.Foldout(_folderToggleStates[subFolder], new GUIContent(folderName, EditorGUIUtility.FindTexture("Folder Icon")), true);

            if (_folderToggleStates[subFolder])
            {
                //한 칸 들여쓰기하여 재귀
                EditorGUI.indentLevel++;
                DisplayFolderContents(subFolder);
                EditorGUI.indentLevel--;
            }
        }

        //파일 디스플레이
        foreach (string file in files)
        {
            if (Path.GetExtension(file) != ".meta")
            {
                string fileName = Path.GetFileName(file);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(16 * EditorGUI.indentLevel);

                var iconTexture = AssetDatabase.GetCachedIcon(file) as Texture2D;
                if (iconTexture != null)
                {
                    // 아이콘 크기 조절
                    iconTexture = ResizeTexture(iconTexture, FILE_ICON_SIZE, FILE_ICON_SIZE);
                }

                //파일 더블클릭 시 해당 에셋 오픈 버튼
                if (GUILayout.Button(new GUIContent(fileName, iconTexture), EditorStyles.label, GUILayout.ExpandWidth(true)))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(file);
                    if (obj != null)
                    {
                        EditorGUIUtility.PingObject(obj);
                        Selection.activeObject = obj;
                    }

                    double timeSinceLastClick = EditorApplication.timeSinceStartup - _lastClickTime;
                    if (timeSinceLastClick < DOUBLE_CLICK_DELAY)
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(file));
                    }

                    _lastClickTime = EditorApplication.timeSinceStartup;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 아이콘 크기 조절
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newWidth"></param>
    /// <param name="newHeight"></param>
    /// <returns></returns>
    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        FilterMode originalFilterMode = source.filterMode;
        source.filterMode = FilterMode.Point;

        RenderTexture rt = new(newWidth, newHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);

        Texture2D resizedTexture = new(newWidth, newHeight);
        resizedTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = null;
        rt.Release();

        source.filterMode = originalFilterMode;

        return resizedTexture;
    }

    #endregion
}
