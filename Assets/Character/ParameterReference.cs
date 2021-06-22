using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ParameterReference is a "box" of sorts that stores a parameter, as well as their value,
/// calculation actions and other parameters they're dependent on.
/// </summary>
public class ParameterReference
{
    // Parameter stored in this reference
    public Parameter parameterBase;
    public int pValue; // Param's current value

    // Action control
    public Action onChange; // Main action function
    public Action<Parameter> recalculate; // Recalculation functions
    public List<Parameter> dependents; // Other params this reference is dependent on

    /// <summary>
    /// Creator function for ParameterReference
    /// </summary>
    /// <param name="parambase"> parameter to be saved by the reference </param>
    /// <param name="value"> value assigned to the parameter; default 0 </param>
    public ParameterReference(Parameter parambase, int value = 0)
    {
        parameterBase = parambase;
        pValue = value;
    }

    /// <summary>
    /// GetText() is a simple function that returns the parameter's name and its value together as a single string
    /// </summary>
    /// <returns> string of the param's name and value </returns>
    public string GetText() { return parameterBase.name + " " + pValue.ToString(); }

    /// <summary>
    /// Sum() receives an int, which is added to the value.
    /// Then, the onChange action is called, and we run the 
    /// RecalculateDependencies() function.
    /// </summary>
    /// <param name="sum"> amount to add to the value </param>
    internal void Sum(int sum)
    {
        pValue += sum;
        onChange?.Invoke();
        RecalculateDependencies();
    }

    /// <summary>
    /// Subtract() receives an int, which is removed from the value.
    /// Then, the onChange action is called, and we run the 
    /// RecalculateDependencies() function.
    /// The same as Sum() basically.
    /// </summary>
    /// <param name="sum"> amount to subtract from the value </param>
    internal void Subtract(int sum)
    {
        pValue -= sum;
        onChange?.Invoke();
        RecalculateDependencies();
    }

    /// <summary>
    /// Null() sets the value to 0.
    /// Then, like Sum() and Subtract(), run the the onChange action 
    /// and the RecalculateDependencies() function.
    /// </summary>
    public void Null()
    {
        pValue = 0;
        onChange?.Invoke();
        RecalculateDependencies();
    }

    /// <summary>
    /// In RecalculateDependencies(), we cycle through all the dependents
    /// saved in this reference's dependent list, if it's not null.
    /// If able, we call run each of their recalculate actions.
    /// </summary>
    internal void RecalculateDependencies()
    {
        if (dependents != null)
        {
            foreach (Parameter parameter in dependents)
            {
                recalculate?.Invoke(parameter);
            }
        }
    }

}
