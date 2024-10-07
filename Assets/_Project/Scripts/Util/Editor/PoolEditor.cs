using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PoolEditor : MonoBehaviour
{
    [MenuItem("PxP/Pool/Edit %#&o")]
    public static void EditUserConfiguration()
    {
        var instance = PoolCreator.Instance;
        EditorGUIUtility.PingObject(PoolCreator.Instance);
        Selection.activeObject = PoolCreator.Instance;
    }
    [MenuItem("PxP/Pool/Create")]
    public static void CreateUserConfiguration()
    {
        var instance = new PoolCreator();
        AssetDatabase.CreateAsset(instance, "Assets/Resources/PoolCreator.asset");
    }
}
