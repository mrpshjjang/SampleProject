/*
* Copyright (c) Sample.

*/

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(OnChangedAttribute))]
    internal class OnChangedAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            object obj = SerializedUtil.GetTargetObjectOfProperty(property, true);
            if (obj == null)
            {
                return;
            }

            var at = attribute as OnChangedAttribute;

            MethodInfo[] methodInfos = obj.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToArray();
            MethodInfo method = methodInfos.FirstOrDefault(m => m.Name == at!.methodName);

            if (method != null && !method.GetParameters().Any())
            {
                EditorApplication.delayCall += () =>
                {
                    method.Invoke(obj, null);
                    property.serializedObject.ApplyModifiedProperties();
                    // EditorUtility.SetDirty(property.serializedObject.targetObject);
                    // AssetDatabase.SaveAssetIfDirty(property.serializedObject.targetObject);
                };
            }
        }
    }
}
