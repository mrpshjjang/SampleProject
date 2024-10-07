using UnityEngine;
using System;
using UnityEditor;

[CustomPropertyDrawer(typeof(SearchableAttribute))]
public class SearchableAttributeDrawer : PropertyDrawer
{
    private string search;
    private string[] options;
    private GUIStyle searchTextFieldStyle;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);

        if (property.propertyType == SerializedPropertyType.Enum)
            height = height * 2 + EditorGUIUtility.standardVerticalSpacing;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            if (searchTextFieldStyle == null)
                searchTextFieldStyle = GUI.skin.FindStyle("ToolbarSearchTextField");

            if (options == null)
                UpdateOptions(property.enumDisplayNames);

            Rect searchRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DrawSearchBar(searchRect, label, property.enumDisplayNames);

            Rect popupRect = new Rect(position.x, searchRect.y + searchRect.height + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            DrawEnumPopup(popupRect, property);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    private void DrawSearchBar(Rect position, GUIContent label, string[] allOptions)
    {
        EditorGUI.BeginChangeCheck();
        search = EditorGUI.TextField(position, label, search, searchTextFieldStyle);
        if(EditorGUI.EndChangeCheck())
            UpdateOptions(allOptions);
    }

    private void DrawEnumPopup(Rect position, SerializedProperty property)
    {
        Rect fieldRect = EditorGUI.PrefixLabel(position, new GUIContent(" "));
        int currentIndex = Array.IndexOf(options, property.enumDisplayNames[property.enumValueIndex]);
        int selectedIndex = EditorGUI.Popup(fieldRect, currentIndex, options);
        if (selectedIndex >= 0)
        {
            int newIndex = Array.IndexOf(property.enumDisplayNames, options[selectedIndex]);
            if (newIndex != currentIndex)
            {
                property.enumValueIndex = newIndex;
                search = string.Empty;
                UpdateOptions(property.enumDisplayNames);
            }
        }
        GUILine(4);
    }

    void GUILine( int lineHeight = 1 ) {
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, lineHeight );
        rect.height = lineHeight;
        EditorGUI.DrawRect(rect, new Color ( 0.0f,0.0f,0.0f, 0 ) );
        EditorGUILayout.Space();
    }

    private void UpdateOptions(string[] allOptions)
    {
        options = Array.FindAll(allOptions, name => string.IsNullOrEmpty(search) || name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0);
    }
}
