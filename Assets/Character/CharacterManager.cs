using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// CharacterManager is a class that acts as a mediator between all of a unit's scripts.
/// This way, all other scripts can simply have a reference to this script saved and use it to find others, instead of
/// having to keep references of all others they need.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    // Name of the unit
    public string unitName;

    // Parameter references
    public Parameter STR; // Strength
    public Parameter END; // Endurance
    public Parameter AGL; // Agility
    public Parameter MAN; // Mana
    public Parameter LCK; // Luck
    public Parameter WLL; // Will
    
    // Sub Parameters
    public Parameter MOV; // Movement
    public Parameter ARM; // Armour
    public Parameter MPGen; // MP Generation
    public Parameter CRT; // Critical Threat
    public Parameter EVA; // Evasion

    // Attack Skills
    public Parameter Melee; // Melee attacks
    public Parameter Range; // Ranged attacks
    public Parameter SplCrft; // Spell attacks

    public NavMeshAgent navMeshAgent; // NavMeshAgent of the unit
    public Movement_Root movementScript; // Movement script

    public MasterControl master; // Master control

    // Other script references
    public AttackAction attackAction;
    public AbilityUseAction abilityUse;
    public Character_Root character;
    public SkillController skillController;
    public CastingAction castingAction;
    public GameObject HPBarCanvas;
    public HealthBar HPBar;
    public GameObject MPBarCanvas;
    public MPBar MPBar;
    public UnitEffects unitEffects;
    public Inventory inventory;
    public Equipment equipment;

    // MassInit() runs all the init functions of all our other scripts in one place
    public void MassInit(MasterControl _masterControl)
    {
        // Save the reference to master
        master = _masterControl;

        character.Init();
        inventory.Init();
        equipment.Init();
        skillController.InitLists();
        unitEffects.Init();

        // Equip our first Servant
        EquipUnitStats();
    }

    /// <summary>
    /// EquipUnitStats() makes sure that this unit has all their stats, weapons, and armour equipped.
    /// We always grab the 1st Servant in their list of Servants for this.
    /// </summary>
    public void EquipUnitStats()
    {
        // Find the Servant
        ServantInstance startStats = inventory.GetServant(0);
        /*
         * 1 - Add stats
         * 2 - Equip Servant + equip weapons + armour
         * 3 - Grab abilities + spells from the Servant and give them to unit
         * 4 - Remove item from the inventory
         */
        character.parameterContainer.Sum(startStats.servant.stats);
        equipment.EquipServant(startStats.servant);
        skillController.GrabActiveAbilities(startStats.servant);
        unitEffects.GrabServantPassives(startStats.servant);
        inventory.RemoveItem(startStats.servant);
    }

    /// <summary>
    /// AnchorUnit() calls the movement script's anchor position script
    /// </summary>
    public void AnchorUnit() { movementScript.AnchorPosition_SOT(); }

    /// <summary>
    /// AbilityInstanceRead() receives an ability instance, checks its type, and handles it
    /// accordingly.
    /// </summary>
    /// <param name="instance"> instance in quesiton </param>
    public void AbilityInstanceRead(AbilityInstance instance)
    {
        if (instance is AttackInstance) { ReadAttack((AttackInstance)instance); }
        if (instance is SpellInstance) { ReadSpell((SpellInstance)instance); }
        if (instance is NPInstance) { ReadNP((NPInstance)instance); }
        if (instance is EffectInstance) { ReadStatus((EffectInstance)instance); }
    }

    /// <summary>
    /// The ReadX() each handle their own instances subclasses.
    /// This is done from the perspective that THIS unit is the target of each instance
    /// </summary>
    /// <param> instance to be activated </param>
    private void ReadAttack (AttackInstance attack) { attack.ActivateAttackInstance(); }
    private void ReadSpell(SpellInstance instance) { instance.HandleSpell(); }
    private void ReadNP(NPInstance NP) { NP.HandleNP(); }
    private void ReadStatus(EffectInstance instance) { instance.ActivateInstance(); }
}
