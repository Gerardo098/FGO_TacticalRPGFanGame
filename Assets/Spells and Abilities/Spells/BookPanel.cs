using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPanel : MonoBehaviour
{
    // List of buttons
    public List<SpellButton> spellButtons;
    public CharacterManager characterManager;
    public SpellSchool selectedSchool;
    public GameObject buttonPrefab;
    public GameObject spellPanel;
    public GameObject backButton;

    public void UpdatePanel()
    {
        while (spellButtons.Count > characterManager.skillController.schools.Count)
        {
            Destroy(spellButtons[0].gameObject);
            spellButtons.RemoveAt(0);
        }

        for (int i = 0; i < characterManager.skillController.schools.Count; i++)
        {
            if (i >= spellButtons.Count)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                spellButtons.Add(newButton.GetComponent<SpellButton>());
            }
            spellButtons[i].Set(characterManager.skillController.schools[i]);
        }
    }

    /*
     * In the BookPanel, the Use() function turns the spell panel on or off
     */
    internal void Use(int id)
    {
        // Turn the bookPanel on or off
        if (spellPanel != null)
        {
            selectedSchool = spellButtons[id].GetSchool();
            spellPanel.SetActive(!spellPanel.activeSelf);
        }
    }

    private void OnEnable()
    {
        if (spellButtons == null) { spellButtons = new List<SpellButton>(); }
        UpdatePanel();
    }

    /*
     * ReceiveCharacter takes a character_root, finds the skill controller from it
     * and saves it. Also, we pass it to the Spell Book panel for safekeeping
     */
    internal void ReceiveCharacter(GameObject character)
    {
        characterManager = character.GetComponent<CharacterManager>();
        // Give the spellBookPanel our skillcontroller and MANA parameter
        spellPanel.GetComponent<SpellPanel>().characterManager = characterManager;
        backButton.GetComponent<CastActionBack>().character = characterManager;
    }
}