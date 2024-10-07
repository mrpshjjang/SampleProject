using UnityEngine;
using System.Collections;

public abstract class GameObjectSingletonBase : MonoBehaviour
{
    public abstract bool AllowMultiInstance { get; }
    public abstract void SetAsSingleton(bool set);
}

public class GameObjectSingleton<T> : GameObjectSingletonBase where T : GameObjectSingleton<T>
{
    public static bool Loaded => inst != null && inst.Valid;

    public static T Instance
    {
        get
        {
            if (inst == null)
            {
                var temp = FindObjectOfType(typeof(T)) as T;
                if (temp != null)
                {
                    temp.TryAttach();
                }
            }

            return inst;
        }
    }

    protected static T Inst => inst;

    /////////////////////////////////////////////////////////////////
    // instance member
    public override bool AllowMultiInstance => false;

    public override void SetAsSingleton(bool set)
    {
        if (!AllowMultiInstance)
        {
            Debug.LogError(typeof(T).Name + " : AllowMultiInstance is false");
            return;
        }

        if (set)
        {
            if (ReferenceEquals(inst, this))
            {
                return;
            }

            inst = this as T;
            OnAttached();
        }
        else
        {
            if (!ReferenceEquals(inst, this))
            {
                return;
            }

            inst = null;
            OnDetached();
        }
    }

    protected virtual bool Valid => inst != null;

    private void TryAttach()
    {
        if (inst != null)
        {
            if (!AllowMultiInstance)
            {
                Debug.LogError(typeof(T).Name + " is already attached");
            }

            return;
        }

        inst = this as T;

        OnAttached();
    }

    protected virtual void Awake()
    {
        if (inst == null || AllowMultiInstance)
        {
            TryAttach();
        }
    }

    protected virtual void OnDestroy()
    {
        if (ReferenceEquals(this, inst))
        {
            //inst = null;
            OnDetached();
        }
    }

    protected virtual void OnAttached() { }

    protected virtual void OnDetached() { }

    public static void SetAsSingleton(GameObject obj, bool set)
    {
        foreach (var comp in obj.GetComponents<GameObjectSingletonBase>())
        {
            comp.SetAsSingleton(set);
        }
    }

    /////////////////////////////////////////////////////////////////
    // private
    protected static T inst = null;
}
