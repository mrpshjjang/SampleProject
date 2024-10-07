/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(PassWordAttribute), true)]
    internal class PassWordAttributeDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using var check = new EditorGUI.ChangeCheckScope();
            string password = EditorGUI.PasswordField(position, label, property.stringValue);
            if (!check.changed)
            {
                return;
            }

            property.stringValue = password;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
