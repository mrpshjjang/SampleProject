using UnityEditor;

[CustomEditor(typeof(UCTabGroup), true)]
[CanEditMultipleObjects]
public class UCTabGroupEditor : Editor
{
    private SerializedProperty selectorProperty;
    private SerializedProperty selectorDirectionProperty;
    private SerializedProperty selectorMoveDurationProperty;
    private SerializedProperty tabsProperty;

    public override void OnInspectorGUI()
    {
        var tabGroup = target as UCTabGroup;

        EditorGUILayout.LabelField("Selector 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(selectorProperty);

        if (tabGroup.SelectorFlag)
        {
            EditorGUILayout.PropertyField(selectorDirectionProperty);
            EditorGUILayout.PropertyField(selectorMoveDurationProperty);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("탭 버튼 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tabsProperty);

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnEnable()
    {
        selectorProperty = serializedObject.FindProperty("selector");
        selectorDirectionProperty = serializedObject.FindProperty("selectorDirection");
        selectorMoveDurationProperty = serializedObject.FindProperty("selectorMoveDuration");
        tabsProperty = serializedObject.FindProperty("tabs");
    }
}