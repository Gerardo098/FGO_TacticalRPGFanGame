using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicElementalAOE is a script that handles all standard area-of-effect 
/// attacks found in each of the elemental spell books.
/// This spell is very flexible and can be used to create multiple different spells
/// of different damage types and effects.
/// </summary>
[CreateAssetMenu(menuName = "Spell/Attack Spell/Basic Elemental AoE")]
public class BasicElementalAOE : Spell
{
    public override void Activate(CharacterManager source, List<CharacterManager> targetList)
    {
        RankRead(); // Update to the current Rank
        // Check if the casting unit has enough to cast the spell
        if (source.skillController.SpendMana(MPCost))
        {
            foreach (CharacterManager unit in targetList)
            {
                // If so, create a new spell instance and hand it to the target
                SpellInstance instance = new SpellInstance(source, unit, this);
                unit.AbilityInstanceRead(instance);
            }
        }
        else { Debug.Log("Failed to cast the spell..."); }
    }

    // RankRead() gets us the spell's range, aoeRange, MP cost, and damage die
    public override void RankRead()
    {
        range = 10f;
        aoeRange = 3f;

        switch (rank)
        {
            case Rank.E:
                MPCost = 2;
                DC = 11;
                dieSize = 8;
                dieAmount = 1;
                break;
            case Rank.D:
                MPCost = 4;
                DC = 12;
                dieSize = 10;
                dieAmount = 1;
                break;
            case Rank.C:
                MPCost = 6;
                DC = 13;
                dieSize = 6;
                dieAmount = 2;
                break;
            case Rank.B:
                MPCost = 8;
                DC = 14;
                dieSize = 8;
                dieAmount = 2;
                break;
            case Rank.A:
                MPCost = 10;
                DC = 15;
                dieSize = 6;
                dieAmount = 3;
                break;
            case Rank.EX:
                MPCost = 10;
                DC = 16;
                dieSize = 8;
                dieAmount = 3;
                break;
            default: // Somehow have no rank? All equals 0
                MPCost = 0;
                DC = 0;
                dieSize = 0;
                dieAmount = 0;
                break;
        }
    }
}
