using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Passive_MadEnhancement is a subclass of the AbilityEffect script 
/// used by the "Mad Enhancement" passive ability.
/// A unit possessing Mad Enhancement receives a bonus to MP Generation and damage, but also suffers
/// additional damage from enemy attacks.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Passive Effect/Passive Ability/Mad Enhancement")]
public class Passive_MadEnhancement : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.PassiveAbility;
        duration = StatusDuration.Infinite;
        stackable = false; // Unstackable
    }

    // RankRead() checks the effect's bonus based on its current rank
    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                bonus = 1;
                break;
            case Rank.D:
                bonus = 2;
                break;
            case Rank.C:
                bonus = 3;
                break;
            case Rank.B:
                bonus = 4;
                break;
            case Rank.A:
                bonus = 5;
                break;
            case Rank.EX:
                bonus = 6;
                break;
            default:
                bonus = 0;
                break;
        }
    }
}