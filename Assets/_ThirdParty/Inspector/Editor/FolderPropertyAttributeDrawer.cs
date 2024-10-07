using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sample.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(FolderAttribute), useForChildren:true)]
    internal class FolderPropertyAttributeDrawer : PropertyDrawer
    {
        private static Texture2D _CaretTexture;
        private Rect _assetDropDownRect;
        private FolderAttribute _folderAttribute;
        private const string _ControlNameFolderDelayedTextField = nameof(_ControlNameFolderDelayedTextField);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _folderAttribute = attribute as FolderAttribute;
            _assetDropDownRect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginProperty(position, label, property);
            Rect posLabel = position;
            EditorGUI.LabelField(posLabel, label);
            Rect posTextField = _assetDropDownRect;
            posTextField.width -= 24;
            string value = property.stringValue;
            GUI.SetNextControlName(_ControlNameFolderDelayedTextField);
            string newValue = EditorGUI.DelayedTextField(posTextField, value, EditorStyles.textField);
            if (!value.Equals(newValue) && _ControlNameFolderDelayedTextField.Equals(GUI.GetNameOfFocusedControl()))
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    property.stringValue = string.Empty;
                }
                else
                {
                    SetFolder(property, newValue);
                }
            }
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
            Rect pickerRect = _assetDropDownRect;
            pickerRect.width = pickerWidth;
            pickerRect.x = _assetDropDownRect.xMax - pickerWidth;

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
            HandleDragAndDrop(property, isDragging, isDropping);

            if (!isPickerPressed)
            {
                return;
            }

            string path = EditorUtility.OpenFolderPanel("폴더 선택", Application.dataPath, "");
            SetFolder(property, path);
            if (_ControlNameFolderDelayedTextField.Equals(GUI.GetNameOfFocusedControl()))
            {
                EditorGUI.FocusTextInControl(string.Empty);
            }
        }

        private static void DrawCaret(Rect pickerRect)
        {
            if (_CaretTexture == null)
            {
                _CaretTexture = EditorGUIUtility.IconContent("d_FolderOpened Icon").image as Texture2D;
            }

            if (_CaretTexture != null)
            {
                GUI.DrawTexture(pickerRect, _CaretTexture, ScaleMode.ScaleToFit);
            }
        }


        private void HandleDragAndDrop(SerializedProperty property, bool isDragging, bool isDropping)
        {
            var rejectedDrag = false;
            if (isDragging)
            {
                rejectedDrag = IsRejectedDrag();
                DragAndDrop.visualMode = rejectedDrag ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Copy;
            }

            if (!rejectedDrag && isDropping)
            {
                string path = DragAndDrop.paths.FirstOrDefault();
                SetFolder(property, path);
            }

            //--------------------------------------------------------------------------------------

            // 드래그 & 드랍 조건
            bool IsRejectedDrag()
            {
                string path = DragAndDrop.paths.FirstOrDefault();
                if (string.IsNullOrEmpty(path))
                {
                    return true;
                }

                // 디렉토리 검사
                if (!Directory.Exists(path))
                {
                    return true;
                }
                // 타입 검사
                var obj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                if (obj == null)
                {
                    return true;
                }

                return false;
            }
        }

        private void SetFolder(SerializedProperty property, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            string dir = path;
            bool isRooted = Path.IsPathRooted(path);
            if (!_folderAttribute.AbsolutePath && isRooted)
            {
                var rootAssets = Path.Combine(Application.dataPath, "../");
                bool isSubFolder = Helper.IsSubFolder(rootAssets, path);
                if (!isSubFolder)
                {
                    property.stringValue = string.Empty;
                    return;
                }

                var rootInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                var pathInfo = new DirectoryInfo(path);
                dir = Helper.GetRelativePath(rootInfo, pathInfo);
            }

            property.stringValue = dir;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
