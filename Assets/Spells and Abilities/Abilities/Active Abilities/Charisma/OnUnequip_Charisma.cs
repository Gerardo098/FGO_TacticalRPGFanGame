using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Charisma/On Unequip")]
public class OnUnequip_Charisma : Trigger_OnUnequip
{
    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        // Set the unit's charisma flag to false
        unit.unitEffects.charisma = false;
        // return
        return true;
    }

    // Unused functions
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        throw new System.NotImplementedException();
    }

    public override void RankRead(Rank rank)
    {
        throw new System.NotImplementedException();
    }
}
