using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Casting_Enemy is the CastingAction subclass used by NPC units.
/// </summary>
public class Casting_Enemy : CastingAction
{
    public List<Spell> spells; // All spells available to the unit
    public Rank casterRank; // How powerful the enemy is as a spellcaster

    /// <summary>
    /// EnemyCast_ST() is our basic casting ability.
    /// We receive a target and the spell we want to cast.
    /// </summary>
    /// <param name="_target"> target for the spell </param>
    /// <param name="spell"> spell to be cast </param>
    internal void EnemyCast_ST(CharacterManager _target, Spell spell)
    {
        // Turn towards the target
        LookAtTarget(gameObject.transform, _target.transform);
        
        targetList = new List<CharacterManager>(); // Generate a new list of targets
        targetList.Add(_target); // Add our target to it

        // Grab the spell and set its rank
        currentSpell = spell;
        currentSpell.rank = casterRank;

        StartCoroutine(Cast()); // Call the coroutine to cast our spell
    }

    /// <summary>
    /// EnemyCast_AOE() handles area-of-effect spells.
    /// This version receives a list of targets from the 
    /// unit's character script instead of just 1 target.
    /// </summary>
    /// <param name="_targets"> List of targets in range </param>
    /// <param name="spell"> spell to be cast </param>
    internal void EnemyCast_AOE(List<CharacterManager> _targets, Spell spell)
    {
        // Save the list reference
        targetList = _targets;

        // Turn towards the 1st target in the target list
        LookAtTarget(gameObject.transform, targetList[0].transform);

        // Grab the spell and set its rank
        currentSpell = spell;
        currentSpell.rank = casterRank;
        StartCoroutine(Cast()); // Call the coroutine to cast our spell
    }

    /// <summary>
    /// EnemyCast_NoTarget() handles spells that do not require a target to cast
    /// </summary>
    /// <param name="spell"> spell to be cast </param>
    internal void EnemyCast_NoTarget(Spell spell)
    {
        // Generate a new list of targets - we get a null error if we don't
        targetList = new List<CharacterManager>();

        // Grab the spell and set its rank
        currentSpell = spell;
        currentSpell.rank = casterRank;
        StartCoroutine(Cast()); // Call the coroutine to cast our spell
    }

    /// <summary>
    /// ReturnSpell() returns the spell located at a given index from the list of
    /// available spells.
    /// </summary>
    /// <param name="index"> index to find the spell in </param>
    /// <returns> Spell reference </returns>
    public Spell ReturnSpell(int index)
    {
        // If the list of spells is empty, return null
        if (spells == null) { return null; }
        // If the index is greater than the # of items in the list, return null
        else if (index > spells.Count) { return null; }
        return spells[index]; // Return the spell
    }

}
