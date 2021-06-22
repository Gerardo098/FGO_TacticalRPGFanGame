using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parameter/Container")]
public class ItemParameterContainer : ParameterStructure
{
    public int[] integers;

    internal void Form(ParameterStructure paramBase)
    {
        // If the parameter list is empty, initialize it
        if (parameters == null) { parameters = new List<Parameter>(); }
        // Go through all values in the parambase structure and add them to the list
        for (int i = 0; i < paramBase.parameters.Count; i++)
        {
            parameters.Add(paramBase.parameters[i]);
        }

        integers = new int[parameters.Count];
    }
}
