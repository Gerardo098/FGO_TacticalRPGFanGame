using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Formula/Resistance")]
public class Formula_Resistance : Formula_Root
{
    // Resistance = Endurance + Bonuses/Penalties
    public Parameter endurance;
    int END;

    public override int Calculate(ParameterContainer parameters)
    {
        parameters.Get(endurance, out END);
        return END;
    }

    public override List<Parameter> GetReferences()
    {
        List<Parameter> paramValues = new List<Parameter>();
        paramValues.Add(endurance);

        return paramValues;
    }
}
