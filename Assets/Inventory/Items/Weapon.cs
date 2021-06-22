using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// As the name implies, Weapon is a subclass of the ItemBase class that focuses on 
/// weapons of all kinds.
/// </summary>
public abstract class Weapon : ItemBase
{
    public float range; // How far can this weapon reach?
    internal int number; // Number of damage dice
    internal int size; // Type of damage dice (ex. d4, d6, d8, etc.)
    public List<AbilityEffect> weaponEffects; // Any effects the weapon might have

    public int ReturnDieNumber() { return number; }
    public int ReturnDieSize() { return size; }
    public float ReturnRange() { return range; }
    public void GetDamageDice(out int _number, out int _size)
    {
        RankRead();
        _number = number;
        _size = size;
    }
    public abstract void RankRead();
}
