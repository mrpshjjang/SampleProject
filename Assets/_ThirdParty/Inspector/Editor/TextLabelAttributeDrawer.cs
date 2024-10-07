/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(TextLabelAttribute), true)]
    public class TextLabelAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        private TextLabelAttribute Attribute => attribute as TextLabelAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, PropertyDrawerHelper.TempGUIContent(Attribute.Text), true);
        }
    }
}
