using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UseAbility_Enemy is the AbilityUseAction subclass used by NPC units
/// </summary>
public class UseAbility_Enemy : AbilityUseAction
{
    // Active abilities and NPs available to the unit
    public List<Ability> abilities;
    public List<Ability> NoblePhantasms;

    /// <summary>
    /// UseAbility() activates an ability at a given index from the above list.
    /// ATM you're expected to know what abilities sit at what position in the list.
    /// </summary>
    /// <param name="index"> Index used to find the skill </param>
    /// <param name="target"> Unit targeted by the ability (if any) </param>
    internal void UseAbility(int index, CharacterManager target = null)
    {
        // Set the ability as the currently chosen ability
        // We check that the index is within bounds of the # of abilities in our list
        if (abilities.Count >= index) { SetAbility(abilities[index]); }
        // Check whether the ability is not null and we have the MP+HP to activate our ability
        if (ability != null && charMan.skillController.SpendMana(ability.GetMPCost()))
        {
            // Finally run the activate script based on whether target is null or not
            if (target == null) 
            { 
                ability.Activate(charMan);
                return;
            }
            else 
            { 
                ability.Activate(charMan, target.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// UseNoblePhantasm_Target() is a copy of the above UseAbility(), 
    /// except we use the NoblePhantasms list instead, and a target is required.
    /// </summary>
    /// <param name="index"> Index used to find the NP </param>
    /// <param name="target"> Unit targeted by the ability </param>
    internal void UseNoblePhantasm_Target(int index, CharacterManager target)
    {
        // Set the ability as the currently chosen ability
        // We check that the index is within bounds of the # of abilities in our list
        if (NoblePhantasms.Count >= index) { SetAbility(NoblePhantasms[index]); }

        // Check whether the ability is not null and we have the MP+HP to activate our NP
        if (ability != null && charMan.skillController.SpendMana(ability.GetMPCost()))
        {
            // We can activate it now
            ability.Activate(charMan, target.gameObject);
        }   
    }
}
