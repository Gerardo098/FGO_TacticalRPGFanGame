using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillManaBurst is a subclass of the ActiveAbility script used by the "Mana Burst" heroic skill.
/// Mana Burst increases the user's strength by a set amount for that turn.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Active Ability/Mana Burst")]
public class SkillManaBurst : Ability
{
    [SerializeField]
    private Status_ManaBurst manaBurst; // Status effect reference
    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        RankRead(); // Get the correct MP cost for the skill
        manaBurst.SetOgRank(GetCurrentRank()); // Set the ability's original rank = our skill's current rank
        // Create the effect instance
        EffectInstance instance = new EffectInstance(manaBurst, charMan, charMan);
        // Send the charMan the new instance
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