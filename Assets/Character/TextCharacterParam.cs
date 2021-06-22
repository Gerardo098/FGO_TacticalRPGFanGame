using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCharacterParam : MonoBehaviour
{
    public Parameter trackedParam;
    public Character_Root character;

    void UpdateText()
    {
        string str = character.parameterContainer.GetText(trackedParam);
        GetComponent<Text>().text = str;
    }

    private void Start()
    {
        character.parameterContainer.Subscribe(UpdateText, trackedParam);
    }
}
