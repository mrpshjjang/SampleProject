using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(UCButton), true)]
[CanEditMultipleObjects]
public class UCButtonEditor : ButtonEditor
{
    private SerializedProperty keepClickProperty;
    private SerializedProperty pressTypeProperty;
    private SerializedProperty pressScaleXProperty;
    private SerializedProperty pressScaleYProperty;
    private SerializedProperty pressJellyMinProperty;
    private SerializedProperty pressJellyMaxProperty;
    private SerializedProperty durationProperty;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var button = target as UCButton;

        EditorGUILayout.PropertyField(keepClickProperty);
        EditorGUILayout.PropertyField(pressTypeProperty);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        switch (button.PressType)
        {
            case eButtonPressType.Scale:

                EditorGUILayout.LabelField("Scale Type", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(pressScaleXProperty);
                EditorGUILayout.PropertyField(pressScaleYProperty);
                EditorGUILayout.EndHorizontal();

                break;

            case eButtonPressType.Jelly:

                EditorGUILayout.LabelField("Jelly Type", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(pressJellyMinProperty);
                EditorGUILayout.PropertyField(pressJellyMaxProperty);
                EditorGUILayout.EndHorizontal();

                break;
                ;
        }

        // EditorGUILayout.PropertyField(this.durationProperty);

        serializedObject.ApplyModifiedProperties();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        keepClickProperty = serializedObject.FindProperty("isKeepClick");
        pressTypeProperty = serializedObject.FindProperty("pressType");
        pressScaleXProperty = serializedObject.FindProperty("pressScaleX");
        pressScaleYProperty = serializedObject.FindProperty("pressScaleY");
        pressJellyMinProperty = serializedObject.FindProperty("pressJellyMin");
        pressJellyMaxProperty = serializedObject.FindProperty("pressJellyMax");
        durationProperty = serializedObject.FindProperty("animDuration");
    }
}
