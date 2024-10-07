/*
* Copyright (c) Sample.
*/

using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    internal class RequiredAttributeDrawer : PropertyDrawer
    {
        private RequiredAttribute Attribute => attribute as RequiredAttribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = PropertyDrawerHelper.GetValidateHelpBoxHeight(property, label, Validate);
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isObjectReferenceType = IsObjectReferenceType(property);
            if (isObjectReferenceType)
            {
                if (property.objectReferenceValue)
                {
                    PropertyDrawerHelper.PropertyField(position, property);
                }
                else
                {
                    PropertyDrawerHelper.DrawHelpBox(position, property, MessageType.Error, StyleUtil.errorBackgroundColor, "참조가 필요합니다.");
                }
            }
            else
            {
                PropertyDrawerHelper.DrawHelpBox(position, property, MessageType.Warning, StyleUtil.normalBackgroundColor, $"{attribute} 미지원 타입 입니다.");
            }
        }

        private static bool Validate(SerializedProperty p)
        {
            // 오브젝트 레퍼런스가 없으면 box 오류, 경고
            return IsObjectReferenceType(p) && p.objectReferenceValue != null;
        }

        private static bool IsObjectReferenceType(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }

        public static bool ValidateError(SerializedProperty property)
        {
            if (!IsObjectReferenceType(property))
                return true;
            return property.objectReferenceValue != null;
        }
    }
}
