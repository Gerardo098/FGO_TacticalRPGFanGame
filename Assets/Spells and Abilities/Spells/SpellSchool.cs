using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Spell School")]
public class SpellSchool : ScriptableObject
{
    // Name of the school in question
    public string schoolName;
    // All spells of belonging to this school
    public List<Spell> availableSpells;
    // Available Rank - used by characters to know what level of spells from the school they can cast
    public Rank rank;

    public string GetName() { return schoolName + " [" + rank + "]"; }
}
