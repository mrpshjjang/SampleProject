/*
* Copyright (c) Sample.
*/

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(ValidateInputAttribute), true)]
    internal class ValidateInputAttributeDrawer : PropertyDrawer
    {
        private ValidateInputAttribute Attribute => attribute as ValidateInputAttribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Validate(property))
            {
                PropertyDrawerHelper.PropertyField(position, property);
            }
            else
            {
                PropertyDrawerHelper.DrawHelpBox(position, property, MessageType.Error, StyleUtil.errorBackgroundColor,
                    string.IsNullOrEmpty(Attribute.Message) ? $"검증 실패 : {Attribute.MethodName}" : Attribute.Message);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = PropertyDrawerHelper.GetValidateHelpBoxHeight(property, label, Validate);
            return height;
        }

        private bool Validate(SerializedProperty p)
        {
            return Validate(p, Attribute.MethodName);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool Validate(SerializedProperty p, string methodName)
        {
            object obj = SerializedUtil.GetTargetObjectOfProperty(p, true);
            if (obj == null)
            {
                return true;
            }

            MethodInfo methodInfo = ReflectionUtil.GetMethod(obj, methodName);
            if (methodInfo.ReturnType != typeof(bool))
                return true;

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length == 0)
            {
                return (bool)methodInfo.Invoke(obj, null);
            }

            if (parameterInfos.Length == 1)
            {
                FieldInfo field = ReflectionUtil.GetField(obj, p.name);
                return (bool)methodInfo.Invoke(obj, new []{field.GetValue(obj)});
            }

            return false;
        }
    }
}
