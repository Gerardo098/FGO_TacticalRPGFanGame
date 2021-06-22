using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CastingAction is an abstract class that all units' own cast action scripts are based off of.
/// </summary>
public abstract class CastingAction : MonoBehaviour
{
    public bool casting; // True if we have a legal target or not
    public CharacterManager characterManager; // Reference to this script's owner
    internal List<CharacterManager> targetList; // List of targets affected by the cast spell
    internal Spell currentSpell; // Spell being used

    /// <summary>
    /// SetSpell() receives a spell and saves a reference to it as "currentSpell"
    /// </summary>
    /// <param name="spell"> spell to be saved </param>
    public void SetSpell(Spell spell) { currentSpell = spell; }

    /// <summary>
    /// Cast() is a coroutine function that handles the activation of a spell
    /// </summary>
    /// <returns> IEnumerator </returns>
    internal IEnumerator Cast()
    {
        // If we have a spell - can't cast if we don't
        if (currentSpell != null)
        {
            currentSpell.Activate(characterManager, targetList); // Cast the spell
            // We cast the spell - tell the game to stop with the casting
            casting = false;
            characterManager.character.ReduceActionCount(); // We used up an action here
            // Done with the coroutine
            yield return null;
        }
    }

    /// <summary>
    /// LookAtTarget() simply has the attacking unit turn towards the target unit.
    /// </summary>
    /// <param name="source"> attacker's transform </param>
    /// <param name="target"> target's transform </param>
    public void LookAtTarget(Transform source, Transform target)
    {
        // Save the target's X and Z, but keep the source's Y
        float targetX = target.position.x;
        float targetZ = target.position.z;
        float sourceY = source.position.y;

        // Create a new vector3 with the X, Y, and Z, and have the source look in that direction
        Vector3 direction = new Vector3(targetX, sourceY, targetZ);
        source.LookAt(direction);
    }
    /// <summary>
    /// Overload of LookAtTarget() to receuve a vector3 instead of the transform of the target
    /// because it worked better in the Casting_Player subclass.
    /// </summary>
    /// <param name="source"> attacker's transform </param>
    /// <param name="target"> target's position </param>
    public void LookAtTarget(Transform source, Vector3 target)
    {
        // Save the target's X and Z, but keep the source's Y
        float targetX = target.x;
        float targetZ = target.z;
        float sourceY = source.position.y;

        // Create a new vector3 with the X, Y, and Z, and have the source look in that direction
        Vector3 direction = new Vector3(targetX, sourceY, targetZ);
        source.LookAt(direction);
    }
}

/// <summary>
/// SpellInstance is the ability instance subclass used exclusively for spells
/// </summary>

// NOTE - CHECK FOR MP COSTS BEFORE CREATING A SPELL INSTANCE
public class SpellInstance : AbilityInstance
{
    protected Spell spell; // Spell used by the instance
    protected List<string> spellTags; // Spell's tags

    // Attacker's stats and bonuses
    protected int ATKBonus;
    protected int MAN;
    protected int DMGBonus;

    // Target's bonuses
    protected int EVABonus = 0;
    protected int ARMBonus = 0;
    protected int DCBonus = 0;
    protected Rank resistance = Rank.None;

    // Reroll flags
    protected Reroll advantageATK = Reroll.No;
    protected Reroll advantageDMG = Reroll.No;

    /// <summary>
    /// Creator function; receive all the necessary variables and save them
    /// </summary>
    /// <param name="_source"> source unit casting the spell </param>
    /// <param name="_target"> target of the spell </param>
    /// <param name="_spell"> spell being cast </param>
    public SpellInstance(CharacterManager _source, CharacterManager _target, Spell _spell)
    {
        source = _source; // Grab the attacker
        target = _target; // Grab the target
        spell = _spell; // Save the spell

        // Save the spell's rank and tags
        rank = spell.rank;
        spellTags = spell.tags;
    }

    /// <summary>
    /// HandleSpell() activates the instance to proceed with its assembly line process
    /// </summary>
    public void HandleSpell()
    {
        if (spell.HitRoll == true) // If the spell has to hit to inflict its effect, handle that here
        {
            OnAttackSearch(); // Search for effects that'd trigger now
            int roll = AttackRoll(); // Calculate the to hit roll
            bool hitFlag = CompareEVA(roll); // Compare the roll to the target's evasion

            // If we hit, find all worthwhile triggers and apply the spell's effects
            if (hitFlag)
            {
                OnHitSearch();
                ApplyOnHit();
            }

            // Else, the spell missed
            else { Debug.Log(source.unitName + "'s spell failed to strike the target!"); }
        }
        // If the spell does not require a hit roll
        else 
        {
            // Immediately find the on hit triggers and apply the effects
            OnHitSearch();
            ApplyOnHit();
        }
    }

    /// <summary>
    /// AttackRoll() takes the source unit's spellcasting stat and calculates their attack roll
    /// </summary>
    /// <returns> roll result, used as the attack roll </returns>
    private int AttackRoll()
    {
        // Check for rerolls
        switch (advantageATK)
        {
            case Reroll.Advantage:
                return RollingManager.BasicRoll(source, ATKBonus, Reroll.Advantage);
            case Reroll.Disadvantage:
                return RollingManager.BasicRoll(source, ATKBonus, Reroll.Disadvantage);
            case Reroll.No: // If no rerolls or somehow no flag, ignore rerolls
            default:
                return RollingManager.BasicRoll(source, ATKBonus);
        }
    }

    /// <summary>
    /// The Search() functions look for stats and effects that would be worthwhile at that point in the asembly line
    /// </summary>
    private void OnAttackSearch()
    {
        GrabATK();
        GrabEVA();
        Effects_ToHit();
    }
    private void OnHitSearch()
    {
        GrabMAN();
        GrabARM();
        Effects_OnHit();
    }

    /// <summary>
    /// ApplyOnHit() handles the spell's effects, applying them to the target
    /// </summary>
    private void ApplyOnHit()
    {
        // If the target proved to have a resistance to spells, the spell is defended against
        if (!CheckResistance()) 
        {
            Debug.Log(source.unitName + "'s spell had no effect - " + 
                target.unitName + "'s Magic Resistance is too high!");
            return;
        }

        // Inflict damage here
        bool damageFlag; // Flag used to see if the spell does inflict damage
        if (spell.damageDealing) 
        { 
            damageFlag = HandleDamage(); // Check if the spell's damage beat the target's defenses
            if (damageFlag) { Effects_OnDamage(); } // Check for triggers here
        }

        // Check for and apply effect(s) here
        if (spell.HasEffect) { ApplyEffect(); }
    }

    /// <summary>
    /// ApplyEffects() checks the spell for any effects it might be holding.
    /// If so, create an effect instance to handle them, and pass them to the target
    /// </summary>
    private void ApplyEffect() 
    {
        AbilityEffect effect = spell.spellEffect;
        EffectInstance instance = new EffectInstance(effect, target, source);
        target.AbilityInstanceRead(instance);
    }

    /// <summary>
    /// HandleDamage() calls the DamageRoll() and InflictDamage() functions
    /// </summary>
    /// <returns> True if the target suffered damage; false otherwise </returns>
    private bool HandleDamage()
    {
        int damage = DamageRoll(); // Save our damage roll
        return(InflictDamage(damage)); // Check if we've done damage
    }

    /// <summary>
    /// DamageRoll() checks the spell for its damage die, and rolls for damage
    /// </summary>
    /// <returns>  </returns>
    private int DamageRoll()
    {
        // Find how much damage the spell inflicts
        spell.GetDamageDice(out int number, out int size);
        DMGBonus += MAN; // The target's mana increases damage

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
    /// InflictDamage takes an int, an amount of damage, and compares it to the target's defenses.
    /// </summary>
    /// <param name="damage"> amount of damage to be dealt, before alterations </param>
    /// <returns> true if the target suffered damage, false otherwise </returns>
    private bool InflictDamage(int damage)
    {
        // Reduce the damage by the target's total armour
        int reduced = damage - ARMBonus;

        // If the armour does not wholly negate the damage
        if (reduced > 0)
        {
            Debug.Log(source.unitName + " inflicted " + reduced + " damage to " + target.unitName);
            target.character.ReduceHealth(reduced);
            return true; // We successfully damaged the target
        }
        // We failed to break through the target's defenses
        return false;
    }

    /// <summary>
    /// CompareEVA
    /// </summary>
    /// <param name="roll"></param>
    /// <returns></returns>
    private bool CompareEVA(int roll)
    {
        if (roll > EVABonus) { return true; } // We have successfully hit
        // Else, we've missed
        return false;
    }

    /// <summary>
    /// The Effects_X() functions search for passive and status effects on both the
    /// source and target units that might affect the spell at a specific point.
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
    /// The GrabX() functions find the stat value beloinging to either the source or target unit
    /// </summary>
    private void GrabATK()
    {
        source.character.parameterContainer.Get(source.SplCrft, out ATKBonus);
    }
    private void GrabEVA()
    {
        target.character.parameterContainer.Get(target.EVA, out EVABonus);
    }
    private void GrabARM()
    {
        target.character.parameterContainer.Get(target.ARM, out ARMBonus);
    }
    private void GrabMAN()
    {
        source.character.parameterContainer.Get(source.MAN, out MAN);
    }
    
    /// <summary>
    /// CheckResistance() compares the rank of the spell to the rank of the target's
    /// resistance ability. If it's higher, the spell breaks through; else, the spell
    /// is negated.
    /// </summary>
    /// <returns> bool flag signifying whether the spell was negated or not </returns>
    private bool CheckResistance()
    {
        if ((int)spell.rank > (int)resistance) { return true; }
        return false;
    }

    // Advantage Functions
    /// <summary>
    /// The ChangeXAdvantage() functions receive a Reroll enum and hand it, and their respective flag, to the CompareRerollFlag() function
    /// </summary>
    /// <param name="reroll"> Reroll flag we want to apply to a flag </param>
    public void ChangeATKAdvantage(Reroll reroll)
    {
        advantageATK = CompareRerollFlag(advantageATK, reroll);
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
    // Return methods
    public Spell ReturnSpell() { return spell; }
    public List<string> ReturnSpellTags() { return spellTags; }
    public int ReturnMAN() { return MAN; }
    public int ReturnDCBonus() { return DCBonus; }

    // Simple methods
    public void SetResistance(Rank rank) { resistance = rank; } // Used by Magic Resistance and similar skills
    public void SetARMBonus(int bonus) { ARMBonus += bonus; }
    public void SetEVABonus(int bonus) { EVABonus += bonus; }
    public void SetDMGBonus(int bonus) { DMGBonus += bonus; }
}
