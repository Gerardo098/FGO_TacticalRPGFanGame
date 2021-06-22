using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AltElementalAttack is a script that handles all non-standard 
/// single-target attacks found in each of the elemental spell books.
/// This represents spells that focus more on effects than damage.
/// This spell is very flexible and can be used to create multiple different spells
/// of different damage types and effects.
/// </summary>
[CreateAssetMenu(menuName = "Spell/Attack Spell/Alternative Elemental Attack")]
public class AltElementalAttack : Spell
{
    public override void Activate(CharacterManager source, List<CharacterManager> targetList)
    {
        RankRead(); // Update to the current Rank
        // Check if the casting unit has enough to cast the spell
        if (source.skillController.SpendMana(MPCost))
        {
            // If so, create a new spell instance and hand it to the target
            SpellInstance instance = new SpellInstance(source, targetList[0], this);
            targetList[0].AbilityInstanceRead(instance);
        }
        else { Debug.Log("Failed to cast the spell..."); }
    }

    // RankRead() gets us the spell's range, MP cost, and damage die
    public override void RankRead()
    {
        range = 10f;

        switch (rank)
        {
            case Rank.E:
                MPCost = 1;
                dieSize = 4;
                dieAmount = 1;
                break;
            case Rank.D:
                MPCost = 2;
                dieSize = 6;
                dieAmount = 1;
                break;
            case Rank.C:
                MPCost = 3;
                dieSize = 8;
                dieAmount = 1;
                break;
            case Rank.B:
                MPCost = 4;
                dieSize = 10;
                dieAmount = 1;
                break;
            case Rank.A:
                MPCost = 5;
                dieSize = 6;
                dieAmount = 2;
                break;
            case Rank.EX:
                MPCost = 5;
                dieSize = 8;
                dieAmount = 2;
                break;
            default: // Somehow have no rank? All equals 0
                MPCost = 0;
                dieSize = 0;
                dieAmount = 0;
                break;
        }
    }
}
