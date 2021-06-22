using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Status_CursedBarbs is a subclass of the AbilityEffect script used by the "Cursed Barbs" status effect.
/// Cursed Barbs imposes disadvantage to a healing ability's heal effect for a turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Passive Effect/Status Effect/Cursed Barbs")]
public class Status_CursedBarbs : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.SourceNextTurn;
        stackable = false; // Unstackable
    }

    // RankRead() checks the effect's DC based on its current rank
    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                DC = 10;
                return;
            case Rank.D:
                DC = 11;
                return;
            case Rank.C:
                DC = 12;
                return;
            case Rank.B:
                DC = 13;
                return;
            case Rank.A:
                DC = 14;
                return;
            case Rank.EX:
                DC = 15;
                return;
            default:
                DC = 0;
                return;
        }
    }
}
