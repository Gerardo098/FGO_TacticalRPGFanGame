using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Charisma/On Equip")]
public class OnEquip_Charisma : Trigger_OnEquip
{
    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        // Set the unit's charisma flag to true
        unit.unitEffects.charisma = true;
        // Return
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
