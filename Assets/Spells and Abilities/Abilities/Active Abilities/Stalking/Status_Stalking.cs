using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Status Effect/Stalking")]
public class Status_Stalking : AbilityEffect
{
    public override void InternalInit()
    {
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.SourceNextTurn;
        DCFlag = false;
        stackable = false;
    }

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