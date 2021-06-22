using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit under the effect of Stalking suffers a penalty to their EVA based on the
/// effect's rank
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Stalking/On Attack (Target)")]
public class OnAttackTarget_Stalking : Trigger_OnAttack_Target
{
    int EVApenalty;

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null OR is not of type AttackInstance, return false
        if (instance == null) { return false; }
        // If the source of the instance is not equal to the source of this effect, return
        if (instance.ReturnSource() != effect.GetSource()) { return false; }

        // Else, check the instance type
        if (instance.GetType() == typeof(AttackInstance))
        {
            ApplyEffect(effect, (AttackInstance)instance);
            return true; // return
        }
        if (instance.GetType() == typeof(SpellInstance))
        {
            ApplyEffect(effect, (SpellInstance)instance);
            return true; // return
        }

        // Nothing happened, return false;
        return false;
    }

    private void ApplyEffect(AbilityEffect effect, SpellInstance instance)
    {
        // The penalty has to be negative
        EVApenalty = effect.GetBonus() * -1;
        instance.SetEVABonus(EVApenalty);
    }

    private void ApplyEffect(AbilityEffect effect, AttackInstance instance)
    {
        // The penalty has to be negative
        EVApenalty = effect.GetBonus() * -1;
        instance.SetEVABonus(EVApenalty);
    }

    // Unused override actions
    public override void RankRead(Rank rank) { return; }
}
