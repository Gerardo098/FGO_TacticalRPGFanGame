using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Status Effect/Burning")]
public class Status_Burned : AbilityEffect
{
    [SerializeField]
    private Parameter END;

    public override void InternalInit()
    {
        effect = EffectClass.StatusEffect;
        duration = StatusDuration.TurnCount;
        DCFlag = true;
        parameter = END;
        turnCount = Random.Range(2, 4);
        stackable = false;
    }

    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                DC = 11;
                dieSize = 4;
                dieAmount = 1;
                break;
            case Rank.D:
                DC = 12;
                dieSize = 6;
                dieAmount = 1;
                break;
            case Rank.C:
                DC = 13;
                dieSize = 8;
                dieAmount = 1;
                break;
            case Rank.B:
                DC = 14;
                dieSize = 10;
                dieAmount = 1;
                break;
            case Rank.A:
                DC = 15;
                dieSize = 12;
                dieAmount = 1;
                break;
            case Rank.EX:
                DC = 16;
                dieSize = 6;
                dieAmount = 2;
                break;
            default:
                break;
        }
    }
}
