using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * The ItemEditor serves to alter the GUI of Items to make them easier to read and work with.
 * In specific, we turn the list of parameters horizonally and pair them up with their ints.
 */
[CustomEditor(typeof(Servant))]
public class ServantEditor : Editor
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
