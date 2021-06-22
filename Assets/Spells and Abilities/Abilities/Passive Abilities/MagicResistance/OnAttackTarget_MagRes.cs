using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit possessing this ability is immune to spells of Rank equal to or lower than
/// this Skill's own.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Magic Resistance/On Attack (Target)")]
public class OnAttackTarget_MagRes : Trigger_OnAttack_Target
{
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null OR is not of type SpellInstance, return false
        if (instance == null) { return false; }
        if (instance.GetType() != typeof(SpellInstance)) { return false; }

        // Set the target's resistance rank equal to the effect's current rank
        (instance as SpellInstance).SetResistance(effect.GetCurrentRank());
        return true;
    }

    public override void RankRead(Rank rank)
    {
        throw new System.NotImplementedException();
    }
}
