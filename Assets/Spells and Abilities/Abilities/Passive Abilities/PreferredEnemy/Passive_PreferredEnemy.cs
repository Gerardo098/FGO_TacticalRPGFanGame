using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passive_PreferredEnemy is a subclass of the AbilityEffect script 
/// used by the "Preferred Enemy" passive ability.
/// A unit or weapon with this ability recieves advantage to attack rolls against
/// all targets with a specifed tag(s)
/// </summary>
[CreateAssetMenu(menuName = "Ability/Passive Ability/Preferred Enemy")]
public class Passive_PreferredEnemy : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.PassiveAbility;
        duration = StatusDuration.Infinite;
        stackable = false; // Unstackable
    }

    // No need for RankRead()
    public override void RankRead() { return; }
}
