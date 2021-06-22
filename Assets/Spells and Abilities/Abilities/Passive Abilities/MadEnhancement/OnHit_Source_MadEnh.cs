using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit possessing Mad Enhancement receives a bonus to MP Generation and damage, but also suffers
/// additional damage from enemy attacks.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Mad Enhancement/ On Hit (Source)")]
public class OnHit_Source_MadEnh : Trigger_OnHit_Source
{
    int bonus; // Bonus damage

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null OR is not of type AttackInstance, return false
        if (instance == null) { return false; }
        if (instance.GetType() != typeof(AttackInstance)) { return false; }
        AttackInstance attack = (AttackInstance)instance;

        // Read the current rank for the bonus, and send it to the instance
        RankRead(effect.GetCurrentRank());
        attack.damage += bonus;
        return true; // Return
    }

    public override void RankRead(Rank rank)
    {
        switch (rank)
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
