using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using SerializedProperty = UnityEditor.SerializedProperty;

public static class CustomEditorUtility
{
    #region string Utility

    public static string StartWithLowerChar(this string input)
    {
        Span<char> result = new Span<char>(input.ToCharArray());
        result[0] = char.ToLower(input[0]);
        return result.ToString();
    }

    

    #endregion
    #region Property Field
    public static SerializedProperty DrawPropertyField(this SerializedObject serializedObject, string path, string label = "")
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        
        if(string.IsNullOrEmpty(label))
            EditorGUILayout.PropertyField(property);
        else
            EditorGUILayout.PropertyField(property, new GUIContent(label));
        
        return property;
    }
    public static SerializedProperty DrawPropertyFieldStringValueLabel(this SerializedObject serializedObject, string path, string label = "")
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        
        if(string.IsNullOrEmpty(label))
            EditorGUILayout.PropertyField(property, new GUIContent($"{property.displayName} : {property.stringValue}"));
        else
            EditorGUILayout.PropertyField(property, new GUIContent($"{label} : {property.stringValue}"));
        
        return property;
    }
    public static SerializedProperty DrawPropertyFieldIntValueLabel(this SerializedObject serializedObject, string path, string label = "")
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        
        if(string.IsNullOrEmpty(label))
            EditorGUILayout.PropertyField(property, new GUIContent($"{property.displayName} : {property.intValue}"));
        else
            EditorGUILayout.PropertyField(property, new GUIContent($"{label} : {property.intValue}"));
        
        return property;
    }
    public static SerializedProperty DrawPropertyFieldFloatValueLabel(this SerializedObject serializedObject, string path, string label = "")
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        
        if(string.IsNullOrEmpty(label))
            EditorGUILayout.PropertyField(property, new GUIContent($"{property.displayName} : {property.floatValue}"));
        else
            EditorGUILayout.PropertyField(property, new GUIContent($"{label} : {property.floatValue}"));
        
        return property;
    }
    public static SerializedProperty DrawPropertyFieldObjectValueLabel(this SerializedObject serializedObject, string path, string label = "")
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        
        if(string.IsNullOrEmpty(label))
            EditorGUILayout.PropertyField(property, new GUIContent($"{property.displayName} : {(property.objectReferenceValue is null ? "Empty" : property.objectReferenceValue.name)}"));
        else
            EditorGUILayout.PropertyField(property, new GUIContent($"{label} : {(property.objectReferenceValue is null ? "Empty" : property.objectReferenceValue.name)}"));
        
        return property;
    }
    #endregion
    
    public static void DrawPreview(this SerializedObject serializedObject, string path, Vector2 previewSize)
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        if (property is null)
        {
            return;
        }
        Rect source = EditorGUILayout.BeginHorizontal();
        Texture2D texture2D = property.objectReferenceValue as Texture2D;
        EditorGUI.DrawPreviewTexture(new Rect(source.position, previewSize), texture2D);
        EditorGUILayout.EndHorizontal();
    }
    public static void DrawPreviewWithField(this SerializedObject serializedObject, string path, Vector2 previewSize)
    {
        SerializedProperty property = serializedObject.FindProperty(path);
        if (property is null) return;
        
        Texture2D texture2D = property.objectReferenceValue as Texture2D;
        if (texture2D is null) return;
        
        EditorGUILayout.Space(20);
        Rect source = EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        EditorGUI.DrawPreviewTexture(new Rect(source.position, previewSize), texture2D);
        EditorGUILayout.ObjectField(property, typeof(Texture2D));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
    }
    public static void DrawLine(int height, Color color, int spaceTop = 30, int spaceBottom = 30)
    {
        EditorGUILayout.Space(spaceTop);
        Rect source = EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(new Rect(source.position, new Vector2(source.width, height)), color);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(spaceBottom);
    }

    #region FoldOut

    public static bool DrawFoldOut(bool foldOut, string label, Action callBack)
    {
        foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(foldOut, label);

        if (foldOut) callBack?.Invoke();
        
        EditorGUILayout.EndFoldoutHeaderGroup();

        return foldOut;
    }

    #endregion

    #region ReorderList
    public static ReorderableList DrawReorderList(this SerializedObject serializedObject, string listPath)
    {
        return new ReorderableList(serializedObject, serializedObject.FindProperty(listPath), true, true, true, true);
    }

    public static void DrawContents(this ReorderableList reorderableList, int limitColumn, string header, int space, params (int fieldWidth, string label, int space, string elementPropertyPath)[] elementInfo )
    {
        void drawHead(Rect rect)
        {
            EditorGUI.LabelField(rect, header);
        }

        void drawMultiElementHeader(Rect rect, SerializedProperty element , int index)
        {
            SerializedProperty header = element.FindPropertyRelative(elementInfo[0].elementPropertyPath);
            string label = null;
            if (header.type.ToLower().Contains("string"))
            {
                label = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} : {header.stringValue}";
            }
            if (header.type.ToLower().Contains("object"))
            {
                label = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} : {header.objectReferenceValue.name}";
            }

            if (string.IsNullOrEmpty(label))
            {
                label = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} ";
            }
            EditorGUI.LabelField(
                new Rect(rect.x
                    , rect.y
                    , elementInfo[0].fieldWidth + space
                    , EditorGUIUtility.singleLineHeight)
                ,label);
        }

        void drawSingleElementRect(Rect rect, SerializedProperty element, int index, int elementWidth = 260)
        {
            GUIContent elementLabel = new GUIContent();

            if (element.type.ToLower().Contains("string"))
            {
                elementLabel.text = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} : {element.stringValue}";
            }

            if (element.type.ToLower().Contains("object"))
            {
                elementLabel.text = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} : {element.objectReferenceValue.name}";
            }

            if (string.IsNullOrEmpty(elementLabel.text))
            {
                elementLabel.text = $"{index}{index switch { 2 => "nd", 3 => "rd", _ => "st" }} : {(element.objectReferenceValue is null ? "Empty" : $"{element.objectReferenceValue.name}")}";
            }

            EditorGUI.LabelField(
                new Rect( rect.x, rect.y, elementWidth + space, EditorGUIUtility.singleLineHeight)
                ,elementLabel);
            
            EditorGUI.PropertyField(
                new Rect(rect.x + elementWidth + space, rect.y, elementWidth, EditorGUIUtility.singleLineHeight),
                element,
                GUIContent.none);
        }
        void drawElementRect(Rect rect, SerializedProperty element)
        {
            int lineCount = 0;
            for (int i = 0; i < elementInfo.Length; i++)
            {
                if (i % limitColumn == 0 && i != 0) lineCount++;
                
                // limitColumn에 따라, n by n 행렬 레이아웃 구성.
                float positionX = rect.x + (i % limitColumn == 0 ? 0 : elementInfo[i].space * 0.5f + space) +
                                  space + 2f * elementInfo[i].fieldWidth *
                                  ((lineCount == i / limitColumn ? i % limitColumn : i) + 1);
                float positionY = lineCount == i / limitColumn
                    ? rect.y + 2 * EditorGUIUtility.singleLineHeight * lineCount
                    : rect.y;
                // 각 파라미터 레이블
                float elementNameWidth = elementInfo[i].label.Length * 14;
                    
                // 항목 그리기 라벨 - 필드 (가로형)
                    
                EditorGUI.LabelField(
                    new Rect( positionX, positionY, elementNameWidth + space, EditorGUIUtility.singleLineHeight)
                    ,$"{elementInfo[i].label}");
                    
                EditorGUI.PropertyField(
                    new Rect(positionX + elementInfo[i].space, positionY, elementInfo[i].fieldWidth, EditorGUIUtility.singleLineHeight)
                    , element.FindPropertyRelative(elementInfo[i].elementPropertyPath)
                    , GUIContent.none);
                
            }
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + (limitColumn == 0 ? 2 * elementInfo.Length : 2 * elementInfo.Length / limitColumn) * EditorGUIUtility.singleLineHeight, rect.width, 1), Color.black);
        }
        
        void drawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            if (elementInfo is null || elementInfo.Length == 0) 
            {
                drawSingleElementRect(rect, element, index);
            }
            else
            {
                drawMultiElementHeader(rect, element,index);

                drawElementRect(rect, element);
            }
        }
        
        // 항목별 높이 레이아웃
        reorderableList.elementHeight = limitColumn == 0
            ? EditorGUIUtility.singleLineHeight * 1.25f
            : EditorGUIUtility.singleLineHeight * 2.5f * elementInfo.Length / limitColumn;

        reorderableList.drawHeaderCallback = drawHead;
        reorderableList.drawElementCallback = drawElement;
    }

    public static void ApplyReorderLayoutList(this ReorderableList reorderableList)
    {
        reorderableList.multiSelect = true;
        reorderableList.showDefaultBackground = true;
        reorderableList.DoLayoutList();
    }
    #endregion
    
    # region  Slider

    public static float DrawSlider(string label, float value, Vector2 range, int space)
    {
        EditorGUILayout.Space(space);
        float ret = EditorGUILayout.Slider(label, value, range.x, range.y);
        EditorGUILayout.Space(space);
        return ret;
    }
    
    #endregion
    
    # region DropDown
    public static (int selectedInex, string selectedValue) DrawHorizontalDropdown(this SerializedObject serializedObject, string path,string label, int index, int space = 10)
    {
        List<string> listContents = new List<string>(64);
        SerializedProperty list = serializedObject.FindProperty(path);
        for (int i = 0; i < list.arraySize; i++)
        {
            listContents.Add(list.GetArrayElementAtIndex(i).stringValue);
        }
        EditorGUILayout.Space(space);
        EditorGUILayout.BeginHorizontal();
        int ret = EditorGUILayout.Popup(label,index, listContents.ToArray());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(space);

        return (ret, serializedObject.FindProperty(path).GetArrayElementAtIndex(index).stringValue);
    }
    public static (int selectedInex, string selectedValue) DrawVerticalDropdown(this SerializedObject serializedObject, string path ,string label,int index, int space = 10)
    {
        List<string> listContents = new List<string>(64);
        SerializedProperty list = serializedObject.FindProperty(path);
        for (int i = 0; i < list.arraySize; i++)
        {
            listContents.Add(list.GetArrayElementAtIndex(i).stringValue);
        }
        EditorGUILayout.Space(space);
        EditorGUILayout.BeginVertical();
        int ret = EditorGUILayout.Popup(label,index, listContents.ToArray());
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(space);
        
        return (ret, serializedObject.FindProperty(path).GetArrayElementAtIndex(index).stringValue);
        
    }
    #endregion
    
    # region Button
    public static void DrawVerticalStretchWidthButton(string name, Action action, int height = 24 , int space = 20)
    {
        EditorGUILayout.Space(space);
        Rect source = EditorGUILayout.BeginVertical();
        if (GUI.Button(source, name, new GUIStyle(GUI.skin.button){ stretchWidth = true, fixedHeight = height, alignment = TextAnchor.MiddleCenter}))
        {
            action?.Invoke();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(space);
    }
    public static void DrawVerticalButton(string name, Action action, int width = 72, int height = 24 , int space = 20)
    {
        EditorGUILayout.Space(space);
        Rect source = EditorGUILayout.BeginVertical();
        if (GUI.Button(source, name, new GUIStyle(GUI.skin.button){ fixedWidth = width, fixedHeight = height, alignment = TextAnchor.MiddleCenter}))
        {
            action?.Invoke();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(space);
    }
    public static void DrawHorizonStretchWidthButton(string name, Action action, int height = 24 , int space = 20)
    {
        EditorGUILayout.Space(space);
        Rect source = EditorGUILayout.BeginHorizontal();
        if (GUI.Button(source, name, new GUIStyle(GUI.skin.button){ stretchWidth = true, fixedHeight = height, alignment = TextAnchor.MiddleCenter}))
        {
            action?.Invoke();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(space);
    }
    public static void DrawHorizonButton(string name, Action action, int width = 72, int height = 24 , int space = 20)
    {
        EditorGUILayout.Space(space);
        Rect source = EditorGUILayout.BeginHorizontal();
        if (GUI.Button(source, name, new GUIStyle(GUI.skin.button){ fixedWidth = width, fixedHeight = height, alignment = TextAnchor.MiddleCenter}))
        {
            action?.Invoke();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(space);
    }
    #endregion
    
    #region Draw Explains
    public static void DrawExplainBox(string title , string[] explains, string titleColor, int boxHeight = 169, int titleFontSize = 14, int contentFontSize = 11, int spaceTop = 20, int spaceBottom = 20)
    {
        EditorGUILayout.Space(spaceTop);
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box){richText = true, stretchWidth = true, fixedHeight = boxHeight});
        DrawTitle(title, titleFontSize , titleColor);
        EditorGUI.indentLevel++;
        if(explains is not null && explains.Length != 0)
        {
            for (int i = 0; i < explains.Length; i++)
            {
                DrawExplain(explains[i], contentFontSize);
            }   
        }
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel-= 2;
        EditorGUILayout.Space(spaceBottom);
    }
    public static void DrawTitle(string title, int fontSize, string color, int space = 10)
    {
        EditorGUILayout.Space(space);
        EditorGUILayout.LabelField($"<color={color}>{title}</color>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = fontSize });
    }
    public static string DrawSubTitle(string title, string color)
    {
        return $"<color={color}>{title}</color>";
    }

    public static void DrawExplain(string content, int fontSize, int boxSize = 48, int space = 15)
    {
        EditorGUILayout.LabelField(content, new GUIStyle(GUI.skin.label) { richText = true, fontSize = fontSize , fixedHeight = boxSize});
        EditorGUILayout.Space(space);
    }

    #endregion
}