using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A slightly different version of the original SpellPanelToggle script
 * This one is used exclusively by the Spell panels; closing the main panel (bookPanel)
 * Also closes the child panel (spellPanel).
 */

public class SpellPanelToggle : MonoBehaviour
{
    public GameObject bookPanel;
    public GameObject spellPanel;

    /*
     * TogglePanel() does what it says - opens or closes a panel, depending what it's 
     * currently doing right now
     */
    public void TogglePanel()
    {
        if (bookPanel != null)
        {
            bookPanel.SetActive(!bookPanel.activeSelf);
        }
        // Turn the spell panel off just in case
        spellPanel.SetActive(false);
    }
}
