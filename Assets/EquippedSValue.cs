using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSValue : MonoBehaviour
{
    public Equipment equipment;
    Text text;
    
    void UpdateText()
    {
        string str = "Servant: " + equipment.curServant.GetName();
    }

    //public void Set()
}
