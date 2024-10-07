/*
* Copyright (c) Sample.
*/

using UnityEditor;

namespace Sample.SpecData.Editor.Asset
{
    [CustomEditor(typeof(SpecDataAsset))]
    internal class SpecDataAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
