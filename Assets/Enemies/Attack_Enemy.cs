using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack_Enemy is the AttackAction subclass used by NPC units
/// </summary>
public class Attack_Enemy : AttackAction
{
    // Because the enemy can't select weapons on its own from its own inventory,
    // we keep track of them here
    public List<Weapon> additionalWeapons;

    /// <summary>
    /// EnemyAttack() calls upon the Attack() function the moment we find a suitable target for our attack.
    /// This always uses the weapon in the unit's main hand.
    /// </summary>
    /// <param name="_target"> Unit to target for our attack </param>
    public void EnemyAttack(CharacterManager _target)
    {
        target = _target; // Save a reference to our target
        Attack();
    }

    /// <summary>
    /// SecondaryWeaponAttack() takes a target to attack, but it also receives a 
    /// specific weapon(s) to use for this attack action
    /// </summary>
    /// <param name="_target"> Unit to target for our attack </param>
    /// <param name="_main"> Main hand weapon, required </param>
    /// <param name="_offhand"> Offhand weapon, optional </param>
    public void SecondaryWeaponAttack(CharacterManager _target, Weapon _main, Weapon _offhand = null)
    {
        target = _target; // Save a reference to our target
        Attack(_main, _offhand);
    }

    /// <summary>
    /// ReturnWeaponAtIndex() finds the weapon located in the 
    /// additionalWeapons list at a given index and returns it.
    /// 
    /// You're kind of expected to know what weapon is stored in what position here.
    /// </summary>
    /// <param name="index"> Index to find the weapon at </param>
    /// <returns></returns>
    public Weapon ReturnWeaponAtIndex(int index)
    {
        // If the index is greater than the amount of items in the list, return null
        if (index > additionalWeapons.Count) { return null; }
        // Else, return the weapon
        else { return additionalWeapons[index]; }
    }
}
