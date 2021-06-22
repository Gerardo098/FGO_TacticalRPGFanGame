using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPPanel is the script used by the NP panel in the turn canvas panel collection.
/// THe NPPanel shows a user a list of their available "active" Noble Phantasm
/// (Passive NPs are to be shown in the unit's character status panel)
/// </summary>
public class NPPanel : MonoBehaviour
{
    // List of buttons
    List<AbilityButton> abilityButtons;
    public CharacterManager charMan; // Character reference
    public GameObject buttonPrefab; // Prefab

    // Panels and buttons
    public GameObject buttonPanel;
    public GameObject backButton;

    /// <summary>
    /// Use() is called when a button on this panel is pressed.
    /// It checks for the NP associated with that button, and checks if it is able to 
    /// be activated that turn.
    /// </summary>
    /// <param name="id"> Pressed button's sibling index </param>
    internal void Use(int id)
    {
        // Find the NP associated with the button
        Ability chosenNP = charMan.skillController.activeNPs[id];
        Debug.Log(charMan.name + " activates: " + chosenNP.GetName());

        // Check whether the NP hasn't already been used enough times this turn
        if (chosenNP.CheckUses(charMan))
        {
            // If the NP requires the unit to select a target, activate that script
            if (chosenNP.needTarget == true) { ActivateAbilityUse(chosenNP); }

            // Else, look at the NP's mana cost
            else
            {
                // If the unit can afford to pay the MP price, activate the noble phantasm
                if (charMan.skillController.SpendMana(chosenNP.GetMPCost())) { chosenNP.Activate(charMan); }
                // If not, let the user know
                else { Debug.Log("Unable to activate ability; not enough MP/HP to cast."); }
            }
        }
    }

    /// <summary>
    /// UpdatePanel() checks whether the amount of buttons on the panel equal
    /// the amount of NP abilities available to the user.
    /// </summary>
    public void UpdatePanel()
    {
        // If we have too many buttons, remove any extras
        while (abilityButtons.Count > charMan.skillController.activeNPs.Count)
        {
            Destroy(abilityButtons[0].gameObject);
            abilityButtons.RemoveAt(0);
        }

        // Within this for loop, we want a # of buttons equal to the 
        // # of NPs available to us
        for (int i = 0; i < charMan.skillController.activeNPs.Count; i++)
        {
            // If i is greater than the # of buttons on the panel
            if (i >= abilityButtons.Count)
            {
                // Create a new button and save it to the list
                GameObject newButton = Instantiate(buttonPrefab, transform);
                abilityButtons.Add(newButton.GetComponent<AbilityButton>());
            }
            // set the button with the information it requires
            abilityButtons[i].Set(charMan.skillController.activeNPs[i], true);
        }
    }

    /// <summary>
    /// ActivateAbilityUse() receives an active ability that requires a target selected by
    /// the user, as opposed to one that has no target.
    /// We set the relevant gameobjects to active or inactive while enabling the character's
    /// abilityUse script.
    /// </summary>
    /// <param name="_ability"></param>
    public void ActivateAbilityUse(Ability _ability)
    {
        // Find the abilityUseAction script, enable it, and hand it the ability
        AbilityUseAction abilityUse = charMan.abilityUse;
        abilityUse.enabled = true;
        abilityUse.SetAbility(_ability);

        // Turn the other stuff OFF
        buttonPanel.SetActive(false); // turn the turn button panel inactive
        backButton.SetActive(true); // Turn the back button active
        gameObject.SetActive(false); // Turn outselves inactive
    }

    /// <summary>
    /// ReceiveCharacter() takes a CharManager reference and saves it as the character 
    /// to read the NP abilities of.
    /// </summary>
    /// <param name="character"></param>
    internal void ReceiveCharacter(CharacterManager character)
    {
        charMan = character; // Save the char
        // In addition, hand the reference to the backButton object
        backButton.GetComponent<AbilityActionBack>().character = character;
    }

    // OnEnable() is run when the script is enabled
    // At that point, make sure our abilityButtons list exists
    // and update the panel
    private void OnEnable()
    {
        if (abilityButtons == null) { abilityButtons = new List<AbilityButton>(); }
        UpdatePanel();
    }
}
