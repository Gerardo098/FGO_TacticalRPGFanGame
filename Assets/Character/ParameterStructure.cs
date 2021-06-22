using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Parameter Structure is a scriptable object that holds a list of parameters.
/// We use this to serve as a basis for what stats a certain unit will have.
/// </summary>
[CreateAssetMenu(menuName = "Parameter/Structure")]
public class ParameterStructure : ScriptableObject
{
    public List<Parameter> parameters;
}
