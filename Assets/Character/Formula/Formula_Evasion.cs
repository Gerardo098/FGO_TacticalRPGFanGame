using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Formula/Evasion")]
public class Formula_Evasion : Formula_Root
{
    // Evasion = 10 + Agility + Bonuses/Penalties
    public Parameter agility;
    int AGL;

    public override int Calculate(ParameterContainer parameters)
    {
        parameters.Get(agility, out AGL);
        return 10 + AGL;
    }

    public override List<Parameter> GetReferences()
    {
        List<Parameter> paramValues = new List<Parameter>();
        paramValues.Add(agility);
        return paramValues;
    }
}
