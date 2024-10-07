using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : GameObjectSingleton<DontDestroyObject>
{
    protected override void Awake()
    {
        if (Loaded)
        {
            DestroyImmediate(gameObject);
            return;
        }

        base.Awake();
    }

    protected void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        Destroy(gameObject);
    }
}