using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TurnCanvasControl is a part of the turn system, handling the character action panel for all
/// player-controlled characters.
/// </summary>
public class TurnCanvasControl : MonoBehaviour
{
    public GameObject turnCanvas; // The turn canvas

    private Character_Root character; // PC whose turn it is

    // Children Scripts
    public InventoryManager inventoryManager; // Inventory manager reference
    // One for the activate and one for the back button (TODO - fix this)
    public ActivateMovement activateMovement1;
    public ActivateMovement activateMovement2;
    // Attack activate buttons
    public ActivateAttack activateAttack1;
    public ActivateAttack activateAttack2;
    // Casting button
    public GameObject SpellBackButton;
    public GameObject abilityBackButton;
    public GameObject NPBackButton;
    // Child panels
    public AbilityPanel abilityPanel;
    public NPPanel npPanel;
    public StatsPanel statsPanel;
    public BookPanel bookPanel;
    public SpellPanel spellPanel;

    /// <summary>
    /// ActivateTCC() sends the current unit's data to all "sub-" scripts in the turn canvas.
    /// NOTE - this is only done during the turn of a "player" unit; NPCs don't user the turn canvas.
    /// </summary>
    /// <param name="_character"> Unit whose turn it is </param>
    public void ActivateTCC(Character_Root _character)
    {
        // Save a reference to the character
        character = _character;
        // Set all the Canvas' childrens' scripts to use the newly aquired character
        inventoryManager.Init(character);
        activateMovement1.character = character.gameObject;
        activateMovement2.character = character.gameObject;

        activateAttack1.character = character.gameObject;
        activateAttack2.character = character.gameObject;

        // New unit's ability and Spell lists
        bookPanel.ReceiveCharacter(character.gameObject);
        abilityPanel.ReceiveCharacter(character.charMan); // Grab the char manager here for the ability panel
        npPanel.ReceiveCharacter(character.charMan);
        statsPanel.GetCharacter(character);
    }

    /// <summary>
    /// CanvasON and CanvasOFF both handle switching windows on our action panel on and off
    /// CanvasON is called at the start of the turn, leaving only the necessary objects
    /// on the canvas set to active.
    /// CanvasOFF is called at the end of a turn, turning all child objects under the canvas
    /// to inactive. We also turn the canvas off.
    /// </summary>
    public void CanvasON()
    {
        turnCanvas.SetActive(true); // Set the turn canvas action
        // In addition, turn the canvas' button panel and window nests active HERE
        turnCanvas.transform.Find("ButtonPanel").gameObject.SetActive(true);
        turnCanvas.transform.Find("SkillPanelNest").gameObject.SetActive(true);
        turnCanvas.transform.Find("SpellPanelNest").gameObject.SetActive(true);
        
    }
    public void CanvasOFF()
    {
        // Close all "windows" (aka child objects) in the canvas
        foreach (Transform child in turnCanvas.transform) { child.gameObject.SetActive(false); }
        SpellBackButton.SetActive(false);
        abilityBackButton.SetActive(false);
        NPBackButton.SetActive(false);
        turnCanvas.SetActive(false); // Then turn the canvas off
    }
}
