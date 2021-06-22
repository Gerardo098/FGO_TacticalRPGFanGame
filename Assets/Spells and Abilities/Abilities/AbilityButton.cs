using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// AbilityButton is the script that handles all active ability usage via button prefabs
/// on the skill and NP panels.
/// </summary>
public class AbilityButton : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// enum dictating the type of ability we're using the button for
    /// </summary>
    enum AbilityType { Heroic, NP }

    Text text; // Name of the ability
    AbilityType flag; // flag dictating type of ability
    AbilityPanel abilityPanel; // Panel for Heroic Skills
    NPPanel noblePanel; // Panel for Noble Phantasms

    /*
     * OnPointerClick calls the appropriate functions to activate 
     * the ability in question when the button is pressed.
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        // First, check the abilityType flag to call the correct functions from the correct panel
        // If we're working with a heroic Skill
        if (flag == AbilityType.Heroic)
        {
            // If our ability panel is null, find it
            if (abilityPanel == null) { abilityPanel = transform.parent.GetComponent<AbilityPanel>(); }
            abilityPanel.Use(transform.GetSiblingIndex()); // Call the panel's use function
        }
        // If we're handling a Noble Phantasm ability
        else
        {
            // If our NP panel is null, find it
            if (noblePanel == null) { noblePanel = transform.parent.GetComponent<NPPanel>(); }
            noblePanel.Use(transform.GetSiblingIndex()); // Call the panel's use function
        }

    }

    /// <summary>
    /// Set() receives an ability, grabbing its name, rank, and type for the button to save
    /// </summary>
    /// <param name="ability"> ability to be saved </param>
    /// <param name="NP"> Bool flag on whether the ability is an NP or a basic active skill </param>
    internal void Set(Ability ability, bool NP = false)
    {
        // Check if it's a heroic skill or NP
        if (NP == false) { flag = AbilityType.Heroic; } 
        else { flag = AbilityType.NP; }

        // Then we just grab the name of the ability 
        if (text == null) { text = transform.GetChild(0).GetComponent<Text>(); }
        text.text = ability.GetName();
    }
}
