using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbilityEffect is the abstract class used by all passive effects, such as passive
/// abilities and status effects.
/// We also save all information regarding their duration, difficulty check, range, and all
/// other necessary information, depending on the effect in quesiton.
/// </summary>
public abstract class AbilityEffect : ScriptableObject
{
    public string effectName; // Name of the ability
    public TargetType targetType; // Whether the ability is friendly, hostile, or neutral.
    [SerializeField]
    protected List<string> tags; // Effect tags, used by other effects

    // Rank - how strong the skill is
    [SerializeField]
    protected Rank OriginalRank; // The ability's OG rank
    protected Rank CurrentRank; // The ability's current rank, if changed from the OG

    // Stuff handled by each effect individually
    protected EffectClass effect; // the type of ability effect we're working with
    protected StatusDuration duration; // How long the effect lasts
    protected int turnCount; // # of turns the effect lasts (if counting turns)
    protected bool stackable; // If an affected unit can be affected by multiple instances of this effect
    protected bool DCFlag; // If this effect calls upon a saving throw vs DC check
    protected Parameter parameter; // Parameter used if a saving throw is required
    protected float range; // Range of the effect

    // Source and target for the ability
    protected CharacterManager source;
    protected CharacterManager target;


    // Numbers - usually handled by a effect's Rank Read function
    protected int bonus;
    protected int penalty; 
    protected int dieAmount;
    protected int dieSize;
    protected int DC;

    // All Triggers
    public List<EffectTrigger> triggers;

    public Trigger_OnEquip onEquip;
    public Trigger_OnUnequip onUnequip;

    public Trigger_OnRoundStart roundStart;
    public Trigger_OnRoundEnd roundEnd;
    public Trigger_OnTurnStart turnStart;
    public Trigger_OnTurnEnd turnEnd;

    public Trigger_OnAttack_Source onAttackSource;
    public Trigger_OnAttack_Target onAttackTarget;

    public Trigger_OnHit_Source onHitSource;
    public Trigger_OnHit_Target onHitTarget;

    public Trigger_OnDamage_Source onDamageSource;
    public Trigger_OnDamage_Target onDamageTarget;

    // Functions =====================================================================

    /// <summary>
    /// Init() - the function is called when the effect is applied to a target unit
    /// </summary>
    /// <param name="_target"> target unit the effect is applied to </param>
    /// <param name="_source"> source unit of the effect, if any </param>
    public void Init(CharacterManager _target, CharacterManager _source = null) 
    {
        // When initialized, the effect's current rank equals the OG rank
        CurrentRank = OriginalRank;
        // If the source is null, then we assume it's a self-applied effect
        if (_source == null) { _source = _target; }
        target = _target;
        source = _source;
        // Activate the RankRead() function
        RankRead();
        // Activate the InternalInit()
        DCFlag = false; // Just to avoid having it un-initialized
        InternalInit();
        // Activate the OnEquip trigger, if able
        EquipEffect(target);
    }

    /// <summary>
    /// TurnCountChange() is called when we're dealing with an effect that lasts a certain
    /// number of turns. Also checks if the effect has run out of time or not.
    /// </summary>
    /// <param name="amount"> Amount of "turns" we reduce the effect's duration by </param>
    /// <returns> bool flag if the effect will remain (true) or not (false) </returns>
    public bool TurnCountChange(int amount = 1)
    {
        // Reduce the turn count by the amount
        turnCount -= amount;
        // If the turn count is 0 or less
        if (turnCount <= 0)
        {
            // The effect has reached the end of its life cycle
            return true;
        }
        return false; // Else, it will continue
    }

    // Return functions
    public string GetName() { return effectName + " [" + CurrentRank + "]"; }
    public int GetBonus() 
    {
        RankRead();
        return bonus;
    }
    internal bool GetDCFlag() { return DCFlag; }
    internal bool GetStackFlag() { return stackable; }
    public Parameter GetParameter()
    {
        if (parameter != null) { return parameter; }
        else { return null; }
    }
    public int GetDC() 
    {
        RankRead();
        return DC; 
    }
    public void GetDice(out int size, out int amount)
    {
        size = dieSize;
        amount = dieAmount;
    }
    public CharacterManager GetTarget() { return target; }
    public CharacterManager GetSource() { return source; }
    public float GetRange() { return range; }
    public Rank GetCurrentRank() { return CurrentRank; }
    public Rank GetOriginalRank() { return OriginalRank; }
    public EffectClass GetEffectClass() { return effect; }
    public List<string> GetTags() { return tags; }
    public StatusDuration GetDuration() { return duration; }

    // Simple set-up functions
    public void SetOgRank(Rank newRank) { OriginalRank = newRank; }
    public void SetCurrentRank(Rank newRank) { CurrentRank = newRank; }

    public void EquipEffect(CharacterManager unit)
    {
        // Check if the effect has a OnEquip trigger and activate it
        if (onEquip != null) { onEquip.Activate(this, unit); }
    }
    public void UnequipEffect(CharacterManager unit)
    {
        // Check if the effect has a OnUnequip trigger and activate it
        if (onUnequip != null) { onUnequip.Activate(this, unit); }
    }

    // Abstracts
    public abstract void RankRead();
    public abstract void InternalInit();
}