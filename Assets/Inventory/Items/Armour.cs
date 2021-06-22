using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Armour is a class handling armour-type equipment for units.
/// Unlike weapons, armour is pretty basic and their only 
/// differences are effects (if any) and their rank.
/// </summary>
[CreateAssetMenu(menuName = "Data/Armour")]
public class Armour : ItemBase
{
    public Parameter ARM; // Armour, that we add to the unit
    public List<AbilityEffect> armourEffects;

    public int GetArmourScore()
    {
        switch (rank)
        {
            case Rank.E: return 1;
            case Rank.D: return 2;
            case Rank.C: return 3;
            case Rank.B: return 4;
            case Rank.A: return 5;
            case Rank.EX: return 6;
            default: return 0;
        }
    }
}
