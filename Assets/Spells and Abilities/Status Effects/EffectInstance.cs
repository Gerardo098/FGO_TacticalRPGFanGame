using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EffectInstance is the subclass of AbilityInstance used by all effects and abilities.
/// Rather than attacks and spells, this one focuses mostly on whether a unit aquires an
/// effect, such as poison or paralysis, from another instance.
/// </summary>
public class EffectInstance : AbilityInstance
{
    protected AbilityEffect abilityEffect; // Effect in question
    protected List<string> effectSourceTags; // Effect tags
    protected Rank effectRank; // Effect rank

    protected Parameter parameter; // Parameter used for a saving throw, if applicable
    protected bool DCFlag; // Bool flag dictating whether the effect requires a saving throw or not
    protected int DC; // Effect's DC score
    protected int STBonus; // Saving throw bonus allowed to the target; can be negative
    protected Reroll advantageDC = Reroll.No; // Saving throw reroll allowed to the target

    // Specific variables for specific abilities
    // Healing
    protected int healBonus; // Heal bonus allowed to the source; can be negative
    protected Reroll advantageHeal = Reroll.No; // Heal reroll allowed to the source

    // Functions -------------------------------------------
    
    /// <summary>
    /// 
    /// This function is used to create an instance of EffectInstance.
    /// We hand it the necessary information to get started
    /// 
    /// </summary>
    /// <param name="_status"> Ability effect to be applied to target </param>
    /// <param name="_target"> target of the ability </param>
    /// <param name="_source"> source of the ability, if any </param>
    public EffectInstance
        (AbilityEffect _status, CharacterManager _target, CharacterManager _source = null)
    {
        // Save the target and source of the effect
        target = _target;
        source = _source;
        // If the source is null, assume it's a self-inflicted effect
        if (source == null) { source = target; }

        // Save the effect, its tags, and rank
        abilityEffect = _status;
        effectSourceTags = abilityEffect.GetTags();
        rank = abilityEffect.GetCurrentRank();
        // Initialize the STBonus int
        STBonus = 0;
    }

    /// <summary>
    /// 
    /// ActivateInstance() initializes the assembly line process for the effect instance.
    /// 
    /// </summary>
    public void ActivateInstance()
    {
        // Check for any units that would trigger prior to the effect hitting
        Effects_ToHit(); 

        // Find if the effect has a DC score to beat or not
        DCFlag = abilityEffect.GetDCFlag();

        
        // If true, handle checking the DC
        if (DCFlag)
        {
            // Check if the DC has been beaten
            bool flag = CheckDC();
            // If true, apply the effects on the target
            if (flag)
            {
                Effects_OnHit();
                target.unitEffects.AddEffect(abilityEffect, source);
            }
            // If false, let the player know the unit avoided the effect
            else { Debug.Log(target.unitName + " successfully avoided the effect!"); }
        }

        // If false, just apply the effect directly to the target
        else
        {
            Effects_OnHit();
            target.unitEffects.AddEffect(abilityEffect, source);
        }
    }

    /// <summary>
    /// 
    /// CheckDC() handles the reading of the ability's DC score, 
    /// the stats necessary to roll against is, and the roll itself.
    /// 
    /// </summary>
    /// <returns> CheckDC() returns true if the DC is beaten; false if not </returns>
    private bool CheckDC()
    {
        // Grab the DC and parameter
        DC = abilityEffect.GetDC();
        parameter = abilityEffect.GetParameter();

        // Grab the target unit's score in this parameter
        if (parameter != null) { STBonus += target.character.GetParameter(parameter); }

        // Make the roll and compare
        int roll = SavingThrowRoll();
        bool hitFlag = CompareDC(roll);

        // Return the outcome
        return hitFlag;
    }

    /// <summary>
    /// 
    /// SavingThrowRoll() calls on the Rolling manager to handle the roll.
    /// Also checks whether the unit has a reroll in it or not
    /// 
    /// </summary>
    /// <returns> return the result of the roll </returns>
    private int SavingThrowRoll()
    {
        // Check for advantage / disadvantage on the roll
        switch (advantageDC)
        {
            // Depending on the case, add advantage or disadvanatge
            case Reroll.Advantage:
                return RollingManager.BasicRoll(source, STBonus, Reroll.Advantage);
            case Reroll.Disadvantage:
                return RollingManager.BasicRoll(source, STBonus, Reroll.Disadvantage);

            // If no rerolls, roll normally
            case Reroll.No:
            default:
                return RollingManager.BasicRoll(source, STBonus);
        }
    }

    /// <summary>
    /// 
    /// CompareDC() takes an int and compares it to the effect's DC.
    /// If higher, it has been avoided/resisted.
    /// If lower or equal, it is imposed on the target
    /// 
    /// </summary>
    /// <param name="roll"> Int variable of the unit's roll </param>
    /// <returns> Bool flag signaling whether the DC was beaten or not </returns>
    private bool CompareDC(int roll)
    {
        if (roll > DC) { return true; } // We have successfully defended against the effect
        // Else, we've failed to defend ourselves
        return false;
    }

    /// <summary>
    /// 
    /// Effects_ToHit() and Effects_OnHit() search through the target and source units' 
    /// passive and status effects to see if any of those would alter the outcome.
    /// 
    /// </summary>
    private void Effects_ToHit()
    {
        target.unitEffects.SearchPassives(Timing.OnAttack_Target, this);
        source.unitEffects.SearchPassives(Timing.OnAttack_Source, this);
    }
    private void Effects_OnHit()
    {
        target.unitEffects.SearchPassives(Timing.OnHit_Target, this);
        source.unitEffects.SearchPassives(Timing.OnHit_Source, this);
    }

    /// <summary>
    /// 
    /// ActivateSOTInstance() can be considered a "mini" activation used by effects
    /// that allow their host another attempt against its DC at the start of their turn.
    /// 
    /// </summary>
    /// <returns> Bool flag signaling whether the DC was beaten or not </returns>
    public bool ActivateSOTInstance()
    {
        DC = abilityEffect.GetDC();
        source.unitEffects.SearchPassives(Timing.OnAttack_Source, this);

        bool flag = CheckDC();
        // If we return true, we keep the effect
        if (flag) { return true; }

        return false;
    }

    // Helper functions ===============================

    /// <summary>
    /// 
    ///  The ChangeXAdvantage() functions handle any changes to their 
    ///  respective reroll flags, such as their saving throw or any heal effect.
    ///  
    /// </summary>
    /// <param name="reroll"> Enum flag to dictate whether a reroll is allowed or not </param>
    public void ChangeDCAdvantage(Reroll reroll)
    {
        advantageDC = CompareRerollFlag(advantageDC, reroll);
    }
    public void ChangeHealAdvantage(Reroll reroll)
    {
        advantageHeal = CompareRerollFlag(advantageHeal, reroll);
    }

    /// <summary>
    /// 
    /// CompareRerollFlag() checks the given reroll enum flag and compares it
    /// to the current flag. A decision is made depending on what both are set to.
    /// 
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

    // Return functions
    public AbilityEffect ReturnPassive() { return abilityEffect; }
    public void ReturnHealingInfo(out int bonus, out Reroll reroll) 
    {
        bonus = healBonus;
        reroll = advantageHeal;
    }

}
