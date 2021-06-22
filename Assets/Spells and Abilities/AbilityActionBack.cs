using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbilityActionBack is a script used by the back button in both the skill and NP activation
/// modes allowing the user to return to the panels if they change their mind.
/// </summary>
public class AbilityActionBack : MonoBehaviour
{
    public CharacterManager character; // Character using the turn panel
    public GameObject buttonPanel; // Button panel reference
    public GameObject abilityPanel; // Ability panel reference

    public void DeactivateAbilityUse()
    {
        // Turn the CastingAction off while turning the menu back on
        character.abilityUse.enabled = false;
        // Set the panels to active
        buttonPanel.SetActive(true);
        abilityPanel.SetActive(true);
        // Button sets itself as inactive
        gameObject.SetActive(false);
    }
}
