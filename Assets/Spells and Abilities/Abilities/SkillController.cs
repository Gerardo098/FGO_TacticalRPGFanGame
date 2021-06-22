using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Skill controller is a script that houses all of a unit's abilities, whether they're spells,
 * active and passive skills, general abilities, etc. 
 */
public class SkillController : MonoBehaviour
{
    public CharacterManager charMan; // Character manager link
    // Spell schools available to the unit
    public List<SpellSchool> schools;
    // Abilities + heroic skills available to the unit
    public List<Ability> activeSkills;
    public List<Ability> activeNPs;

    /// <summary>
    /// 
    ///  The GetX() functions find and return a specific ability when given an int to serve as index
    ///  
    /// </summary>
    /// <param name="id"> Index used to find the ability </param>
    /// <returns> Functions return the ability in question </returns>
    public Ability GetActiveAbility(int id) { return activeSkills[id]; }
    public Ability GetActiveNP(int id) { return activeNPs[id]; }

    /// <summary>
    /// 
    /// SpendMana() handles and limits a unit's magic points (MP) spenditure.
    /// 
    /// </summary>
    /// <param name="spentMP"> Amount of MP to be spent </param>
    /// <returns> Flag on whether the unit can legally spend the MP or not </returns>
    public bool SpendMana(int spentMP)
    {
        // Find the difference between the character's current MP score and the amount they've spent.
        int MPCost = charMan.character.currentMP - spentMP;

        // If a character does not have enough MP in their MP pool to fully pay for the cost,
        // they take the difference in damage.
        if (MPCost < 0)
        {
            // First, check if the unit has enough HP to survive casting their ability
            // If true, the unit can spend the HP without KO'ing themselves
            if (charMan.character.MPSpendCheck(spentMP))
            {
                charMan.character.ReduceHealth(MPCost * -1);
                charMan.character.ReduceMP(charMan.character.currentMP);
                
                return true;
            }
            // If the unit would kill themselves this way, prevent the activation by returning false.
            else { return false; }

        }

        // Else, if the character would not have unspent MP, the cost is simply removed
        // from their current MP score as normal + return true
        else
        {
            charMan.character.currentMP = MPCost;
            return true;
        }
    }

    /// <summary>
    /// 
    /// GrabActiveAbilities() takes a Servant, usually upon equipping them, and hands the unit their active abilites
    /// 
    /// </summary>
    /// <param name="servant"> Servant in question </param>
    internal void GrabActiveAbilities(Servant servant)
    {
        // Clear the lists or initialize them
        CleanAbilities();

        // Iterate through the servant's active abilities, initialize them, and add them to our list of abilities
        foreach (Ability ability in servant.ReturnAbilities()) // Grab the abilities
        {
            ability.Init();
            activeSkills.Add(ability);
        }
        // We do the same for the servant's active NPs as well
        foreach (Ability NP in servant.ReturnNP())
        {
            NP.Init();
            activeNPs.Add(NP);
        }

        schools = servant.ReturnSpellSchools(); // Grab the spells
        // Set the # of uses of each skill to 0; the Servant was just equipped afterall
        ResetActiveUses();
    }

    /// <summary>
    /// 
    ///  CleanAbilities() empties the lists for both active skills and spell schools
    ///  when called. However, we first check if those lists are initialized (not null)
    ///  to avoid any null exception errors
    ///  
    /// </summary>
    internal void CleanAbilities()
    {
        // Active skills and NPs
        if (activeSkills != null) { activeSkills.Clear(); }
        if (activeNPs != null) { activeNPs.Clear(); }

        // Spell schools
        if (schools != null) { schools.Clear(); }
    }

    /// <summary>
    /// 
    ///  InitLists() is a simple function that creates new lists IF the
    ///  list is null; ignoring it otherwise to avoid overwriting.
    ///  
    /// </summary>
    internal void InitLists()
    {
        if (activeSkills == null) { activeSkills = new List<Ability>(); }
        if (activeNPs == null) { activeNPs = new List<Ability>(); }
        if (schools == null) { schools = new List<SpellSchool>(); }
    }

    /// <summary>
    /// 
    /// ResetActiveUses() takes each active ability and resets the # of times it has been used
    /// at the start of each of a unit's turn. This allows the unit to use them again.
    /// 
    /// </summary>
    public void ResetActiveUses()
    {
        foreach (Ability ability in activeSkills) { ability.ResetUses(); }
        foreach (Ability NP in activeNPs) { NP.ResetUses(); }
    }
}
