using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillCharisma is a subclass of the ActiveAbility script used by the "Charisma" heroic skill.
/// Charisma grants a status effect to all friendly units within 5m of the user that grants a 
/// boost to their next roll.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Active Ability/Charisma")]
public class SkillCharisma : Ability
{
    [SerializeField]
    private LayerMask layer; // Layer flag
    [SerializeField]
    private Status_Charisma charismaStatus; // Status effect reference

    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        RankRead(); // Get the correct MP cost and bonus for the skill
        charismaStatus.SetOgRank(GetCurrentRank());
        // Check for all units within range - filtering with the LayerMask layer variable
        Collider[] foundColliders = Physics.OverlapSphere(charMan.transform.position, range, layer);

        // Look through each collider we've found
        foreach (Collider collider in foundColliders)
        {
            // Grab the collider's CharMananger script
            CharacterManager unit = collider.gameObject.GetComponent<CharacterManager>();
            // If it's not OUR CharMan script and the unit is an ally
            if (unit != charMan && charMan.character.CompareTeams(unit))
            {
                // Create an instance of the charisma effect and hand it to the unit
                EffectInstance instance = new EffectInstance(charismaStatus, unit, charMan);
                unit.AbilityInstanceRead(instance);
            }
        }
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