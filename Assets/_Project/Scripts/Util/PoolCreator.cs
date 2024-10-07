#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sample.Inspector;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PoolCreator))]
public class PoolCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (PoolCreator)target;

        if (GUILayout.Button("풀 자동 생성", GUILayout.Height(40)))
        {
            script.CreatePool();
        }
    }
}


public class PoolCreator : ScriptableObject
{
    private static PoolCreator instance = null;

    [Folder]
    public string PoolFolder;

    public static PoolCreator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("PoolCreator") as PoolCreator;
            }

            return instance;
        }
    }

    #region public

    public void CreatePool()
    {
        var folders = AssetDatabase.GetSubFolders(PoolFolder);

        foreach (string folder in folders)
        {
            if (!poolContainerList.Any(e => e.poolName.Equals(folder)))
            {
                var contain = folder.Split('/').Last();
                PoolContainer container = new PoolContainer()
                {
                    poolName = contain,
                };

                var goFolder = AssetDatabase.LoadAssetAtPath(folder, typeof(DefaultAsset) );
                container.directoryList.Add(goFolder);

                poolContainerList.Add(container);
            }
        }

        foreach (var container in this.poolContainerList)
        {
            if (container == null)
                continue;

            List<string> listCreatedName = new List<string>();
            foreach (var directory in container.directoryList)
            {
                string assetPath = AssetDatabase.GetAssetPath(directory);

                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    if (listCreatedName.Contains(container.poolName))
                    {
                        TryCreatePool(container.poolName, assetPath, false);
                    }
                    else
                    {
                        TryCreatePool(container.poolName, assetPath, true);
                        listCreatedName.AddUnique(container.poolName);
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
    }

    #endregion


    #region private

    /// <summary>
    /// 풀 자동 생성
    /// </summary>
    /// <param name="name"></param>
    /// <param name="folderPath"></param>
    private void TryCreatePool(string name, string folderPath, bool isFirst)
    {
        string[] subFolders = AssetDatabase.GetSubFolders(folderPath);
        foreach (var folder in subFolders)
        {
            TryCreatePool(name, folder, false);
        }

        string[] files = Directory.GetFiles(folderPath);

        foreach (string file in files)
        {
            if (Path.GetExtension(file) != ".meta")
            {
                Transform obj = AssetDatabase.LoadAssetAtPath<Transform>(file);
                if (obj != null)
                {
                    obj.SetAddressableGroup(name);
                }
            }
        }
    }

    [Header("오브젝트 풀링")]
    [SerializeField] private List<PoolContainer> poolContainerList;

    #endregion


    #region lifecycle

    #endregion

    [Serializable]
    public class PoolContainer
    {
        public string poolName;
        public List<UnityEngine.Object> directoryList = new();
    }
}
#endif
