using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC_Nobu_3ThousandWorlds is the class handling Oda Nobunaga's, the boss unit's, 
/// special attack, Three Thousand Worlds.
/// This is a version exclusive to the boss version of Nobunaga, separate from the
/// one the unit would have if they were in the player's party.
/// </summary>
[CreateAssetMenu(menuName = "Ability/Noble Phantasm/Three Thousand Worlds")]
public class NPC_Nobu_3ThousandWorlds : Ability
{
    // Masks to find specific units
    public LayerMask characterMask;
    public LayerMask obstacleMask;
    internal float AOERadius = 0;
    internal float IndividualRadius = 0;

    /// <summary>
    /// Activate() override
    /// </summary>
    /// <param name="charMan"> Unit reference calling this function </param>
    /// <param name="target"> target object in the scene, optional </param>
    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        // This override NEEDS a target. If there is none, return.
        if (target == null) { return; }

        RankRead(); // Find all necessary numbers first
        // Face the target
        LookAtTarget(charMan.transform, target.transform);

        // Grab the target's position
        Vector3 targetPos = target.transform.position;
        // Find all units around the target
        Collider[] colliders = Physics.OverlapSphere(targetPos, AOERadius, characterMask);
        
        foreach (Collider collider in colliders)
        {
            // Find that collider's position and direction
            Vector3 unitPos = collider.transform.position;
            Vector3 dirToTarget = (target.transform.position - unitPos);
            // If there is nothing inbetween the target spot and a unit within range,
            // then we pass them to the HandleAOE() function
            if (!Physics.Raycast(unitPos, dirToTarget, Mathf.Infinity, obstacleMask)) { HandleAOE(charMan, unitPos); }
        }
    }

    /// <summary>
    /// HandleAOE() creates an AbilityInstance of the NP and hands them out to each
    /// of the units caught in its AOE
    /// </summary>
    /// <param name="source"> this unit, the source of the NP </param>
    /// <param name="position"> position of the target object </param>
    private void HandleAOE(CharacterManager source, Vector3 position)
    {
        // OverlapSphere to check for units
        Collider[] colliders = Physics.OverlapSphere(position, IndividualRadius, characterMask);
        foreach (Collider collider in colliders)
        {
            CharacterManager target = collider.gameObject.GetComponent<CharacterManager>();
            // If the collider does not have a null CharManager script, we can
            // create the instance and hand it to them
            if (target != null)
            {
                TTW_Instance TTW = new TTW_Instance(source, target, this);
                target.AbilityInstanceRead(TTW);
            }
        }
    }

    /// <summary>
    /// Unimplemented
    /// </summary>
    public override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// NPEffect() receives a NoblePhantasm ability instance and sets the numbers within
    /// </summary>
    /// <param name="NP"> instance in question </param>
    public override void NPEffect(NPInstance NP)
    {
        TTW_Instance TTW = (TTW_Instance)NP;

        RankRead();
        DCRoll(TTW);
        DamageRoll(TTW);
    }

    /// <summary>
    /// DCRoll() rolls the saving throw for the instance's target unit
    /// </summary>
    /// <param name="TTW"> TTW ability instance </param>
    private void DCRoll(TTW_Instance TTW)
    {
        CharacterManager target = TTW.ReturnTarget(); // Grab the target
        // Find their total ST bonus
        int BonusST = target.character.GetAgility() + TTW.ReturnBonusST();
        // Roll the saving throw
        int savingThrow = RollingManager.BasicRoll(target, BonusST);
        // Compare the roll to the DC; set the flag to true if the former is greater
        if (savingThrow > DC) { TTW.SetDCFlag(true); }
    }

    /// <summary>
    /// DamageRoll() rolls damage for the NP against the target and saves it
    /// </summary>
    /// <param name="TTW"> TTW ability instance </param>
    private void DamageRoll(TTW_Instance TTW)
    {
        List<string> targetTags = TTW.ReturnTarget().character.characterTags;
        CharacterManager TTWSource = TTW.ReturnSource();

        // 3kWorlds works best against Divine units and units with the Riding ability
        if (targetTags.Contains("Divine") || targetTags.Contains("Rider"))
        {
            TTW.SetDamage(RollingManager.CustomRoll(TTWSource, dieAmount, dieSize, Reroll.Advantage));
        }
        // Normal damage against other units though
        else { TTW.SetDamage(RollingManager.CustomRoll(TTWSource, dieAmount, dieSize)); }
    }

    /// <summary>
    /// RankRead() sets up the ability's parameters and numbers up based on its
    /// current Rank.
    /// </summary>
    public override void RankRead()
    {
        // Things not reliant on rank are set here
        AOERadius = 10f;
        IndividualRadius = 5f;
        dieSize = 8;

        // Rank read switch
        switch (CurrentRank)
        {
            case Rank.E:
                dieAmount = 5;
                DC = 20;
                MPCost = 50;
                break;
            case Rank.D:
                dieAmount = 6;
                DC = 21;
                MPCost = 60;
                break;
            case Rank.C:
                dieAmount = 7;
                DC = 22;
                MPCost = 70;
                break;
            case Rank.B:
                dieAmount = 8;
                DC = 23;
                MPCost = 80;
                break;
            case Rank.A:
                dieAmount = 9;
                DC = 24;
                MPCost = 90;
                break;
            case Rank.EX:
                dieAmount = 10;
                DC = 25;
                MPCost = 100;
                break;
            // If we somehow have no rank, set everything to 0
            default:
                dieAmount = 0;
                dieSize = 0;
                DC = 0;
                MPCost = 0;
                break;
        }
    }
}

/// <summary>
/// TTW_Instance is a subclass of the NoblePhantasm ability instance used
/// exclusively by the ability Three Thousand Worlds.
/// </summary>
public class TTW_Instance : NPInstance
{
    // Reference to the abiluty
    private readonly Ability TTW;

    /// <summary>
    /// Creator function
    /// </summary>
    /// <param name="_source"> source of the ability (Nobu) </param>
    /// <param name="_target"> target unit affected by the ability </param>
    /// <param name="NP"> The ability </param>
    public TTW_Instance(CharacterManager _source, CharacterManager _target, Ability NP)
    {
        // Save the information
        source = _source;
        target = _target;
        TTW = NP;

        // Grab the units' and the ability's tags
        sourceUnitTags = source.character.characterTags;
        targetUnitTags = target.character.characterTags;
        NPTags = TTW.tags;
    }

    /// <summary>
    /// HandleNP() calls all functions that are triggered when the NP hits
    /// </summary>
    public override void HandleNP()
    {
        Effects_OnNP();
        TTW.NPEffect(this);
        InflictDamage();
    }

    /// <summary>
    /// InflictDamage() takes the damage amount calculated by the DamageRoll()
    /// function and inflicts it upon the target.
    /// </summary>
    internal void InflictDamage()
    {
        if (DCCleared) { Damage /= 2; }
        int ARM = target.character.GetArmour() + BonusARM;
        int total = Damage - ARM;
        if (total > 0) { target.character.ReduceHealth(total); }
    }
}