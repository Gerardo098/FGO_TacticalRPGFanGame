using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spell_Heal is a script that handles the Heal spell.
/// Upon being cast, the target receives the "Healing Spell" triggered effect
/// </summary>
[CreateAssetMenu(menuName = "Spell/Healing Spell/Heal")]
public class Spell_Heal : HealingSpell
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

    // RankRead() gets us the spell's range and MP cost
    public override void RankRead()
    {
        range = 5f;

        switch (rank)
        {
            case Rank.E:
                MPCost = 1;
                break;
            case Rank.D:
                MPCost = 2;
                break;
            case Rank.C:
                MPCost = 3;
                break;
            case Rank.B:
                MPCost = 4;
                break;
            case Rank.A:
                MPCost = 5;
                break;
            case Rank.EX:
                MPCost = 5;
                break;
            default: // Somehow have no rank? All equals 0
                MPCost = 0;
                break;
        }
    }
}
