using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorSettings))]
public class AnimatorSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.Space(10);
        serializedObject.DrawPropertyField("controllers"); 
        CustomEditorUtility.DrawLine(2, Color.green);   
        EditorGUILayout.Space(15);
        serializedObject.DrawPropertyField("stateNamePart");
        CustomEditorUtility.DrawLine(2, Color.green);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        serializedObject.DrawPropertyField("isAnyState");
        EditorGUILayout.Separator();
        SerializedProperty isBelndTreeCheck = serializedObject.DrawPropertyField("isBlendTree");
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        CustomEditorUtility.DrawLine(2, Color.green);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        SerializedProperty hasSpeedParameter = serializedObject.DrawPropertyField("hasSpeedParameter");
        if(hasSpeedParameter.boolValue)
        {
            EditorGUILayout.Separator();
            serializedObject.DrawPropertyFieldStringValueLabel("speedParameterName");
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        CustomEditorUtility.DrawLine(2, Color.green);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);
        if (isBelndTreeCheck.boolValue)
        {
            serializedObject.DrawPropertyFieldObjectValueLabel("blendTreeData");
            EditorGUILayout.Space(10);
        }
        serializedObject.DrawPropertyFieldObjectValueLabel("transitionData");
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
}