using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Formula/Movement")]
public class Formula_Movement : Formula_Root
{
    // Movement = Strength + 5 + Bonuses/Penalties
    public Parameter strength;
    int STR;

    public override int Calculate(ParameterContainer parameters)
    {
        parameters.Get(strength, out STR);
        return STR + 5;
    }

    public override List<Parameter> GetReferences()
    {
        List<Parameter> paramValues = new List<Parameter>();
        paramValues.Add(strength);

        return paramValues;
    }
}
