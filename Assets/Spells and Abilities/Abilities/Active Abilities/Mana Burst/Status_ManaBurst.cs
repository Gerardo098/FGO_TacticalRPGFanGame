using UnityEngine;

/// <summary>
/// Status_ManaBurst is a subclass of the AbilityEffect script used by the "Mana Burst" status effect.
/// Mana Burst increases the user's strength by a set amount for that turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Status Effect/Mana Burst")]
public class Status_ManaBurst : AbilityEffect
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
