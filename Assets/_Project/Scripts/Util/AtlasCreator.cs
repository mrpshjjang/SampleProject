#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sample.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

[CustomEditor(typeof(AtlasCreator))]
public class AtlasCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (AtlasCreator)target;

        if (GUILayout.Button("아틀라스 자동 생성", GUILayout.Height(40)))
        {
            script.CreateAtlas();
        }
    }
}


public class AtlasCreator : ScriptableObject
{
    private static AtlasCreator instance = null;

    public static AtlasCreator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("AtlasCreator") as AtlasCreator;
            }

            return instance;
        }
    }

    #region public

    public void CreateAtlas()
    {
        var folders = AssetDatabase.GetSubFolders(OriginalTextureFolder);
        var atlasSaveFolder = AtlasSaveFolder.Equals("") ? "Asset/Resources" : AtlasSaveFolder;

        foreach (string folder in folders)
        {
            if (!atlasInfos.Any(e => e.pathToLoad.Equals(folder)))
            {
                var contain = folder.Split('/').Last();
                AtlasInfo info = new AtlasInfo
                {
                    pathToLoad = folder,
                    pathToSave = $"{atlasSaveFolder}",
                    prefix = $"{AtlasPrefix}_{contain}_",
                    enableRotation = DefEnableRotation,
                    enableTightPacking = DefEnableTightPacking,
                    padding = DefPadding,
                    filterMode = DefFilterMode,
                    sRGB = DefSRGB,
                    fileAddType = DefFileAddType
                };
                atlasInfos.Add(info);
            }
        }

        if (atlasInfos is {Count: > 0})
        {
            foreach (var info in atlasInfos)
            {
                CreateSpriteAtlases(info);
            }
        }
    }

    #endregion


    #region private

    private static void CreateSpriteAtlases(AtlasInfo info)
    {
        string[] subdirectories;

        if (info.isRecursive)
            subdirectories = Directory.GetDirectories(info.pathToLoad);
        else
            subdirectories = new[] {info.pathToLoad};

        CreateSpriteAtlasForDirectory(info, info.pathToLoad);
        foreach (string subdirectory in subdirectories)
        {
            CreateSpriteAtlasForDirectory(info, subdirectory);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateSpriteAtlasForDirectory(AtlasInfo info, string directory)
    {
        //아틀라스 생성
        string atlasName = info.prefix + Path.GetFileName(directory);
        string filePathToSave = $"{info.pathToSave}/{atlasName}.spriteatlas";

        //이미 있다면 재생성하지 않기
        if (AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePathToSave) != null)
            return;

        //아틀라스 설정 (옵션에 따라 조정 가능)
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        SpriteAtlasPackingSettings packSettings = new SpriteAtlasPackingSettings()
        {
            enableRotation = info.enableRotation,
            enableTightPacking = info.enableTightPacking,
            padding = info.padding,
        };
        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            filterMode = info.filterMode,
            sRGB = info.sRGB,
        };
        spriteAtlas.SetIncludeInBuild(true);
        spriteAtlas.SetPackingSettings(packSettings);
        spriteAtlas.SetTextureSettings(textureSettings);

        switch (info.fileAddType)
        {
            case eAtlasFileAddType.DIRECTORY:
                // 디렉토리만 Add하는 타입
                var asset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(directory);
                spriteAtlas.Add(new[] {asset});
                break;
            case eAtlasFileAddType.FILES:
                // 디렉토리 내의 모든 파일 찾기 타입
                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".aseprite"))
                    {
                        string assetPath = file.Replace("\\", "/");
                        var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                        if (obj != null)
                        {
                            // 스프라이트 아틀라스에 스프라이트 추가
                            spriteAtlas.Add(new[] {obj});
                        }
                    }
                }

                break;
        }

        // 아틀라스 저장
        AssetDatabase.CreateAsset(spriteAtlas, filePathToSave);
    }

    [Header("아틀라스 Load & Save")]
    [SerializeField] private List<AtlasInfo> atlasInfos;

    [Folder]
    public string OriginalTextureFolder;

    [Folder]
    public string AtlasSaveFolder;

    public string AtlasPrefix;

    [Header("Default Option")]
    public bool DefEnableRotation = true;
    public bool DefEnableTightPacking = true;
    public int DefPadding = 4;
    public FilterMode DefFilterMode = FilterMode.Point;
    public bool DefSRGB = true;
    public eAtlasFileAddType DefFileAddType = eAtlasFileAddType.DIRECTORY;

    #endregion


    #region lifecycle

    #endregion

    [Serializable]
    public class AtlasInfo
    {
        public string pathToSave;                                           //아틀라스를 저장할 경로
        public string pathToLoad;                                           //이미지를 로드할 경로
        public string prefix;                                               //아틀라스 네이밍
        public eAtlasFileAddType fileAddType = eAtlasFileAddType.DIRECTORY; //파일 포함 방식
        public bool isRecursive = true;                                     //하위폴더 전부 탐색인지, 루트폴더만 탐색인지

        [Header("Pack Settings")]
        public bool enableRotation = true;
        public bool enableTightPacking = true;
        public int padding = 4;

        [Header("Texture Settings")]
        public FilterMode filterMode;
        public bool sRGB = false;
    }

    public enum eAtlasFileAddType
    {
        DIRECTORY,  //디렉토리만 포함
        FILES,      //파일만 포함
    }
}
#endif
