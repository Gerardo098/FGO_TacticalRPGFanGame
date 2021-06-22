using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Formula/MP Generation")]
public class Formula_MPGeneration : Formula_Root
{
    // MP Generation = Mana + Bonuses/Penalties
    public Parameter mana;
    int MAN;

    public override int Calculate(ParameterContainer parameters)
    {
        parameters.Get(mana, out MAN);
        return MAN;
    }

    public override List<Parameter> GetReferences()
    {
        List<Parameter> paramValues = new List<Parameter>();
        paramValues.Add(mana);

        return paramValues;
    }
}
