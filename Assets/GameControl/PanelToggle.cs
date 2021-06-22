using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PanelToggle is a simple script to handle opening and closing panels.
/// This will be the most commonly used panel opening script aside from any fringe cases 
/// that might show up. Will usually be attatched to a button.
/// </summary>
public class PanelToggle: MonoBehaviour
{
    public GameObject panel; // Panel 2 toggle

    /// <summary>
    /// TogglePanel sets a gameobject as active or inactive with the SetActive() 
    /// function and a ! to give us the opposite of what it currently is
    /// </summary>
    public void TogglePanel() { if (panel != null) { panel.SetActive(!panel.activeSelf); } }
}
