using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public enum eAddressabelLabel
{
    Atlas,
    FX,
    UI,
}

public class AddressableManager : GameObjectSingleton<AddressableManager>
{
    #region public

    public bool IsInitialized => _isInit;

    public async UniTask InitAddressable()
    {
        await Addressables.InitializeAsync();

        _isInit = false;
        _cachingObjects.Clear();
        _list.Clear();
        _list.Add(InitSpriteAtlas());
    }

    public async UniTask CheckLoadAddressable()
    {
        await UniTask.WhenAll(_list);
        _isInit = true;
    }

    public Sprite GetSprite(string spriteName)
    {
        if (_sprites.TryGetValue(spriteName, out var sprite))
            return sprite;

        foreach (var atlas in _atlases)
        {
            sprite = atlas.GetSprite(spriteName);
            if (sprite != null)
            {
                _sprites.TryAdd(spriteName, sprite);
                return sprite;
            }
        }

        return null;
    }

    public GameObject GetObject(eAddressabelLabel label, string key)
    {
        if (_cachingObjects.TryGetValue(ZString.Concat(label.ToString(), "/", key), out var value))
        {
            return value;
        }

        return default;
    }

    public T GetObject<T>(eAddressabelLabel label, string key)
    {
        if (_cachingObjects.TryGetValue(ZString.Concat(label.ToString(), "/", key), out var value))
        {
            return value.GetComponent<T>();
        }

        return default;
    }

    public List<T> GetObjects<T>(eAddressabelLabel label)
    {
        List<T> list = new();
        string key = ZString.Concat(label.ToString(), "/");
        foreach (var obj in _cachingObjects)
        {
            if (obj.Key.Contains(key))
            {
                if (obj.Value.TryGetComponent<T>(out var component))
                    list.Add(component);
            }
        }

        return list;
    }

    public async UniTask<T> LoadObject<T>(string path) where T : UnityEngine.Object
    {
        if (_objects.TryGetValue(path, out GameObject obj))
        {
            return obj.TryGetComponent<T>(out var value) ? value : default;
        }

        try
        {
            var loadAsset = await Addressables.LoadAssetAsync<GameObject>(path);
            if (loadAsset != null)
            {
                if (loadAsset.TryGetComponent<T>(out var value))
                {
                    _objects.Add(path, loadAsset);
                    return value;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path} 존재하지 않음");
            return default;
        }

        return default;
    }

    public void ReleaseObject(string path)
    {
        if (_objects.TryGetValue(path, out GameObject obj))
        {
            Addressables.Release(obj);
            _objects.Remove(path);
        }
    }

    /// <summary>
    /// 비동기 형태의 리소스 생성
    /// </summary>
    public async UniTask<T> LoadInstanceAsync<T>(string path, Transform parent = null)
    {
        try
        {
            var obj = await Addressables.InstantiateAsync(path, parent);
            if (obj != null)
            {
                obj.TryGetComponent<T>(out var value);
                return value;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path} 존재하지 않음");
            return default;
        }

        return default;
    }

    /// <summary>
    /// 동기 형태의 리소스 생성
    /// </summary>
    public T LoadInstance<T>(string path, Transform parent = null)
    {
        try
        {
            var obj = Addressables.InstantiateAsync(path, parent).WaitForCompletion();
            if (obj != null)
            {
                obj.TryGetComponent<T>(out var value);
                return value;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path} 존재하지 않음");
            return default;
        }

        return default;
    }

    public void ReleaseInstance(GameObject obj)
    {
        Addressables.ReleaseInstance(obj);
    }

    public async UniTask<bool> CheckAddressablePath(string path)
    {
        if (_pathDic.TryGetValue(path, out var value))
            return value;

        try
        {
            var obj = await Addressables.LoadAssetAsync<GameObject>(path);
            if (obj != null)
            {
                _pathDic.TryAdd(path, true);
                Addressables.Release(obj);
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{path} 로드 실패.");
            _pathDic.TryAdd(path, false);
            return false;
        }

        Debug.LogError($"{path} 로드 실패.");
        _pathDic.TryAdd(path, false);
        return false;
    }

    #endregion

    #region protected

    #endregion

    #region private

    private List<UniTask> _list = new();
    private List<SpriteAtlas> _atlases = new();
    private Dictionary<string, GameObject> _objects = new();
    private Dictionary<string, GameObject> _cachingObjects = new();
    private Dictionary<string, Sprite> _sprites = new();
    private Dictionary<string, bool> _pathDic = new();

    private bool _isInit = false;

    private async UniTask InitObject(eAddressabelLabel label)
    {
        IList<IResourceLocation> list = await Addressables.LoadResourceLocationsAsync(label.ToString());
        IList<GameObject> listObj = await Addressables.LoadAssetsAsync<GameObject>(list, null);
        foreach (var obj in listObj)
        {
            if (obj != null)
            {
                _cachingObjects.TryAdd(ZString.Concat(label.ToString(), "/", obj.name), obj);
            }
        }
    }

    private async UniTask InitSpriteAtlas()
    {
        _atlases.Clear();
        IList<IResourceLocation> list = await Addressables.LoadResourceLocationsAsync(eAddressabelLabel.Atlas.ToString());

        foreach (var location in list)
        {
            var atlas = await Addressables.LoadAssetAsync<SpriteAtlas>(location);
            if (atlas != null)
                _atlases.Add(atlas);
        }

        //Addressables.Release(list);
    }


    #endregion
}
#if UNITY_EDITOR

public static class AddressableExtensions
{
    public static void ClearAddressableGroup(string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var find = settings.groups.Find(e => e.name == groupName);
            settings.groups.Remove(find);
        }
    }

    public static void SetAddressableGroup(this Object obj, string groupName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            var assetpath = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(assetpath);

            var label = settings.GetLabels().Find(e => e.Equals(groupName));
            if(label == null)
                settings.AddLabel(groupName);
            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            e.SetLabel(groupName, true);
            e.SetAddress(obj.name);
            var entriesAdded = new List<AddressableAssetEntry> {e};

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
        }
    }
}

#endif
