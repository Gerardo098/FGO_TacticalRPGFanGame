using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Formula_Root is an abtsract class that calculates the value of a parameter based on
/// the Parameters it is dependent on.
/// </summary>
public abstract class Formula_Root : ScriptableObject
{
    // Return list of dependent parameters
    public abstract List<Parameter> GetReferences();

    /// <summary>
    /// Calculate() will handle the calculation in each formula script by grabbing the 
    /// given parameters and performing all calculations in the inheritor script.
    /// </summary>
    /// <param name="parameters"> container of the parameters used by the formula </param>
    /// <returns> int value for the parameter </returns>
    public abstract int Calculate(ParameterContainer parameters);
}
