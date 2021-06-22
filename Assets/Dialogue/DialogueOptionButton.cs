using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueOptionButton : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI nameText;
    private DialogueManager dialogueManager;
    private DialogueOptionPanel optionPanel;
    private Dialogue optionCase;

    public void OnPointerClick(PointerEventData eventData)
    {
        dialogueManager.StartDialogue(optionCase); // Hand the new dialogue to the dialogue manager
        optionPanel.DeactivateOptions();
    }

    public void Set(DialogueManager manager, DialogueOptionPanel _optionPanel, Dialogue option)
    {
        dialogueManager = manager;
        optionPanel = _optionPanel;
        optionCase = option;
        if (nameText == null) { nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>(); }
        nameText.text = optionCase.optionText;
    }
}
