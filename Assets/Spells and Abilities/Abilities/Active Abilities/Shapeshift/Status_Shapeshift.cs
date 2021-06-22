using UnityEngine;

/// <summary>
/// Status_Shapeshift is a subclass of the AbilityEffect script used by the "Shapeshift" status effect.
/// Shapeshift increases the user's armour and evasion until their next turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Status Effect/Shapeshift")]
public class Status_Shapeshift : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.StartNextTurn;
        stackable = false; // Unstackable
    }

    // RankRead() checks the effect's bonus based on its current rank
    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                bonus = 1;
                return;
            case Rank.D:
                bonus = 2;
                return;
            case Rank.C:
                bonus = 3;
                return;
            case Rank.B:
                bonus = 4;
                return;
            case Rank.A:
                bonus = 5;
                return;
            case Rank.EX:
                bonus = 6;
                return;
            default:
                bonus = 0;
                return;
        }
    }
}