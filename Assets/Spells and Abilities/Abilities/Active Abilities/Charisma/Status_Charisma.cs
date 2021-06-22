using UnityEngine;

/// <summary>
/// Status_Charisma is a subclass of the AbilityEffect script used by the "Charisma" status effect.
/// Charisma grants a status effect to all friendly units within 5m of the user that grants a 
/// boost to their next roll.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Status Effect/Charisma")]
public class Status_Charisma : AbilityEffect
{
    public override void InternalInit()
    {
        // Effect type and duration
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.StartNextTurn;
        stackable = false; // Unstackable
    }

    // RankRead() simply checks the effect's bonus based on its current rank
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