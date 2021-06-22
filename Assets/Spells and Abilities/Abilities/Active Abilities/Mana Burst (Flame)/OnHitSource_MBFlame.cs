using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mana Burst Flame increases the user's armour against fire damage 
/// and increases their damage by a set amount for that turn.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Mana Burst (Flame)/On Hit (Source)")]
public class OnHitSource_MBFlame : Trigger_OnHit_Source
{
    int bonus; // Bonus ARM

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null, return false
        if (instance == null) { return false; }

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

        return true;
    }

    /// <summary>
    /// ApplyEffect() hands the ability instance the damage bonus
    /// </summary>
    /// <param name="effect"> AbilityEffect source of the bonus </param>
    /// <param name="instance"> Ability instance to be changed </param>
    private void ApplyEffect(AbilityEffect effect, SpellInstance instance)
    {
        // The penalty has to be negative
        bonus = effect.GetBonus();
        instance.SetDMGBonus(bonus);
    }

    /// <summary>
    /// ApplyEffect() hands the ability instance the damage bonus
    /// </summary>
    /// <param name="effect"> AbilityEffect source of the bonus </param>
    /// <param name="instance"> Ability instance to be changed </param>
    private void ApplyEffect(AbilityEffect effect, AttackInstance instance)
    {
        // The penalty has to be negative
        bonus = effect.GetBonus();
        instance.SetDMGBonus(bonus);
    }

    public override void RankRead(Rank rank) { return; }
}