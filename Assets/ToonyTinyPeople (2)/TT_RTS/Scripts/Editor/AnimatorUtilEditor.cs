using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using static CustomEditorUtility;

[CustomEditor(typeof(AnimatorUtil))]
public class AnimatorUtilEditor : Editor
{
    private AnimatorUtil data;
    private ReorderableList parameterDatas;
    private ReorderableList animatorSettings;
    
    private bool parameterFoldOut = false;
    private bool animatorSettingsFoldOut = false;
    private void OnEnable()
    {
        data = target as AnimatorUtil;
        parameterDatas = serializedObject.DrawReorderList("ParameterDatas");
        parameterDatas 
            .DrawContents(
                2,
                "Managing Parameters",
                60,
                new[]
                {
                    (120, "Key", 25, "parameterName"),
                    (80, "Type",40, "parameterType"),
                });
        animatorSettings = serializedObject.DrawReorderList("SettingsList");
        animatorSettings
            .DrawContents(
                0,
                "Managing State & Transition",
                15,
                null);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
      
        EditorGUILayout.Space(10);  
        parameterFoldOut = DrawFoldOut(parameterFoldOut, "Managing Parameter", parameterDatas.ApplyReorderLayoutList);
        DrawLine(2, Color.green);
        
        animatorSettingsFoldOut = DrawFoldOut(animatorSettingsFoldOut, "Managing State & Transition", animatorSettings.ApplyReorderLayoutList);
        DrawLine(2, Color.green);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField($"Init Code Snippet");
        SerializedProperty snippetProperty = serializedObject.FindProperty("parameterNameSnippet");
        snippetProperty.stringValue = data.WriteSnippetSetParameter();
        EditorGUILayout.SelectableLabel
        (
            string.IsNullOrEmpty(snippetProperty.stringValue) ? "" : snippetProperty.stringValue,
            new GUIStyle(GUI.skin.textArea)
            {
                richText = true,
                padding = new RectOffset(10,10,10,10)
            },
            new GUILayoutOption[]
            {
                GUILayout.MinHeight(128),
                GUILayout.MaxHeight(256),
            });     
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField($"Reset Code Snippet");
        SerializedProperty methodSnippetProperty = serializedObject.FindProperty("parameterResetSnippet");
        methodSnippetProperty.stringValue = data.WriteSnippetResetParameterMethod();
        EditorGUILayout.SelectableLabel
        (
            string.IsNullOrEmpty(methodSnippetProperty.stringValue) ? "" : methodSnippetProperty.stringValue,
            new GUIStyle(GUI.skin.textArea)
            {
                richText = true,
                padding = new RectOffset(10,10,10,10)
            },
            new GUILayoutOption[]
            {
                GUILayout.MinHeight(128),
                GUILayout.MaxHeight(256),
            }); 
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        DrawLine(2, Color.green);
        if (GUILayout.Button("Set Up", new GUILayoutOption[]{GUILayout.Height(32)}))
        {
            data.SetParameters();
            data.SetTransition();
        }
        EditorGUILayout.EndVertical();      
        serializedObject.ApplyModifiedProperties();
    }
}
