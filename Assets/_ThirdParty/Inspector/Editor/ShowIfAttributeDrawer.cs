/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
    internal class ShowIfAttributeDrawer : PropertyDrawer
    {
        private bool Condition(SerializedProperty property)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            SerializedProperty condition = FindSiblingProperty(property, showIfAttribute!.Condition);
            if (condition == null)
            {
                return true;
            }

            bool c = condition.boolValue;
            return c;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            bool condition = Condition(property) == showIfAttribute!.Value;
            if (!condition)
            {
                return 0;
            }

            float height = EditorGUI.GetPropertyHeight(property, label);
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            bool meetsCondition = Condition(property) == showIfAttribute!.Value;
            if (!meetsCondition)
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        //----------------------------------------------------------------------------------------

        private static SerializedProperty GetParent(SerializedProperty property)
        {
            string path = property.propertyPath;
            int i = path.LastIndexOf('.');
            return i < 0 ? null : property.serializedObject.FindProperty(path.Substring(0, i));
        }

        private static SerializedProperty FindSiblingProperty(SerializedProperty property, string path)
        {
            if (string.IsNullOrEmpty(path))
                return default;

            SerializedProperty parent = GetParent(property);
            return parent != default ? parent.FindPropertyRelative(path) : property.serializedObject.FindProperty(path);
        }
    }
}
