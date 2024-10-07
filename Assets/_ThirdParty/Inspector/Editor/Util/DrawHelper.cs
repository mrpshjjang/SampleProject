/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    internal static class DrawHelper
    {
        public static void DrawEmptyProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            label ??= new GUIContent(property.displayName);
            label.image = GetHelpIcon(MessageType.Warning);
            EditorGUI.LabelField(position, label);
        }


        public static Texture GetHelpIcon(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Info => EditorGUIUtility.IconContent("console.infoicon").image,
                MessageType.Warning => EditorGUIUtility.IconContent("console.warnicon").image,
                MessageType.Error => EditorGUIUtility.IconContent("console.erroricon").image,
                _ => null,
            };
        }
    }
}
