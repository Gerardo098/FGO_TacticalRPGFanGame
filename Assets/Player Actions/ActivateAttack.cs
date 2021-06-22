using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ActitvateAttack is a class that sets the button panel and back buttons active/inactive
/// and also enabling the player character's attack action script.
/// </summary>
public class ActivateAttack : MonoBehaviour
{
    // character trying to attack
    public GameObject character;
    // Action menu
    public GameObject buttonPanel;
    // Back button (takes us back to the action menu)
    public GameObject backButton;

    /// <summary>
    /// Activate() sets the button panel and back button active or inactive, one opposite of the
    /// other. In addition, we enable/disable the attackaction script
    /// </summary>
    public void Activate()
    {
        if (character.GetComponent<AttackAction>() != null)
        {
            // Deactivate the menu buttonPanel
            buttonPanel.SetActive(!buttonPanel.activeSelf);
            backButton.SetActive(!backButton.activeSelf);
            // Turn player's attack ON or OFF
            character.GetComponent<AttackAction>().enabled = !character.GetComponent<AttackAction>().enabled;
        }
    }
}
