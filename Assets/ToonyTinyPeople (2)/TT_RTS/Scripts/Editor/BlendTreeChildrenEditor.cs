using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BlendTreeChildren))]
public class BlendTreeChildrenEditor : Editor
{
    private BlendTreeChildren data;

    private ReorderableList motionList;
    private bool motionListFoldOut;
    private void OnEnable()
    {
        data = target as BlendTreeChildren;
        motionList = serializedObject.DrawReorderList("motions");
        motionList.DrawContents(
            1,
            "Animation clip's which is Making up blendTree information",
            10,
            new []
            {
                (140, "Animation Clip",120,"clip"),
                (140, "Threshold",100,"thresholds")
            });
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);

        // 블렌드 타입
        SerializedProperty blendTreeType = serializedObject.DrawPropertyField("blendTreeType");
        CustomEditorUtility.DrawLine(2,Color.green);
        BlendTreeType typeValue = (BlendTreeType)blendTreeType.enumValueIndex;
        // 블렌드 파라미터
        SerializedProperty parameterAxisX = serializedObject.FindProperty("parameterAxisX");
        switch (typeValue)
            {
                default:
                case BlendTreeType.Simple1D:
                    parameterAxisX.stringValue = EditorGUILayout.TextField("1D Parameter ", parameterAxisX.stringValue);
                    break;
                case BlendTreeType.SimpleDirectional2D:
                case BlendTreeType.FreeformDirectional2D:
                case BlendTreeType.FreeformCartesian2D:
                    SerializedProperty parameterAxisY = serializedObject.FindProperty("parameterAxisY");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space(10);
                        parameterAxisX.stringValue = EditorGUILayout.TextField("2D Axis X", parameterAxisX.stringValue);
                        parameterAxisY.stringValue = EditorGUILayout.TextField("2D Axis Y", parameterAxisY.stringValue);
                        EditorGUILayout.Space(10);
                        EditorGUILayout.EndHorizontal();
                        break;
                case BlendTreeType.Direct:
                    throw new ArgumentOutOfRangeException(" - Not Support Direct Type - ");
            }
        CustomEditorUtility.DrawLine(2,Color.green);
        // 리스트 그리기
        motionList.ApplyReorderLayoutList();
        CustomEditorUtility.DrawLine(2,Color.green);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}