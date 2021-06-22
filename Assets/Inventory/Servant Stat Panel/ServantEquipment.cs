using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ServantEquipment is a scriptable object that holds all of a Servant's 
/// equipment and abilities in one spot.
/// </summary>
[CreateAssetMenu(menuName = "Data/Servant Inventory")]
public class ServantEquipment : ScriptableObject
{
    public Weapon mainWeapon; // Servant's weapon
    public Weapon offWeapon; // Offhand weapon, if any
    public Armour armour; // Servant's armour
    public List<Ability> abilities; // Active abilities
    public List<AbilityEffect> passives; // Passive abilities and effects
    public List<SpellSchool> spells; // Spell schools available to the servant
    public List<Ability> NoblePhantasms; // NP - the super move(s) available to all Servants
}
