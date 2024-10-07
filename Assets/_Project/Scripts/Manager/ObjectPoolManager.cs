using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public enum eObjPoolType
{
    Ingame
}

public class ObjectPoolManager : GameObjectSingleton<ObjectPoolManager>
{
    #region AddressablePoolMethod

    public T GetPoolObject<T>(eObjPoolType type, string prefabName) where T : IAddressablePoolHandler
    {
        if (_objPoolAddressable.TryGetValue(prefabName, out var poolContainer))
        {
            poolContainer.pool.Get().poolObject.TryGetComponent<T>(out var value);
            return value;
        }

        return default;
    }

    public void ReturnPoolObject<T>(eObjPoolType type, string prefabName, T obj, Transform trParent = null) where T : IAddressablePoolHandler
    {
        if (_objPoolAddressable.TryGetValue(prefabName, out var poolContainer))
        {
            obj.poolObject.transform.SetParent(trParent != null ? trParent : poolContainer.parent.transform);
            poolContainer.pool.Release(obj);
        }
    }

    private IAddressablePoolHandler CreatePooledItemAddressable(string group, string prefabName, Transform parent)
    {
        var obj = AddressableManager.Inst.LoadInstance<IAddressablePoolHandler>(prefabName, parent);
        obj.poolObject.name = prefabName;
        return obj;
    }

    private void OnTakeFromPool(IAddressablePoolHandler addressablePoolGo)
    {
        addressablePoolGo.poolObject.SetActive(false);
    }

    private void OnReturnedToPool(IAddressablePoolHandler addressablePoolGo)
    {
        addressablePoolGo.ReturnToPool();
        addressablePoolGo.poolObject.SetActive(false);
    }

    private void OnDestroyPoolObjectAddressable(IAddressablePoolHandler addressablePoolGo)
    {
        addressablePoolGo.DestroyToPool();
        AddressableManager.Inst.ReleaseInstance(addressablePoolGo.poolObject);
    }

    #endregion

    public void LoadObjectPool(eObjPoolType type, string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName)) return;


        if (_objPoolAddressable.ContainsKey(prefabName))
        {
            return;
        }

        var parent = new GameObject(prefabName);

        if (parent == null)
        {
            return;
        }

        parent.transform.SetParent(transform);
        parent.transform.localPosition = Vector3.zero;
        parent.transform.localRotation = Quaternion.identity;
        parent.transform.localScale = Vector3.one;

        var pool = new ObjectPool<IAddressablePoolHandler>(
            () => CreatePooledItemAddressable(EnumUtils.GetEnumString(type), prefabName, parent.transform),
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObjectAddressable);

        var poolContainer = new ObjPoolContainer
        {
            parent = parent,
            pool = pool,
        };

        _objPoolAddressable.Add(prefabName, poolContainer);

        for (int i = 0; i < 1; i++)
        {
            var obj = pool.Get();
            pool.Release(obj);
        }
    }

    public void ReleasePool(eObjPoolType type, string prefabName)
    {
        if (_objPoolAddressable.TryGetValue(prefabName, out var poolContainer))
        {
            poolContainer.pool.Clear();
            AddressableManager.Inst.ReleaseObject(prefabName);
            _objPoolAddressable.Remove(prefabName);
        }
    }

    public void AllReleasePool()
    {
        foreach (var poolContainer in _objPoolAddressable)
        {
            poolContainer.Value.pool.Clear();
            AddressableManager.Inst.ReleaseObject(poolContainer.Key);
        }
        _objPoolAddressable.Clear();
        for (var i = this.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    private void OnTakeFromPool<T>(T poolGo) where T : Component
    {
        poolGo.SetActive(false);
    }

    private void OnReturnedToPool<T>(T poolGo) where T : Component
    {
        poolGo.SetActive(false);
    }

    public T GetPoolObject<T>(string key)
    {
        if (_objPool.TryGetValue(key, out var pool))
        {
            pool.Get().TryGetComponent<T>(out var value);
            return value;
        }

        return default;
    }

    public void ReturnPoolObject(string key, GameObject obj)
    {
        if (_objPool.TryGetValue(key, out var pool))
            pool.Release(obj);
    }

    private Dictionary<string, IObjectPool<GameObject>> _objPool = new();
    private Dictionary<string, ObjPoolContainer> _objPoolAddressable = new();
    private Dictionary<string, GameObject> _objPoolParent = new();

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    private GameObject CreatePooledItem(PoolData data)
    {
        var obj = Instantiate(data.Prefab, data.Parent);
        obj.name = data.Prefab.name;
        return obj;
    }

    private void OnDestroyPoolObject<T>(T poolGo) where T : Component
    {
        Destroy(poolGo.gameObject);
    }

    #region lifecycle

    protected override void OnDestroy()
    {
        AllReleasePool();
        base.OnDestroy();
    }

    #endregion
}

[Serializable]
public struct PoolData
{
    public string Key;
    public GameObject Prefab;
    public Transform Parent;
    public int Count;
}

public interface IAddressablePoolHandler
{
    void ReturnToPool();
    void DestroyToPool();
    GameObject poolObject { get; }
}

public class ObjPoolContainer
{
    public GameObject parent;
    public IObjectPool<IAddressablePoolHandler> pool;
}
