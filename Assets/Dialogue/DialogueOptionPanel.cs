using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueOptionPanel : MonoBehaviour
{
    private DialogueManager dialogueManager;
    public List<DialogueOptionButton> optionButtons;
    public GameObject buttonPrefab;

    internal void init(DialogueManager _dialogueManager)
    {
        dialogueManager = _dialogueManager;
    }

    internal void UpdatePanel(List<Dialogue> options)
    {
        while (optionButtons.Count > options.Count)
        {
            Destroy(optionButtons[0].gameObject);
            optionButtons.RemoveAt(0);
        }

        for (int i = 0; i < options.Count; i++)
        {
            if (i >= optionButtons.Count)
            {
                GameObject newButton = Instantiate(buttonPrefab, transform);
                optionButtons.Add(newButton.GetComponent<DialogueOptionButton>());
            }
            optionButtons[i].Set(dialogueManager, this, options[i]);
        }
    }

    internal void DeactivateOptions()
    {
        while (optionButtons.Count > 0)
        {
            Destroy(optionButtons[0].gameObject);
            optionButtons.RemoveAt(0);
        }

        dialogueManager.ActivatePanel();
    }
}
