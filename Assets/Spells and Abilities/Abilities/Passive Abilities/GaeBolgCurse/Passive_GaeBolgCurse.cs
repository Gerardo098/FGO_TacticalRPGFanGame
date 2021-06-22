using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passive_GaeBolgCurse is a subclass of the AbilityEffect script 
/// used by the "Gae Bolg Curse" passive ability.
/// A unit damaged by a weapon possessing this effect must roll a saving throw against it or suffer
/// the Cursed Barbs status effect for a turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Passive Effect/Passive Ability/Gae Bolg Curse")]
public class Passive_GaeBolgCurse : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.PassiveAbility;
        duration = StatusDuration.Infinite;
        stackable = false; // Unstackable
    }

    // RankRead() checks the effect's DC based on its current rank
    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                DC = 10;
                break;
            case Rank.D:
                DC = 11;
                break;
            case Rank.C:
                DC = 12;
                break;
            case Rank.B:
                DC = 13;
                break;
            case Rank.A:
                DC = 14;
                break;
            case Rank.EX:
                DC = 15;
                break;
            default:
                DC = 0;
                break;
        }

        return;
    }
}
