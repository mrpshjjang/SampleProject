using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConfigCreator : MonoBehaviour
{
    [MenuItem("PxP/User/Edit %#&u")]
    public static void EditUserConfiguration()
    {
        var instance = UserConfig.Instance;
        EditorGUIUtility.PingObject(UserConfig.Instance);
        Selection.activeObject = UserConfig.Instance;
    }
    [MenuItem("PxP/User/Create")]
    public static void CreateUserConfiguration()
    {
        var instance = new UserConfig();
        AssetDatabase.CreateAsset(instance, "Assets/Resources/UserConfig.asset");
    }
}
