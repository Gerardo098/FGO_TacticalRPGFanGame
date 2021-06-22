using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Servants are the souls of heroes from mankind's history.
/// Each Servant has their own unique stats, gear, and abilities.
/// In this game, they serve as the player's main equipment.
/// </summary>
[CreateAssetMenu(menuName = "Data/Servant")]
public class Servant : ItemBase
{
    public ItemParameterContainer stats; // Stats associated with the Hero
    public ServantEquipment servantGear; // Abilities and gear possessed by the Hero

    // Return functions
    public List<Ability> ReturnAbilities() { return servantGear.abilities; }
    public List<AbilityEffect> ReturnPassives() { return servantGear.passives; }
    public List<Ability> ReturnNP() { return servantGear.NoblePhantasms; }
    public List<SpellSchool> ReturnSpellSchools() { return servantGear.spells; }
}
