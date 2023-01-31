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
        // 스테이트 이름
        serializedObject.DrawPropertyField("controllers"); 
        CustomEditorUtility.DrawLine(2, Color.green);   
        EditorGUILayout.Space(15);
        serializedObject.DrawPropertyField("stateNamePart");
        CustomEditorUtility.DrawLine(2, Color.green);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        // 애니스테이트 연결?
        serializedObject.DrawPropertyField("isAnyState");
        EditorGUILayout.Separator();
        // 블렌드 트리 ?
        SerializedProperty isBelndTreeCheck = serializedObject.DrawPropertyField("isBlendTree");
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        CustomEditorUtility.DrawLine(2, Color.green);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        // 속도 제어 파라미터 사용
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