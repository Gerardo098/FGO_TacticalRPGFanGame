using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit possessing Mad Enhancement receives a bonus to MP Generation and damage, but also suffers
/// additional damage from enemy attacks.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Mad Enhancement/On Equip")]
public class OnEquip_MadEnh : Trigger_OnEquip
{
    int bonus; // Bonus MP gen

    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        // Upon equiping this passive ability, find the bonus
        RankRead(effect.GetCurrentRank());

        // Add the bonus to the unit's MP Generation stat
        Parameter MPGen = unit.MPGen;
        unit.character.parameterContainer.Sum(MPGen, bonus);

        return true;
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

    // UNUSED ==========================================================================
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null) { return false; }
}
