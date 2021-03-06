using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillInstinct is a subclass of the ActiveAbility script used by the "Instinct" heroic skill.
/// Instinct increases the user's evasion and critical threat for a turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Active Ability/Instinct")]
public class SkillInstinct : Ability
{
    [SerializeField]
    private Status_Instinct instinctStatus; // Status effect reference

    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        RankRead(); // Get the correct MP cost for the skill
        instinctStatus.SetOgRank(GetCurrentRank());
        EffectInstance instance = new EffectInstance(instinctStatus, charMan, charMan);
        charMan.AbilityInstanceRead(instance);
    }

    // RankRead() simply checks the skill's MP cost based on its current rank
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
            default: // Somehow have no rank? MP cost is 0
                MPCost = 0;
                break;
        }
    }

    // Unused functions
    public override void Deactivate() { return; }
    public override void NPEffect(NPInstance NP) { return; }
}