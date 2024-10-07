/*
* Copyright (c) Sample.
*/

using System;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    internal static class PropertyDrawerHelper
    {
        private static readonly GUIContent tempGUIContent = new();
        public static GUIContent TempGUIContent(string text)
        {
            tempGUIContent.text = text;
            return tempGUIContent;
        }

        public static GUIContent TempGUIContent(SerializedProperty property)
        {
            tempGUIContent.text = property.displayName;
            return tempGUIContent;
        }

        public static void PropertyField(Rect position, SerializedProperty property)
        {
            position.height = EditorGUI.GetPropertyHeight(property);
            EditorGUI.PropertyField(position, property, TempGUIContent(property), property.isExpanded);
        }

        public static void DrawHelpBox(Rect position, SerializedProperty property, MessageType messageType, Color color, string message)
        {
            var helpBoxRect = new Rect(position.x, position.y, position.width, StyleUtil.boxHeight);
            EditorGUI.HelpBox(helpBoxRect, message, messageType);
            position.y += StyleUtil.boxHeight + StyleUtil.spacing;
            Color oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            PropertyField(position, property);
            GUI.backgroundColor = oldBackgroundColor;
        }

        // 검증 이후 HelpBox 높이를 반환 (기본 높이 + HelpBox 높이)
        public static float GetValidateHelpBoxHeight(SerializedProperty property, GUIContent label, Func<SerializedProperty, bool> validate)
        {
            float height = EditorGUI.GetPropertyHeight(property, label);
            if (!validate.Invoke(property))
            {
                height += StyleUtil.boxHeight + StyleUtil.spacing;
            }
            return height;
        }
    }
}
