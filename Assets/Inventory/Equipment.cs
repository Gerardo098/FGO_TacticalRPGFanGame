using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot
{
    public Weapon equippedWeapon;
    internal void Equip(Weapon weapon) { equippedWeapon = weapon; }
    internal string GetName() { return equippedWeapon.name; }
    internal Weapon GetWeapon() { return equippedWeapon; }
    internal float GetRange() { return equippedWeapon.range; }
}

public class ArmourSlot
{
    public Armour equippedArmour;

    internal void Equip(Armour armour) { equippedArmour = armour; }
    internal string GetName() { return equippedArmour.name; }
    internal Armour GetArmour() { return equippedArmour; }
    internal Parameter GetARM() { return equippedArmour.ARM; }
    internal int GetArmourScore() { return equippedArmour.GetArmourScore(); }
}

public class ServantSlot
{
    public Servant equippedServant;
    internal void Equip(Servant servant) { equippedServant = servant; }
    internal string GetName() { return equippedServant.name; }
}

/*
 * The Equipment class - It is here that all of a character's equipment slots are found
 */
public class Equipment : MonoBehaviour
{
    public CharacterManager characterManager;
    public ServantSlot curServant; // Servant currently equipped by the unit
    public WeaponSlot mainHandSlot; // 1st weapon slot
    public WeaponSlot offHandSlot; // 2nd weapon slot
    public ArmourSlot armour;
    public Action onChange;

    /*
     * TODO
     *      Player has one equipment slot for their equipped Servant and an array of slots for whatever the Servant's
     *      equipment is.
     *      The Servant, when equipped, calls a function that equips all of their equipment onto the player. Which then
     *      adds all bonuses and whatever. The same is true for unequipping a Servant - everything the Servant is carrying is
     *      unequipped when said Servant is switched out for another.
     *      
     *      NOTE: use IF X IS Y to get the scripts
     */

    /*
     * Init() function initiates the equipmentSlots array of size equal to the max # of open slots we can have
     */
    public void Init()
    {
        curServant = new ServantSlot();
        mainHandSlot = new WeaponSlot();
        offHandSlot = new WeaponSlot();
        armour = new ArmourSlot();
        // Initiate equipment slots
    }

    public void EquipWeapon(Weapon toEquip)
    {
        // If neither slots are empty, return
        if (mainHandSlot != null && offHandSlot != null) { return; }

        // Check if the weapon we're trying to equip is a two-handed weapon
        if (toEquip.tags.Contains("Two-Handed"))
        {
            // We have the weapon take up both hands; to avoid "dual wield" shenanigans in combat
            // that will check the weapon there
            mainHandSlot.Equip(toEquip);
            offHandSlot.Equip(toEquip);
        }
        else
        {
            // Is the main hand slot free?
            if (mainHandSlot == null) { mainHandSlot.Equip(toEquip); }
            // If not, put it in the off-hand
            else { offHandSlot.Equip(toEquip); }
        }
    }

    /*
     * EquipServant() is simply a specialized Equip() function that adds our Servant to the specified Servant slot
     */
    public void EquipServant(Servant servant)
    {
        curServant.Equip(servant);
        if (onChange != null) { onChange.Invoke(); }
        EquipGear();
    }

    /*
     * EquipGear() and UnequipGear() are a pair of functions that apply/remove a Servant's gear onto/from the player.
     */
    public void EquipGear()
    {
        ServantEquipment newGear = curServant.equippedServant.servantGear;
        mainHandSlot.Equip(newGear.mainWeapon);
        offHandSlot.Equip(newGear.offWeapon);
        // Add armour
        if (newGear.armour != null)
        {
            armour.Equip(newGear.armour);
            characterManager.character.parameterContainer.Sum(newGear.armour.ARM, newGear.armour.GetArmourScore());
        }
    }

    public void UnequipGear()
    {
        mainHandSlot = null;
        offHandSlot = null;
        // Remove armour
        if (armour != null)
        {
            characterManager.character.parameterContainer.Subtract(armour.GetARM(), armour.GetArmourScore());
            armour = null;
        }
    }

    // Return Methods
    public Weapon ReturnMainWeapon() { return mainHandSlot.GetWeapon(); }
    public Weapon ReturnOffWeapon() { return offHandSlot.GetWeapon(); }
    internal Armour ReturnArmour() { return armour.equippedArmour; }
}
