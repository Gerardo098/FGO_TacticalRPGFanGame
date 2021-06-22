using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// A unit's passive effects are all handled by this script.
/// 
/// </summary>
public class UnitEffects : MonoBehaviour
{
    public CharacterManager charMan; // Reference to the character manager
    private List<AbilityEffect> StatusEffects; // List of status effects (poison, burned, etc)
    private List<AbilityEffect> PassiveEffects; // List of passive abilities (magic resistance, mad enhancement, etc)
    private List<AbilityEffect> TriggeredEffects; // List of other passive effects

    // Charisma flag - used explicitly by the rolling manager to know if the unit is under the effects of Charisma
    public bool charisma = false;

    /// <summary>
    /// 
    /// Init() initializes the lists
    /// 
    /// </summary>
    public void Init()
    {
        PassiveEffects = new List<AbilityEffect>();
        StatusEffects = new List<AbilityEffect>();
        TriggeredEffects = new List<AbilityEffect>();
    }
    
    /// <summary>
    /// 
    /// GrabServantPassives() handles
    /// 
    /// </summary>
    /// <param name="servant"> Servant in question </param>
    public void GrabServantPassives(Servant servant)
    {
        // If we had a Servant equipped before, remove its passive effects
        if (PassiveEffects.Count != 0) 
        { 
            foreach (AbilityEffect effect in PassiveEffects)
            {
                // Run the unequip trigger of each passive
                effect.UnequipEffect(charMan);
            }
            // Clear the list
            PassiveEffects.Clear();
        }
        // Add the new servant's passives to the unit
        foreach (AbilityEffect effect in servant.ReturnPassives())
        {
            AddEffect(effect);
        }
    }

    /// <summary>
    /// AddEffect() receives an effect and the source (if any).
    /// The effect in question is initialized and saved in the appropriate list
    /// </summary>
    /// <param name="effect"> Ability effect to be added to the unit </param>
    /// <param name="source"> source unit of the effect </param>
    public void AddEffect(AbilityEffect effect, CharacterManager source = null)
    {
        // TODO - check if effect is stackable

        // Initialize the effect
        effect.Init(charMan, source);
        // If the effect is an instant effect, don't save it and stop here
        if (effect.GetDuration() == StatusDuration.Instant) { return; }

        // Check the effect's class and save it in the appropriate list
        EffectClass effectClass = effect.GetEffectClass();
        switch (effectClass)
        {
            case EffectClass.PassiveAbility:
                PassiveEffects.Add(effect);
                return;
            case EffectClass.StatusEffect:
                StatusEffects.Add(effect);
                return;
            case EffectClass.TriggeredEffect:
                TriggeredEffects.Add(effect);
                return;
        }
    }
    private bool CompareEffects(AbilityEffect newEffect, List<AbilityEffect> effectList)
    {
        // If it's stackable, we can just return true;
        if (newEffect.GetStackFlag()) { return true; }

        System.Type type = newEffect.GetType();
        //AbilityEffect curEffect = effectList.Find(x => x == newEffect.GetType()); // Grab the old status

        return false;
    }

    /// <summary>
    /// RemoveEffect() as the name implies, receives an effect, activates its UnequipEffect() script,
    /// and finds where it's saved before removing it.
    /// </summary>
    /// <param name="effect"></param>
    public void RemoveEffect(AbilityEffect effect)
    {
        effect.UnequipEffect(charMan);
        EffectClass effectClass = effect.GetEffectClass();
        switch (effectClass)
        {
            case EffectClass.PassiveAbility:
                PassiveEffects.Remove(effect);
                return;
            case EffectClass.StatusEffect:
                StatusEffects.Remove(effect);
                return;
            case EffectClass.TriggeredEffect:
                TriggeredEffects.Remove(effect);
                return;
        }
    }

    /*
    public void AddEffect(StatusEffect effect)
    {
        if (effect.stackable)
        {
            StatusEffectsOLD.Add(effect);
            effect.StatusActivate(charMan);
        }

        // If the effect is NOT stackable, we check to see if the character is already under that effect.
        // If they are already under that effect, check to see if the new effect we're trying to apply is of higher Rank.
        // If our new effect is a stronger version, replace the old version.
        if (!effect.stackable)
        {
            effect.SetTarget(charMan);
            StatusEffect existing = effect.StatusCompare();
            if (existing != null)
            {
                RemoveEffect(existing);
                StatusEffectsOLD.Add(effect);
                effect.StatusActivate(charMan);
            }
        }
        StatusEffectsOLD.Add(effect);
        effect.StatusActivate(charMan);
        Debug.Log("# of effects that " + charMan.unitName + " is under: " + StatusEffectsOLD.Count);
    }
    */

    // Cleanup Methods
    /// <summary>
    /// Clean_StartOfTurn() first checks if the unit is under ANY effects before running the
    /// CheckAllLists function. This is the function used at the start of the turn.
    /// </summary>
    public void Clean_StartOfTurn()
    {
        // If the unit is under NO effects, just return
        int total = StatusEffects.Count + PassiveEffects.Count + TriggeredEffects.Count;
        if (total == 0) { return; }
        
        SOT_CheckAllLists();
    }

    /// <summary>
    /// SOT_CheckAllLists() looks through all non-empty lists and runs the TraverseEffectList() for
    /// each applicable list.
    /// </summary>
    private void SOT_CheckAllLists()
    {
        // Check each list individually
        if (StatusEffects.Count > 0)
        {
            SOT_TraverseEffectList(StatusEffects);
        }
        if (PassiveEffects.Count > 0)
        {
            SOT_TraverseEffectList(PassiveEffects);
        }
        if (TriggeredEffects.Count > 0)
        {
            SOT_TraverseEffectList(TriggeredEffects);
        }
    }

    /// <summary>
    /// SOT_TraverseEffectList() is a simple function that runs a for loop through a given list
    /// and checks each effect saved within.
    /// </summary>
    /// <param name="effectList"> A list of ability effects to look through </param>
    private void SOT_TraverseEffectList(List<AbilityEffect> effectList)
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            AbilityEffect status = effectList[i];
            bool flag = SOT_EffectCheck(status);
            // If we removed an effect, reduce i in the for loop by 1
            // This is to avoid skipping an effect
            if (flag) { i--; }
        }
    }

    /// <summary>
    /// SOT_EffectCheck() looks at a single ability effect and checks if it is 
    /// a start-of-the-turn effect and checks if it should be removed.
    /// </summary>
    /// <param name="status"> the ability effect to be reviewed </param>
    /// <returns> bool flag that tells SOT_TraverseEffectList() if an effect was removed </returns>
    private bool SOT_EffectCheck(AbilityEffect status)
    {
        // Grab ther duration enum of the status and check with a switch
        StatusDuration duration = status.GetDuration();
        switch (duration)
        {
            case StatusDuration.TargetNextTurn:
                // Grab the target from the effect's info
                CharacterManager target = status.GetTarget();
                // If the target is the same as the character, remove the effect
                if (target == charMan) 
                { 
                    RemoveEffect(status);
                    return true;
                }
                return false;

            case StatusDuration.StartSaveEnds:
                // Create a new EffectInstance
                EffectInstance checkDC = new EffectInstance(status, charMan);
                // Rather than re-handling the effect entirely, we simply check the DC roll
                bool flag = checkDC.ActivateSOTInstance();
                // If the flag is false, we remove the effect
                if (!flag) 
                { 
                    RemoveEffect(status);
                    return true;
                }
                return false;

            case StatusDuration.TurnCount:
                // If the status has outlived its expected lifetime, remove it here
                if (status.TurnCountChange()) 
                { 
                    RemoveEffect(status);
                    return true;
                }
                return false;

            // If it's a different status duration, skip it
            default:
                return false;
        }
    }

    /// <summary>
    /// Clean_EndOfTurn() first checks if the unit is under ANY effects before running the
    /// CheckAllLists function. This is the function used at the end of the turn.
    /// </summary>
    public void Clean_EndOfTurn()
    {
        // If the unit is under NO effects, just return
        int total = StatusEffects.Count + PassiveEffects.Count + TriggeredEffects.Count;
        if (total == 0) { return; }

        EOT_CheckAllLists();
    }

    /// <summary>
    /// SOT_CheckAllLists() looks through all non-empty lists and runs the TraverseEffectList() for
    /// each applicable list.
    /// </summary>
    private void EOT_CheckAllLists()
    {
        // Check each list individually
        if (StatusEffects.Count > 0)
        {
            EOT_TraverseEffectList(StatusEffects);
        }
        if (PassiveEffects.Count > 0)
        {
            EOT_TraverseEffectList(PassiveEffects);
        }
        if (TriggeredEffects.Count > 0)
        {
            EOT_TraverseEffectList(TriggeredEffects);
        }
    }

    /// <summary>
    /// SOT_TraverseEffectList() is a simple function that runs a for loop through a given list
    /// and checks each effect saved within.
    /// </summary>
    /// <param name="effectList"> A list of ability effects to look through </param>
    private void EOT_TraverseEffectList(List<AbilityEffect> effectList)
    {
        for (int i = 0; i < effectList.Count; i++)
        {
            AbilityEffect status = effectList[i];

            StatusDuration duration = status.GetDuration();
            if (duration == StatusDuration.EndSaveEnds)
            {
                // Create a new EffectInstance
                EffectInstance checkDC = new EffectInstance(status, charMan);
                // Rather than re-handling the effect entirely, we simply check the DC roll
                bool flag = checkDC.ActivateSOTInstance();
                if (flag)
                {
                    RemoveEffect(status);
                    i--;
                }
            }
        }
    }

    /// <summary>
    /// Clean_EffectsBySource() is called at the start of this unit's turn to check for any
    /// effect that other units might have that this unit is the source of.
    /// We then check if any other units' effect would deactivate at the start of *this* unit's turn.
    /// </summary>
    public void Clean_EffectsBySource()
    {
        // Grab the list of all units on the field
        List<CharacterManager> UnitsOnField = charMan.master.ReturnUnitList();
        foreach (CharacterManager unit in UnitsOnField)
        {
            // For that unit, check their lists of effects
            unit.unitEffects.CheckSource(charMan);
        }
    }

    /// <summary>
    /// CheckSource() receives a CharacterManager, and we check all lists to see if any effect
    /// has the received character is their source.
    /// </summary>
    /// <param name="_source"> The source unit we're looking for in the effects </param>
    public void CheckSource(CharacterManager _source)
    {
        // If the unit is under NO effects, just return
        int total = StatusEffects.Count + PassiveEffects.Count + TriggeredEffects.Count;
        if (total == 0) { return; }

        // Check each list individually
        // If a list is empty, skip it
        if (StatusEffects.Count > 0) 
        {
            SourceUnitEffectCheck(_source, StatusEffects);
        }
        if (PassiveEffects.Count > 0)
        {
            SourceUnitEffectCheck(_source, PassiveEffects);
        }
        if (TriggeredEffects.Count > 0)
        {
            SourceUnitEffectCheck(_source, TriggeredEffects);
        }
    }

    /// <summary>
    /// SourceUnitEffectCheck() checks a given list of ability effects for a given source unit.
    /// If an effect is of the "SourceNextTurn" duration, and its source is the given source, we remove it.
    /// </summary>
    /// <param name="_source"> The source unit we're looking for in the effects </param>
    /// <param name="effectList"> The list of effects we're looking through </param>
    private void SourceUnitEffectCheck(CharacterManager _source, List<AbilityEffect> effectList)
    {
        // For loop to allow us to traverse the list and remove items from it if needed
        for (int i = 0; i < effectList.Count; i++)
        {
            // Grab the effect and its duration
            AbilityEffect status = effectList[i];
            StatusDuration duration = status.GetDuration();
            // If the duration is specifically "SourceNextTurn", check the effect's source
            if (duration == StatusDuration.SourceNextTurn)
            {
                CharacterManager source = status.GetSource();
                if (source == _source)
                {
                    // If we found a reference to our given source, remove the effect
                    RemoveEffect(status);
                    i--; // Decrease the i in the for loop by 1 to avoid skipping any effect in the list
                }
            }
        }
    }

    /// <summary>
    /// Return functions that return a specific list of ability effects
    /// </summary>
    /// <returns> list of abilityeffect instances </returns>
    public List<AbilityEffect> ReturnStatuses() { return StatusEffects; }
    public List<AbilityEffect> ReturnPassives() { return PassiveEffects; }
    public List<AbilityEffect> ReturnTriggered() { return TriggeredEffects; }

    // Triggers ===========================================================================

    /// <summary>
    /// SearchPassives is called upon by an AbilityInstance subclass instance to check for any
    /// effect that might alter said instance, such as a unit having a special resistance against fire attacks or the like.
    /// We give the function a Timing enum as a variable because every time we call this function, we only want to find effects
    /// that trigger at a specific point.
    /// </summary>
    /// <param name="turnTiming"> The turn timing we're looking to trigger </param>
    /// <param name="instance"> instance, if any </param>
    public void SearchPassives(Timing turnTiming, AbilityInstance instance = null)
    {
        // Check the unit's status effects and the like first;
        CheckTiming(StatusEffects, turnTiming, instance);
        CheckTiming(PassiveEffects, turnTiming, instance);
        CheckTiming(TriggeredEffects, turnTiming, instance);

        // Then we can check their gear, if any
        Weapon mainWeapon = charMan.equipment.ReturnMainWeapon();
        Weapon offWeapon = charMan.equipment.ReturnOffWeapon();
        Armour armour = charMan.equipment.ReturnArmour();

        // If there's no equipment, we can stop here
        if (mainWeapon == null && offWeapon == null && armour == null) { return; }

        // Else, we go through the effects of all non-null pieces of equipment
        List<AbilityEffect> equipmentEffects; // List to store an equipment piece's effects for the time being

        if (mainWeapon != null)
        {
            equipmentEffects = mainWeapon.weaponEffects;
            CheckTiming(equipmentEffects, turnTiming, instance);
        }
        if (offWeapon != null)
        {
            equipmentEffects = offWeapon.weaponEffects;
            CheckTiming(equipmentEffects, turnTiming, instance);
        }
        if (armour != null)
        {
            equipmentEffects = armour.armourEffects;
            CheckTiming(equipmentEffects, turnTiming, instance);
        }
    }

    /// <summary>
    /// CheckTiming() is a large switch case check that selects a search function of 
    /// specific type depending on the given timing enum flag.
    /// </summary>
    /// <param name="effects"> List of effects we're going to look through </param>
    /// <param name="turnTiming"> Timing enum used to select the specific search function </param>
    /// <param name="instance"> ability instance, if any </param>
    private void CheckTiming(List<AbilityEffect> effects, Timing turnTiming, AbilityInstance instance = null)
    {
        switch (turnTiming)
        {
            // On Acquisition
            case Timing.OnEquip:
                Search_OnEquip(effects);
                return;
            case Timing.OnUnequip:
                Search_OnUnequip(effects);
                return;

            // Turn Phase Timing
            case Timing.RoundStart:
                Search_StartRound(effects);
                return;
            case Timing.RoundEnd:
                Search_EndRound(effects);
                return;
            case Timing.TurnStart:
                Search_StartTurn(effects);
                return;
            case Timing.TurnEnd:
                Search_EndTurn(effects);
                return;

            // On Attack
            case Timing.OnAttack_Source:
                Search_OAS(effects, instance);
                return;
            case Timing.OnAttack_Target:
                Search_OAT(effects, instance);
                return;

            // On Successful Strike
            case Timing.OnHit_Source:
                Search_OHS(effects, instance);
                return;
            case Timing.OnHit_Target:
                Search_OHT(effects, instance);
                return;

            // On Damage
            case Timing.OnDamage_Source:
                Search_ODS(effects, instance);
                return;
            case Timing.OnDamage_Target:
                Search_ODT(effects, instance);
                return;

            // Default
            default:
                return;
        }
    }

    /// <summary>
    /// SearchPassives() is a function that looks through all effects for those that 
    /// contain a trigger of the given type. If one is found, that trigger is activated.
    /// 
    /// NOTE: At the moment, this function is undergoing testing.
    /// </summary>
    /// <param name="type"> Type of instance we're looking for </param>
    /// <param name="instance"> Ability instance to change, if any is given </param>
    private void SearchPassives_Test(Type type, AbilityInstance instance = null)
    {
        foreach (AbilityEffect ability in PassiveEffects)
        {
            foreach (EffectTrigger trigger in ability.triggers)
            {
                if (trigger.GetType() == type) { trigger.Activate(ability, instance); }
            }
        }
    }

    /// <summary>
    /// All effect Search_X() functions here are pretty much the same - look through a given list,
    /// find all triggers of a certain type and activate the ones we find.
    /// The OnEquip and OnUnqeuip ones are different due to their seperate activate() function.
    /// The others require a non-null AbilityInstance instance in order to activate, else they are unable
    /// to change anything on their own.
    /// </summary>
    /// <param name="effects"></param>

    // On Acquisition
    private void Search_OnEquip(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnEquip trigger = ability.onEquip;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }

        Weapon weapon = charMan.equipment.ReturnMainWeapon();
    }
    private void Search_OnUnequip(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnUnequip trigger = ability.onUnequip;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }

    // Turn Phases
    private void Search_StartRound(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnRoundStart trigger = ability.roundStart;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_EndRound(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnRoundEnd trigger = ability.roundEnd;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_StartTurn(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnTurnStart trigger = ability.turnStart;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_EndTurn(List<AbilityEffect> effects)
    {
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnTurnEnd trigger = ability.turnEnd;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }

    // On Attack
    private void Search_OAS(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnAttack_Source trigger = ability.onAttackSource;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_OAT(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnAttack_Source trigger = ability.onAttackSource;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }

    // On Hit
    private void Search_OHS(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnHit_Source trigger = ability.onHitSource;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_OHT(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnHit_Target trigger = ability.onHitTarget;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }

    // On Damage
    private void Search_ODS(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnDamage_Source trigger = ability.onDamageSource;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }
    private void Search_ODT(List<AbilityEffect> effects, AbilityInstance instance)
    {
        if (instance == null) { return; }
        foreach (AbilityEffect ability in effects)
        {
            Trigger_OnDamage_Target trigger = ability.onDamageTarget;
            // If the ability has a non-null trigger here, activate it
            if (trigger != null) { trigger.Activate(ability); }
        }
    }

    // Trigger helper functions
    // ...
    /*
     * Object.GetType()
     * stackoverflow.com/questions/18279663/get-the-class-name-from-an-object-variable
     * https://stackoverflow.com/questions/3561202/check-if-instance-is-of-a-type
     */


    // SPECIFIC SKILL FUNCTIONS ======================================================
    // Ordered Alphabetically

    // CHARISMA =========================================

    /// <summary>
    /// CharismaCheck() finds and returns the Charisma status effect that the unit is affected by
    /// </summary>
    /// <returns> Instance of Status_Charisma </returns>
    internal Status_Charisma CharismaCheck()
    {
        // Grab the instance of Charisma this unit is under
        Status_Charisma check = (Status_Charisma)StatusEffects.Find(x => x == (Status_Charisma)x);
        return check; // Return it
    }


    // ===========================================================================================
    // OLD FUNCTIONS - KEPT HERE FOR LATER

    /*

    public void AddEffect(StatusEffect effect)
    {
    if (effect.stackable)
    {
        StatusEffectsOLD.Add(effect);
        effect.StatusActivate(charMan);
    }

    // If the effect is NOT stackable, we check to see if the character is already under that effect.
    // If they are already under that effect, check to see if the new effect we're trying to apply is of higher Rank.
    // If our new effect is a stronger version, replace the old version.
    if (!effect.stackable)
    {
        effect.SetTarget(charMan);
        StatusEffect existing = effect.StatusCompare();
        if (existing != null)
        {
            RemoveEffect(existing);
            StatusEffectsOLD.Add(effect);
            effect.StatusActivate(charMan);
        }
    }
    StatusEffectsOLD.Add(effect);
    effect.StatusActivate(charMan);
    Debug.Log("# of effects that " + charMan.unitName + " is under: " + StatusEffectsOLD.Count);
    }


    */
}

