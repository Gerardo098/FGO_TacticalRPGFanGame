using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect_HealingSpell is a subclass of the AbilityEffect script 
/// used by the "Healing Spell" triggered effect.
/// A unit that gains this effect is immediately healed a random amount from a given range
/// </summary>
[CreateAssetMenu(menuName = "Ability/Triggered Effect/Spell - Healing")]
public class Effect_HealingSpell : AbilityEffect
{
    public override void InternalInit()
    {
        effect = EffectClass.TriggeredEffect;
        duration = StatusDuration.Instant;
    }

    public override void RankRead() { return; }
}