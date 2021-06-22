using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/*
 * The ItemParameterContainerEditor serves to alter the GUI of ItemParameterContainers to make them easier
 * to use - in specific, we turn the list of parameters horizonally and pair them up with their ints.
 */

[CustomEditor(typeof(ItemParameterContainer))]
public class ItemParameterContainerEditor : Editor
{
    int index = 0; 

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Show(serializedObject.FindProperty("parameters"));
        serializedObject.ApplyModifiedProperties();
    }

    private void Show(SerializedProperty list)
    {
        index = 0;
        EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));

        for (int i = 0; i < list.arraySize; i++)
        {
            // Match every field-value pair horizontally
            EditorGUILayout.BeginHorizontal();

            SerializedProperty sp = list.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(sp, GUIContent.none);
            ShowState(i, sp);

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ShowState(int i, SerializedProperty sp)
    {
        if (sp.objectReferenceValue == null) { return; }
        ShowValueField("integers", index);
        index += 1;
    }

    private void ShowValueField(string valueArray, int index)
    {
        SerializedProperty list = serializedObject.FindProperty(valueArray);
        if (index >= list.arraySize)
        {
            list.arraySize += 1;
        }
        SerializedProperty arrayElement = list.GetArrayElementAtIndex(index);
        if(arrayElement != null)
        {
            EditorGUILayout.PropertyField(arrayElement, GUIContent.none);
        }
    }
}
