using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CastActionBack class is used by the back button used by the casting action
/// to allow us to deactivate it and return to the spell book panel.
/// </summary>
public class CastActionBack : MonoBehaviour
{
    public CharacterManager character; // Character using the class
    public GameObject buttonPanel; // reference to the turn panel
    public GameObject bookPanel; // reference to the book panel

    /// <summary>
    /// DeactivateCasting() is used by a button to turn itself inactive, set the button panel
    /// and book panel actice, and disable the character's casting action.
    /// </summary>
    public void DeactivateCasting()
    {
        // Turn the CastingAction off while turning the menu back on
        character.castingAction.enabled = false;
        buttonPanel.SetActive(true);
        bookPanel.SetActive(true);
        gameObject.SetActive(false); // Sets itself as inactive
    }
}
