 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPanel : MonoBehaviour
{
    public List<SpellButton> spellButtons;
    public CharacterManager characterManager;
    public GameObject buttonPrefab;
    public SpellSchool spellSchool;
    public int MANA; // the character's mana score

    public GameObject buttonPanel;
    public GameObject bookPanel;
    public GameObject backButton;

    public void UpdatePanel()
    {
        int spells = spellSchool.availableSpells.Count;

        while (spellButtons.Count > spells)
        {
            Destroy(spellButtons[0].gameObject);
            spellButtons.RemoveAt(0);
        }

        for (int i = 0; i < spells; i++)
        {
            if (i >= spellButtons.Count)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                spellButtons.Add(newButton.GetComponent<SpellButton>());
            }
            spellButtons[i].Set(spellSchool.availableSpells[i], spellSchool.rank);
        }
    }

    internal void Use(int id)
    {
        //Debug.Log("The unit casts: " + spellSchool.availableSpells[id].GetName());

        // Turn the CastingAction component ON
        CastingAction castAction = characterManager.castingAction;
        castAction.enabled = true;
        castAction.SetSpell(spellSchool.availableSpells[id]);

        // Turn the other stuff OFF
        buttonPanel.SetActive(false);
        bookPanel.SetActive(false);
        backButton.SetActive(true); // Turn the back button active
        gameObject.SetActive(false); // Turn outselves inactive
    }

    private void OnEnable()
    {
        if (spellButtons == null) { spellButtons = new List<SpellButton>(); }
        spellSchool = bookPanel.GetComponent<BookPanel>().selectedSchool;
        UpdatePanel();
    }
}
