using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbilityUseAction is the root abstract class for the player and NPC ability activation scripts.
/// As both player and NPC subclasses are too different, this script contains the bare minimum
/// that is shared between the two.
/// </summary>
public abstract class AbilityUseAction : MonoBehaviour
{
    public CharacterManager charMan; // Character reference
    protected Character_Root currentTarget; // Unit we're targeting.
    protected Ability ability; // Ability we're activating.

    /// <summary>
    /// SetAbility() receives an active ability and saves it prior to
    /// activation.
    /// </summary>
    /// <param name="_ability"> ability to be saved </param>
    public void SetAbility(Ability _ability) { ability = _ability; }

    /// <summary>
    /// AbilityActionCount() checks for the amount of actions an ability requires
    /// after it's been used.
    /// </summary>
    internal void AbilityActionCount() 
    {
        // Find the ActionType flag
        switch (ability.ReturnActionType())
        {
            // If it's a single action, just charge the unit 1 action
            case ActionType.Single:
                charMan.character.ReduceActionCount();
                break;
            // If it's a full action, spend all of a character's actions
            case ActionType.Full:
                charMan.character.FullAction();
                break;
            default: // On default, just do nothing (free action)
                break;
        }
        
    }
}
