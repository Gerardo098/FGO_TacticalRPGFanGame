using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Active Ability/Stalking")]
public class SkillStalking : Ability
{
    [SerializeField]
    private Status_Stalking stalkingStatus;

    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        RankRead(); // Get the correct MP cost for the skill
        if (target != null)
        {
            stalkingStatus.SetOgRank(GetCurrentRank()); // Set the ability's original rank = our skill's current rank
            CharacterManager targetMan = target.GetComponent<CharacterManager>();
            if (targetMan == null) { return; }
            EffectInstance instance = new EffectInstance(stalkingStatus, targetMan, charMan);
            targetMan.AbilityInstanceRead(instance);
        }
    }

    public override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public override void NPEffect(NPInstance NP)
    {
        throw new System.NotImplementedException();
    }

    /*
     * A messy way of checking the cost of an ability based on its rank
     */
    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                MPCost = 2;
                break;
            case Rank.D:
                MPCost = 4;
                break;
            case Rank.C:
                MPCost = 6;
                break;
            case Rank.B:
                MPCost = 8;
                break;
            case Rank.A:
            case Rank.EX:
                MPCost = 10;
                break;
            default:
                MPCost = 0;
                break;
        }
    }
}