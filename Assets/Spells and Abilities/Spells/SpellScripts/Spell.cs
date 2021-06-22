using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spell is the abstract class that all spells in the game inherit from.
/// </summary>
public abstract class Spell : ScriptableObject
{
    // Name of the spell
    public string Name;
    // Available Rank - how strong the skill is
    public Rank rank;
    // Target type - what sort of spell it is
    public TargetType targetType;
    // Does the spell need to hit before its effects occur?
    public bool HitRoll;
    // tags for the spell - used by other effects
    public List<string> tags;
    // MP cost of the spell
    internal int MPCost;
    // How far a target/s can be from the spell
    internal float range;
    // The range for aoe spells, from the target outwards
    internal float aoeRange;
    // The difficulty check (DC) of the spell, used when it has other effects
    internal int DC;

    public bool damageDealing;
    public bool HasEffect;
    public AbilityEffect spellEffect;

    internal int dieSize; // Size of damage die
    internal int dieAmount; // Amount of damage die

    /*
     * GetName() is a simple function that simply returns the name + rank of the spell
     */
    public string GetName() { return Name + " [" + rank + "]"; }
    public int ReturnMP() { return MPCost; }
    public int ReturnDC() { return DC; }
    public void GetDamageDice(out int _number, out int _size)
    {
        RankRead(); // Get the correct numbers
        _number = dieAmount;
        _size = dieSize;
    }
    public float ReturnRange()
    {
        RankRead(); // Get the correct range
        return range; // Return it
    }
    public float ReturnAoE()
    {
        RankRead();
        return aoeRange;
    }

    /// <summary>
    /// Activate() is the function called the moment a spell is cast.
    /// </summary>
    /// <param name="source"> unit casting the spell </param>
    /// <param name="targetList"> list of charManager references of all legal targets </param>
    public abstract void Activate(CharacterManager source, List<CharacterManager> targetList);
    
    /// <summary>
    /// RankRead() takes the spell's current Rank and finds all parameters based
    /// on that rank.
    /// </summary>
    public abstract void RankRead();
}

/// <summary>
/// HealingSpell is a subclass of spell that adds a bit more functions and variables
/// used only by healing spells.
/// </summary>
public abstract class HealingSpell : Spell
{
    internal int HSize;
    internal int HAmount;

    // Get back how big the healing will be
    public void GetHealingDice(out int amount, out int size)
    {
        RankRead();
        amount = HAmount;
        size = HSize;
    }
}