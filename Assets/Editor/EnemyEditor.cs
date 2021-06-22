using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    Editor valueEditor;

    private void OnEnable()
    {
        valueEditor = CreateEditor(serializedObject.FindProperty("stats").objectReferenceValue, typeof(ItemParameterContainerEditor));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();
        valueEditor = CreateEditor(serializedObject.FindProperty("stats").objectReferenceValue, typeof(ItemParameterContainerEditor));
        if (valueEditor != null)
        {
            valueEditor.OnInspectorGUI();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
