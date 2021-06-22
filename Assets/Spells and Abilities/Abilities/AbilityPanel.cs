using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbilityPanel is the script used by the ability panel in the turn canvas panel collection.
/// THe ability panel shows a user a list of their available non-passive skills
/// (Passive skills are to be shown in the unit's character status panel)
/// </summary>
public class AbilityPanel : MonoBehaviour
{
    // List of buttons
    List<AbilityButton> abilityButtons;
    public CharacterManager charMan; // Character reference
    public GameObject buttonPrefab; // prefab

    // panels and buttons
    public GameObject buttonPanel;
    public GameObject backButton;

    /// <summary>
    /// Use() is called when a button on this panel is pressed.
    /// It checks for the skill associated with that button, and checks if it is able to 
    /// be activated that turn.
    /// </summary>
    /// <param name="id"> Pressed button's sibling index </param>
    internal void Use(int id)
    {
        // Find the skill associated with the button
        Ability chosenSkill = charMan.skillController.activeSkills[id];
        Debug.Log(charMan.name + " activates: " + chosenSkill.GetName());

        // Check whether the skill hasn't already been used enough times this turn
        if (chosenSkill.CheckUses(charMan))
        {
            // If the skill requires a target, activate that script
            if (chosenSkill.needTarget == true) { ActivateAbilityUse(chosenSkill); }

            // Else, look at the skill's MP cost
            else
            {
                // If we can afford to pay the MP price, activate the skill
                if (charMan.skillController.SpendMana(chosenSkill.GetMPCost())) { chosenSkill.Activate(charMan); }
                else { Debug.Log("Unable to activate ability; not enough MP/HP to cast."); }
            }

        }
    }

    /// <summary>
    /// UpdatePanel() checks whether the amount of buttons on the panel equal
    /// the amount of heroic skills available to the user.
    /// </summary>
    public void UpdatePanel()
    {
        // If we have too many buttons, remove any extras
        while (abilityButtons.Count > charMan.skillController.activeSkills.Count)
        {
            Destroy(abilityButtons[0].gameObject);
            abilityButtons.RemoveAt(0);
        }

        // Within this for loop, we want a # of buttons equal to the 
        // # of skills available to us
        for (int i = 0; i < charMan.skillController.activeSkills.Count; i++)
        {
            // If i is greater than the # of buttons on the panel
            if (i >= abilityButtons.Count)
            {
                // Create a new button and save it to the list
                GameObject newButton = Instantiate(buttonPrefab, transform);
                abilityButtons.Add(newButton.GetComponent<AbilityButton>());
            }
            // set the button with the information it requires
            abilityButtons[i].Set(charMan.skillController.activeSkills[i]);
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
        AbilityUseAction abilityUse = charMan.abilityUse;
        abilityUse.enabled = true;
        abilityUse.SetAbility(_ability);

        // Turn the other stuff OFF
        buttonPanel.SetActive(false);
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
