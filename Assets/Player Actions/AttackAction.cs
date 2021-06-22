using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AttackAction is an abstract class that all units' own attack action scripts are based off of.
/// </summary>
public abstract class AttackAction : MonoBehaviour
{
    public CharacterManager charMan; // Unit this script is attached to
    protected CharacterManager target; // Unit we're going to attack
    protected float attackRange; // How far away this unit can reach to attack
    public bool attacking; // Is the unit currently performing an attack action?

    /// <summary>
    /// The basic Attack() creates an Attack instance for each weapon the unit is wielding
    /// and hands it to the target. 
    /// </summary>
    protected void Attack()
    {
        // Grab the unit's weapons
        Weapon mainWeapon = charMan.equipment.ReturnMainWeapon();
        Weapon offWeapon = charMan.equipment.ReturnOffWeapon();
        // Turn the unit towards the target
        LookAtTarget(gameObject.transform, target.transform);

        // Create an instance for the main weapon and hand it to the target to handle
        AttackInstance attack = new AttackInstance(charMan, target, mainWeapon);
        target.AbilityInstanceRead(attack);

        // If we're dual-wielding, create a 2nd instance
        if (offWeapon != null)
        {
            AttackInstance attack2 = new AttackInstance(charMan, target, offWeapon);
            target.AbilityInstanceRead(attack2);
        }
        // If we got no weapons (main wep included), we still got 2 fists!
        if (mainWeapon == null && offWeapon == null)
        {
            AttackInstance attack2 = new AttackInstance(charMan, target, offWeapon);
            target.AbilityInstanceRead(attack2);
        }

        target = null; // Clear the target
        attacking = false; // No longer attacking
        charMan.character.ReduceActionCount(); // An attack uses up 1 action that turn
    }

    /// <summary>
    /// An Attack() overload function that instead of searching for the unit's weapons, it
    /// instead receives them as arguments. This is used exclusively by NPC units that are carrying
    /// multiple weapons because of their odd inventory/equipment shenanigans.
    /// </summary>
    /// <param name="mainWeapon"> main weapon, necessary </param>
    /// <param name="offWeapon"> off-hand weapon, optional </param>
    protected void Attack(Weapon mainWeapon = null, Weapon offWeapon = null)
    {
        // Look at the target
        LookAtTarget(gameObject.transform, target.transform);
        // Create an instance for the main weapon and hand it to the target to handle
        AttackInstance attack = new AttackInstance(charMan, target, mainWeapon);
        target.AbilityInstanceRead(attack);

        // If offWeapon is not null, create a 2nd instance
        if (offWeapon != null)
        {
            AttackInstance attack2 = new AttackInstance(charMan, target, offWeapon);
            target.AbilityInstanceRead(attack2);
        }
        // If we got no weapons, we still got 2 fists!
        if (mainWeapon == null && offWeapon == null)
        {
            AttackInstance attack2 = new AttackInstance(charMan, target, offWeapon);
            target.AbilityInstanceRead(attack2);
        }

        target = null; // Clear the target
        attacking = false; // No longer attacking
        charMan.character.ReduceActionCount(); // An attack uses up 1 action that turn
    }

    /// <summary>
    /// CheckWeaponRange() sets the attackRange based on the weapon we're holding.
    /// </summary>
    protected void CheckWeaponRange()
    {
        // If we have a weapon, set the float variable
        if (charMan.equipment.mainHandSlot != null)
        {
            attackRange = charMan.equipment.mainHandSlot.GetRange();
        }
        // If we don't have a weapon (unarmed), then it's just 1f
        else { attackRange = 1f; }
    }

    /// <summary>
    /// LookAtTarget() simply has the attacking unit turn towards the target unit.
    /// </summary>
    /// <param name="source"> attacker's position </param>
    /// <param name="target"> target's position </param>
    public void LookAtTarget(Transform source, Transform target)
    {
        // Save the target's x and z position but keep the source's y position
        float targetX = target.position.x;
        float targetZ = target.position.z;
        float sourceY = source.position.y;
        // Create a new vector3 with these coordinates and make the source look that way
        Vector3 direction = new Vector3(targetX, sourceY, targetZ);
        source.LookAt(direction);
    }
}

/// <summary>
/// Attack is a subclasss of AbilityInstance that handles all weapon-based attacks, including unarmed attacks like punching.
/// It works in an assembly-line way, with things read and handled one at a time
/// </summary>
public class AttackInstance : AbilityInstance
{
    // Type of attack
    protected enum AttackType { Melee, Ranged }
    protected AttackType attackType;
    // Weapon used by the attack
    protected Weapon weapon;
    // Attacker stat values and bonuses
    protected int ATKBonus = 0;
    protected int STR = 0;
    protected int MAN = 0;
    protected int CRTBonus = 0;
    protected int DMGBonus = 0;
    protected bool Critical = false;
    // Defender bonuses
    protected int EVABonus = 0;
    protected int ARMBonus = 0;
    // Attacker reroll flags
    protected Reroll advantageATK = Reroll.No;
    protected Reroll advantageCRT = Reroll.No;
    protected Reroll advantageDMG = Reroll.No;

    public int damage; // Total damage

    /// <summary>
    /// Creator function(); receive the basic info of the attack
    /// </summary>
    /// <param name="_source"> unit attacking </param>
    /// <param name="_target"> unit being attacked </param>
    /// <param name="_weapon"> weapon used for the attack </param>
    public AttackInstance(CharacterManager _source, CharacterManager _target, Weapon _weapon)
    {
        // Save all variables
        source = _source;
        target = _target;
        weapon = _weapon;
    }

    /// <summary>
    /// ActivateAttackInstance() gets the ability instance rolling
    /// </summary>
    public void ActivateAttackInstance()
    {
        // Grab important weapon info
        CheckWeapon();

        // Check for effects and triggers prior to the attack
        OnAttackSearches();

        // Roll the attack
        int roll = AttackRoll();
        bool hitFlag = CompareEVA(roll);

        // We have successfully hit the target
        if (hitFlag)
        {
            OnHitSearches();
            // Roll for damage
            int dmg = DamageRoll();
            bool dmgFlag = InflictDamage(dmg);
            if (dmgFlag) { Effects_OnDamage(); }
            return;
        }

        // Else, we the attacker failed to strike
        else { Debug.Log(source.unitName + "'s attack failed to strike the target!"); }
    }

    /// <summary>
    /// CheckWeapon() reads the weapon and saves the type of weapon that it is, melee or ranged.
    /// Also checks if we're barehanded or not.
    /// </summary>
    private void CheckWeapon()
    {
        // For whether it's a melee or ranged attack, we always check the main hand weapon
        // If unarmed, it's automatically a melee attack
        if (weapon == null)
        {
            attackType = AttackType.Melee;
            ChangeATKAdvantage(Reroll.Disadvantage); // Units suffer disadvantage when fighting unarmed
        }
        else // Else, check the weapon's tags
        {
            if (weapon.tags.Contains("Ranged")) { attackType = AttackType.Ranged; }
            else { attackType = AttackType.Melee; }
        }
    }

    /// <summary>
    /// OnAttackSearches() finds all values important to the instance BEFORE rolling to hit
    /// </summary>
    private void OnAttackSearches()
    {
        GrabATK();
        GrabEVA();
        Effects_ToHit();
    }
    /// <summary>
    /// OnHitSearches() finds all values important to the instance AFTER the attack hit
    /// </summary>
    private void OnHitSearches()
    {
        GrabCRT();
        GrabARM();
        Effects_OnHit();
        Critical = CriticalThreat();
    }

    /// <summary>
    /// These Effects_X() functions look for passive abilities and status effects that would alter the
    /// attack depending on their timing, as written in the name.
    /// </summary>
    private void Effects_ToHit()
    {
        source.unitEffects.SearchPassives(Timing.OnAttack_Source, this);
        target.unitEffects.SearchPassives(Timing.OnAttack_Target, this);
    }
    private void Effects_OnHit()
    {
        source.unitEffects.SearchPassives(Timing.OnHit_Source, this);
        target.unitEffects.SearchPassives(Timing.OnHit_Target, this);
    }
    private void Effects_OnDamage()
    {
        source.unitEffects.SearchPassives(Timing.OnDamage_Source, this);
        target.unitEffects.SearchPassives(Timing.OnDamage_Target, this);
    }

    /// <summary>
    /// AttackRoll() is a function that handles our attacker's To Hit roll 
    /// and whether it successfully meets the target or not
    /// </summary>
    /// <returns> amount rolled </returns>
    private int AttackRoll()
    {
        // Check for advantage / disadvantage on the roll
        switch (advantageATK)
        {
            case Reroll.Advantage:
                return RollingManager.BasicRoll(source, ATKBonus, Reroll.Advantage);
            case Reroll.Disadvantage:
                return RollingManager.BasicRoll(source, ATKBonus, Reroll.Disadvantage);
            case Reroll.No: // If no advantage or no flag, don't reroll
            default:
                return RollingManager.BasicRoll(source, ATKBonus);
        }
    }

    /// <summary>
    /// CompareEVA() compares the attacker's to hit roll against the target's evasion stat.
    /// If the amount rolled is higher than the evasion, the attack hits. Else, it misses.
    /// </summary>
    /// <param name="roll"> amount rolled by the attacker </param>
    /// <returns> bool flag on whether the attack struck or not </returns>
    private bool CompareEVA(int roll)
    {
        if (roll > EVABonus) { return true; } // We have successfully hit
        // Else, we've missed
        return false;
    }

    /// <summary>
    /// CriticalThreat() checks to see if our successful attack was a critical hit or not
    /// </summary>
    /// <returns> bool flag on whether the attack inflicts critical damage </returns>
    public bool CriticalThreat()
    {
        int threat; // int variable for our roll

        switch (advantageCRT)
        {
            case Reroll.Advantage:
                threat = RollingManager.BasicRoll(source, CRTBonus, Reroll.Advantage);
                break;
            case Reroll.Disadvantage:
                threat = RollingManager.BasicRoll(source, CRTBonus, Reroll.Disadvantage);
                break;
            default:
                threat = RollingManager.BasicRoll(source, CRTBonus);
                break;
        }

        // Check to see if the amount rolled is greater than or equal to 20 (20+)
        return threat >= 20;
    }

    /// <summary>
    /// DamageRoll() grabs the damage numbers from the weapon and rolls for damage
    /// </summary>
    /// <returns> amount rolled by the damage die </returns>
    private int DamageRoll()
    {
        // Check how strong the weapon is
        weapon.GetDamageDice(out int number, out int size);

        // Check on whether we have adv or dis on our damage roll
        switch (advantageDMG)
        {
            case Reroll.Advantage:
                return RollingManager.CustomRoll(source, number, size, DMGBonus, Reroll.Advantage);
            case Reroll.Disadvantage:
                return RollingManager.CustomRoll(source, number, size, DMGBonus, Reroll.Disadvantage);
            default:
                return RollingManager.CustomRoll(source, number, size, DMGBonus);
        }
    }

    /// <summary>
    /// InflictDamage() takes the amount of damage rolled above and reduces the enemy's health by the amount.
    /// However, damage is first reduced by the unit's armour.
    /// </summary>
    /// <param name="damage"> amount to remove from the target's HP, prior to reduction </param>
    /// <returns> whether the damage broke the target's armour or not </returns>
    private bool InflictDamage(int damage)
    {
        if (Critical) 
        {
            Debug.Log("Critical hit!");
            damage *= 2;
        }

        // Reduce the damage by the target's total armour
        int reduced = damage - ARMBonus;
        
        if (reduced > 0)
        {
            Debug.Log(source.unitName + " inflicted " + reduced + " damage to " + target.unitName);
            target.character.ReduceHealth(reduced);
            return true; // We successfully damaged the target
        }

        // We failed to break through the target's defenses
        return false;
    }

    // Return functions
    public Weapon ReturnWeapon() { return weapon; }

    // Advantage Functions
    /// <summary>
    /// The ChangeXAdvantage() functions receive a Reroll enum and hand it, and their respective flag, to the CompareRerollFlag() function
    /// </summary>
    /// <param name="reroll"> Reroll flag we want to apply to a flag </param>
    public void ChangeATKAdvantage(Reroll reroll)
    {
        advantageATK = CompareRerollFlag(advantageATK, reroll);
    }
    public void ChangeCRTAdvantage(Reroll reroll)
    {
        advantageCRT = CompareRerollFlag(advantageCRT, reroll);
    }
    public void ChangeDMGAdvantage(Reroll reroll)
    {
        advantageDMG = CompareRerollFlag(advantageDMG, reroll);
    }

    /// <summary>
    /// CompareRerollFlag() checks the given reroll enum flag and compares it
    /// to the current flag. A decision is made depending on what both are set to.
    /// </summary>
    /// <param name="current"> What the reroll enum is set to at the moment </param>
    /// <param name="change"> what an effect wishes to have the flag changed to </param>
    /// <returns> the final flag result </returns>
    private Reroll CompareRerollFlag(Reroll current, Reroll change)
    {
        // If the current flag is set to no, return the new flag
        if (current == Reroll.No) { return change; }
        // If the flags are the same, don't change anything
        if (current == change) { return current; }

        // If they are different however, return a NO reroll flag
        return Reroll.No;
    }

    // Simple functions

    /// <summary>
    /// SetEVABonus() receives an int and adds it to the target unit's EVA bonus.
    /// You can pass negative values to represent penalties to EVA.
    /// </summary>
    /// <param name="bonus"> Amount to add/subtract </param>
    public void SetEVABonus(int bonus) { EVABonus += bonus; }

    /// <summary>
    /// SetARMBonus() receives an int and adds it to the target unit's ARM bonus.
    /// You can pass negative values to represent penalties to ARM.
    /// </summary>
    /// <param name="bonus"> Amount to add/subtract </param>
    public void SetARMBonus(int bonus) { ARMBonus += bonus; }

    /// <summary>
    /// SetDMGBonus() receives an int and adds it to the attacking unit's DMG bonus.
    /// You can pass negative values to represent penalties to DMG.
    /// </summary>
    /// <param name="bonus"> Amount to add/subtract </param>
    public void SetDMGBonus(int bonus) { DMGBonus += bonus; }

    /// <summary>
    /// GrabATK() finds the attacking unit's melee or range stat, depending on what type their weapon is.
    /// </summary>
    private void GrabATK()
    {
        switch (attackType)
        {
            case AttackType.Melee:
                source.character.parameterContainer.Get(source.Melee, out ATKBonus);
                break;
            case AttackType.Ranged:
                source.character.parameterContainer.Get(source.Range, out ATKBonus);
                break;
        }
    }
    
    /// <summary>
    /// The GrabX() functions call for specific parameters to be read
    /// </summary>
    private void GrabCRT()
    {
        source.character.parameterContainer.Get(source.CRT, out CRTBonus);
    }
    private void GrabEVA()
    {
        target.character.parameterContainer.Get(target.EVA, out EVABonus);
    }
    private void GrabARM()
    {
        target.character.parameterContainer.Get(target.ARM, out ARMBonus);
    }
}