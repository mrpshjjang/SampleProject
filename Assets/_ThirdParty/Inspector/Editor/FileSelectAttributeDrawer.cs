/*
* Copyright (c) Sample.
*/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FileSelectAttribute))]
    internal class FileSelectAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dialogueOpened = false;

            var att = attribute as FileSelectAttribute;

            Rect rectButton = position;
            rectButton.width = 80;

            rectButton.x = position.xMax - 80;
            Rect rectField = position;
            rectField.width -= rectButton.width;

            bool enable = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(rectField, property, label);
            GUI.enabled = enable;

            if (GUI.Button(rectButton, new GUIContent("파일 선택")))
            {
                dialogueOpened = true;
                string rootDir = Application.dataPath;
                string select = EditorUtility.OpenFilePanelWithFilters("파일 선택", Application.dataPath, att!.Filters);
                if (!string.IsNullOrEmpty(select))
                {
                    string path = Helper.GetRelativePath(rootDir, select);
                    property.stringValue = path;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            if (dialogueOpened)
            {
                GUIUtility.ExitGUI();
            }
        }
    }
}
