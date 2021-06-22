using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Formula/Critical Threat")]
public class Formula_CriticalThreat : Formula_Root
{
    // Critical Threat = (Luck * 1/2) + Bonuses/Penalties
    public Parameter luck;
    int LCK;

    public override int Calculate(ParameterContainer parameters)
    {
        parameters.Get(luck, out LCK);
        // Round down to the nearest whole # or 0 with Mathf.FloorToInt
        return Mathf.FloorToInt(LCK * 0.5f);
    }

    public override List<Parameter> GetReferences()
    {
        List<Parameter> paramValues = new List<Parameter>();
        paramValues.Add(luck);

        return paramValues;
    }
}
