using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ActivateMovement is a class that handles opening and activating the
/// movement script for a player-controlled character
/// </summary>
public class ActivateMovement : MonoBehaviour
{
    // character we're going to move around
    public GameObject character;
    // Turn action menu
    public GameObject buttonPanel;
    // Back button (takes us back to the action menu)
    public GameObject backButton;

    /// <summary>
    /// On Activate(), we take the button panel and the back button 
    /// and turn them active and inactive (or vice versa) respectively; while
    /// one is active, the other must be inactive.
    /// And we also enable the character's movement script; this should be set to true
    /// while the backbutton is active and the panel inactive
    /// </summary>
    public void Activate()
    {
        if (character.GetComponent<Movement_Player>() != null)
        {
                // Deactivate the menu buttonPanel
                buttonPanel.SetActive(!buttonPanel.activeSelf);
                backButton.SetActive(!backButton.activeSelf);
                // Turn player movement ON or OFF
                character.GetComponent<Movement_Player>().enabled = !character.GetComponent<Movement_Player>().enabled;
        }
    }
}
