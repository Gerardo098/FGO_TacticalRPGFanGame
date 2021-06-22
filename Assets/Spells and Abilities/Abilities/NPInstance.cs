using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPInstance is an abstract subclass of the ability instance class.
/// Like the other inheritors of AbilityInstance, NPInstance handles the effects of a given
/// Noble Phantasm on a given target.
/// Unlike the other subclasses however, NPInstance is abstract; the reason for this is that 
/// NPs are too varied in effect and form, thus each NP has their own subclass dedicated to their own
/// effect that this class assists in handling.
/// </summary>
public abstract class NPInstance : AbilityInstance
{
    // Flags
    protected bool DamageDealing = false; // The NP deals damage
    protected bool IgnoresArmour = false; // The damage ignores armour
    protected bool DCCleared; // Whether the target's saving throw beat the DC or not

    // Bonuses and Penalties
    // Source bonuses
    protected int Damage = 0;
    // Target bonuses
    protected int BonusARM = 0;
    protected int BonusST = 0;

    // Advantage + Disadvantage flags
    protected Reroll advantageDMG = Reroll.No;
    protected Reroll advantageHeal = Reroll.No;

    // Tags
    protected List<string> NPTags;

    /// <summary>
    /// HandeNP() is the function called by the target's CharManager script to 
    /// go through a Noble Phantasm ability in an assembly line fashion.
    /// Each subclass of NPInstance has their own version of this function.
    /// </summary>
    public abstract void HandleNP();

    /// <summary>
    /// Effects_OnNP() searches through the target and source units' 
    /// passive and status effects to see if any of those would alter the outcome
    /// of this instance.
    /// </summary>
    protected void Effects_OnNP()
    {
        source.unitEffects.SearchPassives(Timing.OnAttack_Source, this);
        target.unitEffects.SearchPassives(Timing.OnAttack_Target, this);
    }

    public void SetDamage(int _damage = 0) { Damage = _damage; }
    public void SetDCFlag(bool flag = false) { DCCleared = flag; }
    public void AddBonusSavingThrow(int ST = 0) { BonusST += ST; }

    // Return Methods
    public List<string> ReturnNPTags() { return NPTags; }
    public int ReturnBonusST() { return BonusST; }

    /// <summary>
    ///  The ChangeXAdvantage() functions handle any changes to their 
    ///  respective reroll flags, such as their saving throw or any heal effect.
    /// </summary>
    /// <param name="reroll"> Enum flag to dictate whether a reroll is allowed or not </param>
    public void ChangeDMGAdvantage(Reroll reroll)
    {
        advantageDMG = CompareRerollFlag(advantageDMG, reroll);
    }
    public void ChangeHealAdvantage(Reroll reroll)
    {
        advantageHeal = CompareRerollFlag(advantageHeal, reroll);
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
}
