using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TextCharacterValue() is used by the stat panel of all player-controlled units.
/// It keeps the panel up to date with one of the unit's parameters and their current value.
/// </summary>
public class TextCharacterValue : MonoBehaviour
{
    public Parameter trackValue; // Parameter to track
    public Character_Root character; // Character we're examining
    Text text; // Panel text

    /// <summary>
    /// UpdateText() changes the text of the attached game object to fit current values
    /// </summary>
    void UpdateText()
    {
        // Get the string from the tracked value
        string str = character.parameterContainer.GetText(trackValue);
        // If the panel text is not null, grab it, and update it
        if (text == null) { text = GetComponent<Text>(); }
        text.text = str;
    }


    /// <summary>
    /// Set() is basically this script's initiator function - when a text box with this script is created by 
    /// StatsPanel.cs, Set() is called to apply to the text box which character it will follow and what value 
    /// of theirs to track
    /// </summary>
    /// <param name="_trackValue"> Parameter to keep track of </param>
    /// <param name="_character"> Unit to follow </param>
    public void Set(Parameter _trackValue, Character_Root _character)
    {
        // Save the given parameters first
        character = _character;
        trackValue = _trackValue;

        // If the panel text is null, fill it ourselves.
        if (text == null)
        {
            text = GetComponent<Text>();
        }

        // Fill in the text with the tracked parameter's name and current value
        text.text = character.parameterContainer.GetText(trackValue);
        // In addition, save the UpdateText() function as an action for the tracked parameter
        character.parameterContainer.Subscribe(UpdateText, trackValue);
    }
}
