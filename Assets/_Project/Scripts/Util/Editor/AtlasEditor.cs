using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AtlasEditor : MonoBehaviour
{
    [MenuItem("PxP/SpriteAtlas/Edit %#&a")]
    public static void EditUserConfiguration()
    {
        var instance = AtlasCreator.Instance;
        EditorGUIUtility.PingObject(AtlasCreator.Instance);
        Selection.activeObject = AtlasCreator.Instance;
    }
    [MenuItem("PxP/SpriteAtlas/Create")]
    public static void CreateUserConfiguration()
    {
        var instance = new AtlasCreator();
        AssetDatabase.CreateAsset(instance, "Assets/Resources/AtlasCreator.asset");
    }
}
