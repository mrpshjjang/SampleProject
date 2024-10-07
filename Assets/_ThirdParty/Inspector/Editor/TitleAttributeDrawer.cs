/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(TitleAttribute), true)]
    internal class TitleAttributeDrawer : PropertyDrawer
    {
        private const int SpaceBeforeTitle = 9;
        private const int SpaceBeforeLine = 2;
        private const int LineHeight = 2;
        private const int SpaceBeforeContent = 5;

        private TitleAttribute Attribute => attribute as TitleAttribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label);
            float extraHeight = SpaceBeforeTitle +
                                EditorGUIUtility.singleLineHeight +
                                SpaceBeforeLine +
                                LineHeight
                                + SpaceBeforeContent;
            return height + extraHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var titleRect = new Rect(position)
            {
                y = position.y + SpaceBeforeTitle,
                height = EditorGUIUtility.singleLineHeight,
            };

            var lineRect = new Rect(position)
            {
                y = titleRect.yMax + SpaceBeforeLine,
                height = LineHeight,
            };

            string title = Attribute.Title;
            GUI.Label(titleRect, title, EditorStyles.boldLabel);

            if (Attribute.HorizontalLine)
            {
                EditorGUI.DrawRect(lineRect, Color.gray);
            }

            var contentRect = new Rect(position)
            {
                yMin = lineRect.yMax + SpaceBeforeContent,
            };
            EditorGUI.PropertyField(contentRect, property, label, true);
        }
    }
}
