using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this project, Parameters are the individual stats that units have.
/// Ex. Strength, Endurance, Agility, etc.
/// These are created with scriptable objects to easily move them around.
/// </summary>

[CreateAssetMenu(menuName = "Parameter/Parameter")]
public class Parameter : ScriptableObject
{
    // Name of the parameter in question
    public string parameterName;
    // Formula associated with this parameter
    public Formula_Root formula;
}
