using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StatsPanel is used exclusively by the character stat panel, displaying all their 
/// stats and current values.
/// </summary>
public class StatsPanel : MonoBehaviour
{
    [SerializeField]
    ParameterStructure characterParameters;
    [SerializeField]
    GameObject text;
    [SerializeField]
    Character_Root character;

    /// <summary>
    /// In Start(), we look through the characterParameters and check out currently stored character.
    /// If the character has a stat from the parameter, we post it on the panel.
    /// Else, we post the parameter name and 0, signifying a lack of value.
    /// </summary>
    void Start()
    {
        for (int i = 0; i < characterParameters.parameters.Count; i++)
        {
            GameObject newText = Instantiate(text, transform);
            newText.GetComponent<TextCharacterValue>().Set(characterParameters.parameters[i], character);
        }
    }

    /// <summary>
    /// GetCharacter() replaces our currently saved character with a new one.
    /// This is called at the start of a turn to change the turn character on the panel.
    /// </summary>
    /// <param name="_character"> unit to change the panel for </param>
    public void GetCharacter(Character_Root _character) { character = _character; }
}
