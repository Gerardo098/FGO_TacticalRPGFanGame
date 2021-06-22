using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// EffectTrigger is an abstract "root" class serving as the basis for all other trigger subclasses.
/// 
/// </summary>
public abstract class EffectTrigger : ScriptableObject
{
    // Inherited variables
    private Rank rank;

    // Functions

    /// <summary>
    /// 
    /// SetRank() receives a Rank enum flag and saves it as the trigger's current rank.
    /// 
    /// </summary>
    /// <param name="_rank"> Rank enum to be used </param>
    public void SetRank(Rank _rank) { rank = _rank; }

    /// <summary>
    /// 
    /// GetRank() is a simple return function that returns the trigger's rank
    /// 
    /// </summary>
    /// <returns> Return the trigger's Rank enum </returns>
    public Rank GetRank() { return rank; }

    // Abstract functions

    /// <summary>
    /// 
    /// RankRead() would find and set a trigger's effects and power to match their current rank.
    /// Higher ranks result in stronger effects
    /// 
    /// </summary>
    /// <param name="rank"> Rank to be used for the sake of reading effects </param>
    public abstract void RankRead(Rank rank);

    /// <summary>
    /// 
    /// Activate() is the global function that's called whenever a "trigger" would occur.
    /// When run, the trigger will perform its specific functions.
    /// 
    /// </summary>
    /// <param name="effect"> Effect that the trigger is tied to </param>
    /// <param name="instance"> Ability instance that triggered the function, if applicable </param>
    /// <returns></returns>
    public abstract bool Activate(AbilityEffect effect, AbilityInstance instance = null);
}

// Equip triggers
public abstract class Trigger_OnEquip : EffectTrigger 
{
    /// <summary>
    /// 
    /// An overload of the above Activate() function used exclusively by the OnEquip and OnUnequip triggers.
    /// 
    /// </summary>
    /// <param name="effect"> Effect that the trigger is tied to </param>
    /// <param name="unit"> "Target" for the trigger's effects </param>
    /// <returns></returns>
    public abstract bool Activate(AbilityEffect effect, CharacterManager unit = null);
}
public abstract class Trigger_OnUnequip : EffectTrigger
{
    public abstract bool Activate(AbilityEffect effect, CharacterManager unit = null);
}

// Round + Turn Based Triggers
public abstract class Trigger_OnRoundStart : EffectTrigger{}
public abstract class Trigger_OnRoundEnd : EffectTrigger{}
public abstract class Trigger_OnTurnStart : EffectTrigger{}
public abstract class Trigger_OnTurnEnd: EffectTrigger{}

// On Attack - prior to the target being hit
public abstract class Trigger_OnAttack_Source : EffectTrigger{}
public abstract class Trigger_OnAttack_Target : EffectTrigger{}

// On Hit - the target has been hit but before damage is dealt
public abstract class Trigger_OnHit_Target : EffectTrigger{}
public abstract class Trigger_OnHit_Source : EffectTrigger{}

// On Damage - the target has been damaged by our ability
public abstract class Trigger_OnDamage_Source : EffectTrigger{}
public abstract class Trigger_OnDamage_Target : EffectTrigger{}