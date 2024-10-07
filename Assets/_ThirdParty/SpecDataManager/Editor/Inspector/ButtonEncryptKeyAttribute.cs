/*
* Copyright (c) Sample.
*/

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Sample.SpecData.Editor.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class ButtonEncryptKeyAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(ButtonEncryptKeyAttribute))]
    internal class ButtonEncryptKeyPropertyDrawer : PropertyDrawer
    {
        private static Texture2D _CaretTexture;
        private Rect _assetBtnRect;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _assetBtnRect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginProperty(position, label, property);
            Rect posLabel = position;
            EditorGUI.LabelField(posLabel, label);
            Rect posTextField = _assetBtnRect;
            posTextField.width -= 24;
            string value = property.stringValue;
            bool old = GUI.enabled;
            GUI.enabled = false;
            string key = EditorGUI.DelayedTextField(posTextField, value, EditorStyles.textField);
            if (string.IsNullOrEmpty(key) || key.Length != 16)
            {
                SetNewEncryptKey(property);
            }
            GUI.enabled = old;
            DrawRefObject(position, property);
            EditorGUI.EndProperty();
        }

        private void DrawRefObject(Rect position, SerializedProperty property)
        {
            bool isDragging = Event.current.type == EventType.DragUpdated &&
                              position.Contains(Event.current.mousePosition);
            bool isDropping = Event.current.type == EventType.DragPerform &&
                              position.Contains(Event.current.mousePosition);

            DrawControl(property, isDragging, isDropping);
        }

        private void DrawControl(SerializedProperty property, bool isDragging, bool isDropping)
        {
            const float pickerWidth = 20f;
            Rect pickerRect = _assetBtnRect;
            pickerRect.width = pickerWidth;
            pickerRect.x = _assetBtnRect.xMax - pickerWidth;

            bool isPickerPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                                   pickerRect.Contains(Event.current.mousePosition);
            bool isEnterKeyPressed = Event.current.type == EventType.KeyDown && Event.current.isKey &&
                                     (Event.current.keyCode == KeyCode.KeypadEnter ||
                                      Event.current.keyCode == KeyCode.Return);
            if (isPickerPressed || isDragging || isDropping || isEnterKeyPressed)
            {
                Event.current.Use();
            }

            DrawCaret(pickerRect);

            if (isPickerPressed)
            {
                SetNewEncryptKey(property);
            }
        }

        private void DrawCaret(Rect pickerRect)
        {
            if (_CaretTexture == null)
            {
                _CaretTexture = EditorGUIUtility.IconContent("d_preAudioLoopOff").image as Texture2D;
            }

            if (_CaretTexture != null)
            {
                GUI.DrawTexture(pickerRect, _CaretTexture, ScaleMode.ScaleToFit);
            }
        }

        private void SetNewEncryptKey(SerializedProperty property)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
            var key =  new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            property.stringValue = key;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
