using UnityEngine;

/// <summary>
/// Status_lManaBurstFlame is a subclass of the AbilityEffect script 
/// used by the "Mana Burst (Flame)" heroic skill.
/// Mana Burst Flame increases the user's armour against fire damage 
/// and increases their damage by a set amount for that turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Status Effect/Mana Burst (Flame)")]
public class Status_ManaBurstFlame : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.StartNextTurn;
        DCFlag = false; // no DC roll
        stackable = false; // Unstackable
    }

    // RankRead() simply checks the effect's bonus based on its current rank
    public override void RankRead()
    {
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
}
