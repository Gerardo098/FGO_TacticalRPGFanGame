using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passive_MagicResistance is a subclass of the AbilityEffect script 
/// used by the "Magic Resistance" passive ability.
/// A unit possessing this ability is immune to spells of Rank equal to or lower than
/// this Skill's own.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Passive Ability/Magic Resistance")]
public class Passive_MagicResistance : AbilityEffect
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
