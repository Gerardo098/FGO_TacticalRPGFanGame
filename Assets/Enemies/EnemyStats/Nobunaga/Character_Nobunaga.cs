using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character_Nobunaga is the character subclass used by the boss unit, Oda Nobunaga
/// </summary>
public class Character_Nobunaga : Character_Root
{
    // Layer masks used to find legal targets for our attacks+abilities
    public LayerMask characterMask;
    public LayerMask obstacleMask;

    // Actions state enum
    private enum State { Idle, Moving, Melee, Ranged, UseAbility, NoblePhantasm }

    // Lists of targets for various abilities
    private List<CharacterManager> UnitsOnField;
    private List<CharacterManager> MeleeTargets;
    private List<CharacterManager> RangedTargets;
    private List<CharacterManager> CloseAllies;
    // Specific target for this unit's special attack
    private CharacterManager NPTarget;
    private State state = State.Idle; // Action state flag

    // Script references
    private UseAbility_Enemy useAbility;
    private Attack_Enemy attack;

    // Max and min range for this unit's NP ability
    private readonly float NPMax = 99f;
    private readonly float NPMin = 10f;
    // Whether this unit can activate their Charisma ability or not
    private bool CharismaFlag = false;

    /// <summary>
    /// Init() override
    /// </summary>
    public override void Init()
    {
        // Init stats
        InitValues();
        InitFormulas();

        // Create new lists
        UnitsOnField = new List<CharacterManager>();
        MeleeTargets = new List<CharacterManager>();
        RangedTargets = new List<CharacterManager>();
        CloseAllies = new List<CharacterManager>();

        // Find the useAbility and attack scripts
        useAbility = (UseAbility_Enemy)charMan.abilityUse;
        attack = (Attack_Enemy)charMan.attackAction;
    }

    /// <summary>
    /// In the Update() function, we continually check the state that the 
    /// unit is in, and change it according to the readings we get from other
    /// functions.
    /// </summary>
    private void Update()
    {
        // If we're not the turn unit, don't do anything
        if (!turnUnit) { return; }

        // Switch case checking the state flag
        // Perform an action depending on the enum state
        switch (state)
        {
            // If idle, check for enemies within range
            case State.Idle:
                // TODO = Rather than looking for a NP target every update
                // Only do it when our current MP is at an acceptable amount
                CheckState();
                if (CharismaFlag) { ActivateCharisma(); }
                break;

            // If attacking, find a suitable target
            // Melee attack
            case State.Melee:
                MeleeAttack();
                state = State.Idle;
                break;
            // Ranged attack
            case State.Ranged:
                RangedAttack();
                state = State.Idle;
                break;

            // If moving, enable the movement script
            case State.Moving:
                charMan.movementScript.enabled = true;
                state = State.Idle;
                break;

            // If possible, use the unit's special attack
            case State.NoblePhantasm:
                ActivateNP();
                state = State.Idle;
                break;
        }
    }

    /// <summary>
    /// CheckState() is a function serving as a nest for all target finding functions.
    /// </summary>
    private void CheckState()
    {
        CleanLists(); // Clear the lists
        // If there's no one on the field, return end the turn
        if (!TargetCheck())
        {
            EndTurn();
            return;
        }

        // NP activation check
        // Do we have a full turn's worth of actions and 60+ MP saved up?
        if (ActionUsage() && currentMP >= 60)
        {
            CheckNP(); // Check if we have a legal target for our NP
            if (NPTarget != null) // If our target is not null (that is, if we have one)
            {
                // Let the AI know it can use its NP
                state = State.NoblePhantasm;
                return; // Skip the rest of the function
            }
        }

        CheckFriendlies(); // Team-buff check

        // Melee attack check
        if (CheckMelee())
        {
            state = State.Melee;
            return;
        }
        // Ranged attack check
        if (CheckRanged())
        {
            state = State.Ranged;
            return;
        }

        // If none of the above work out, put us in a moving state
        state = State.Moving;
        return;
    }

    /// <summary>
    /// TargetCheck() checks if there are 0 (or less) enemy units on the field, 
    /// and returns a bool on whether there are (true) or not (false).
    /// This function was made to fix an error where the AI would get stuck in an infinite
    /// loop looking for someone if the units on field list was empty.
    /// </summary>
    /// <returns> bool flag </returns>
    private bool TargetCheck()
    {
        if (UnitsOnField.Count <= 0) { return false; }
        else { return true; }
    }

    /// <summary>
    /// CheckFriendlies() looks for allies units around this unit.
    /// If there are "enough" within a certain range, then there it is worthwhile to
    /// activate the unit's buffs.
    /// </summary>
    private void CheckFriendlies()
    {
        Vector3 NobuPos = transform.position; // Our position

        // Find all allies with the NPC tag
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject NPC in NPCs)
        {
            Vector3 allyPos = NPC.transform.position;
            float d = Vector3.Distance(NobuPos, allyPos);
            if (d >= 5f) // If the ally is within Charisma's range
            {
                CharacterManager unit = NPC.GetComponent<CharacterManager>();
                CloseAllies.Add(unit); // Save them as being in range
            }
        }

        // If we have 2+ friendlies around us (exclusing ourselves) and enough MP
        // it's worth activating charisma that turn.
        if (CloseAllies.Count >= 3 && currentMP >= 8) { CharismaFlag = true; }
    }

    /// <summary>
    /// CheckMelee() looks for all player-controlled units on the field
    /// and saves them in the melee targets list
    /// </summary>
    /// <returns> Bool flag on whether there is minimum 1 unit in range </returns>
    private bool CheckMelee()
    {
        // Range of our melee strike
        float meleeRange = charMan.equipment.ReturnMainWeapon().range;
        foreach (CharacterManager target in UnitsOnField)
        {
            float d = Vector3.Distance(transform.position, target.transform.position);
            if (d <= meleeRange) { MeleeTargets.Add(target); }
        }
        
        // If we have 1 enemy in melee, return true
        if (MeleeTargets.Count > 0) { return true; }
        else { return false; }
    }

    /// <summary>
    /// CheckNP() looks for all enemy units that would serve as suitable 
    /// targets for this unit's special attack. Possible targets are weighed based
    /// on the # of enemies grouped around them (maximizing area-of-effect damage),
    /// and the total health pool of all those units (target the weakest group of enemies)
    /// </summary>
    private void CheckNP()
    {
        // Variables to save
        CharacterManager curTarget = null;
        int curNumberOfUnits = 0;
        int curTotalHP = 0;

        // Find ALL units in range first
        List<CharacterManager> UnitsInRange = NP_UnitsInRange();
        if (UnitsInRange == null) { return; } // If no one is in range, leave

        foreach (CharacterManager target in UnitsInRange)
        {
            // Find all units around that unit
            List<CharacterManager> resultAOE = NP_AOEcheck(target);
            // Check the # of units that the resulting AOE will catch if we attack the target
            int numOfUnits = resultAOE.Count;
            // Save the HP of all those units caught in the AOE
            int totalHP = NP_CheckAOEHP(resultAOE);

            // Nobu will try to catch as many enemies as possible with her NP, targeting the largest group
            if (numOfUnits > curNumberOfUnits)
            {
                curTarget = target;
                curNumberOfUnits = numOfUnits;
                curTotalHP = totalHP;
            }
            // If we have a tie, we instead check between total HP
            // Nobu will target the group with the least total HP
            if (numOfUnits == curNumberOfUnits && curTotalHP > totalHP)
            {
                curTarget = target;
                curNumberOfUnits = numOfUnits;
                curTotalHP = totalHP;
            }
        }

        // If we have a at least 1 NP target, save the target.
        NPTarget = curTarget;
    }

    /// <summary>
    /// CheckRanged(), like CheckMelee() looks for all player-controlled 
    /// units on the field and saves them in the ranged targets list
    /// </summary>
    /// <returns> Bool flag on whether there is minimum 1 unit in range </returns>
    private bool CheckRanged()
    {
        Vector3 NobuPos = transform.position; // Our position
        float gunRange = attack.ReturnWeaponAtIndex(0).range; // Gun range

        foreach (CharacterManager target in UnitsOnField)
        {
            Vector3 targetPos = target.transform.position;
            float d = Vector3.Distance(NobuPos, targetPos);
            if (RangeLOS(targetPos, NobuPos) && d <= gunRange) { RangedTargets.Add(target); }
        }

        // If we have a at least 1 ranged target, return true.
        if (RangedTargets.Count > 0) { return true; }
        else { return false; }
    }

    /// <summary>
    /// RangeLOS() checks to see whether a unit is within this unit's line of sight
    /// for a ranged attack; can't shoot at someone you can't see after all.
    /// </summary>
    /// <param name="TargetPos"> position of the possible target </param>
    /// <param name="OurPos"> our position </param>
    /// <returns> bool flag on whether the target is a legal target </returns>
    private bool RangeLOS(Vector3 TargetPos, Vector3 OurPos)
    {
        Vector3 dirToTarget = (TargetPos - OurPos); // Find the direction to the target
        // Return true or false on whether our raycast struck an obstacle or not
        return !Physics.Raycast(OurPos, dirToTarget, Mathf.Infinity, obstacleMask);
    }


    // Melee and Ranged Attack Methods =======================================================

    /// <summary>
    /// MeleeAttack() selects a target in our melee list
    /// and tells the unit to attack them.
    /// </summary>
    private void MeleeAttack()
    {
        // Pick a target
        CharacterManager target = FindMeleeTarget();
        attack.EnemyAttack(target); // attack
        state = State.Idle; // Return us back to idle
    }

    /// <summary>
    /// RangedAttack() selects a target in our ranged list
    /// and tells the unit to attack them.
    /// </summary>
    private void RangedAttack()
    {
        // Pick a target
        CharacterManager target = FindRangedTarget();
        // Grab the ranged weapon
        Weapon arquebus = attack.ReturnWeaponAtIndex(0);
        // Attack with the ranged weapon
        attack.SecondaryWeaponAttack(target, arquebus);
        state = State.Idle; // Return us back to idle
    }

    /// <summary>
    /// FindMeleeTarget() and FindRangedTarget() both 
    /// select a target from their respective pool of targets.
    /// All potential targets are weighed against each other based on how
    /// low their current HP is; who would be easier to defeat?
    /// </summary>
    /// <returns> CharManager reference to the selected target </returns>
    private CharacterManager FindMeleeTarget()
    {
        // If we only have 1 target, just return them
        if (MeleeTargets.Count == 1) { return MeleeTargets[0]; }
        else
        {
            int lowestHP = 100; // Lowest HP to start
            int index = 0; // Index of the unit
            for (int i = 0; i < MeleeTargets.Count; i++)
            {
                int enemyHP = MeleeTargets[i].character.ReturnCurrentHP();
                // If an enemy has lower HP than the last, save them
                if (enemyHP < lowestHP) 
                {
                    lowestHP = enemyHP;
                    index = i;
                }
            }
            // Return the enemy with the lowest HP
            return MeleeTargets[index];
        }
    }
    private CharacterManager FindRangedTarget()
    {
        // If we only have 1 target, just return them
        if (RangedTargets.Count == 1) { return RangedTargets[0]; }
        else
        {
            int lowestHP = 100; // Lowest HP to start
            int index = 0; // Index of the unit
            for (int i = 0; i < RangedTargets.Count; i++)
            {
                int enemyHP = RangedTargets[i].character.ReturnCurrentHP();
                // If an enemy has lower HP than the last, save them
                if (enemyHP < lowestHP)
                {
                    lowestHP = enemyHP;
                    index = i;
                }
            }
            // Return the enemy with the lowest HP
            return RangedTargets[index];
        }
    }

    // Noble Phantasm Methods ================================================================

    /// <summary>
    /// NP_AOEcheck() looks around a potential target for our special attack for
    /// any further potential victims to the attack's area-of-effect damage.
    /// </summary>
    /// <param name="target"> Target unit to look around </param>
    /// <returns> List of units around that target </returns>
    private List<CharacterManager> NP_AOEcheck(CharacterManager target)
    {
        Vector3 targetPos = target.transform.position; // The unit's position
        List<CharacterManager> targetsAOE = new List<CharacterManager>(); // A list to hold all the units within AOE range of the target

        // Use an overlap sphere to check the area around the target
        Collider[] colliders = Physics.OverlapSphere(targetPos, 10f, characterMask);
        foreach (Collider collider in colliders)
        {
            CharacterManager unit = collider.gameObject.GetComponent<CharacterManager>();
            // If the unit's CharacterManager is not null and they are of the same team as the target, add them to our aoe list
            if (unit != null && target.character.CompareTeams(unit)) { targetsAOE.Add(unit); }
        }
        // Return the aoe list
        return targetsAOE;
    }

    /// <summary>
    /// NP_UnitsInRange() looks for all units in our special attacks range
    /// AND that can be seen (nothing is blovking line of sight)
    /// </summary>
    /// <returns> List of CharManager references we can target </returns>
    private List<CharacterManager> NP_UnitsInRange()
    {
        Vector3 NobuPos = transform.position; // Our position
        // Create a list to save units in
        List<CharacterManager> UnitsInRange = new List<CharacterManager>();

        // Check every unit on the field and grab their position
        foreach (CharacterManager unit in UnitsOnField)
        {
            Vector3 targetPos = unit.transform.position;

            // Check that we have LOS; if this is false, return null
            if (!RangeLOS(targetPos, NobuPos)) { return null; }

            // Check the distance between our position and the target
            float d = Vector3.Distance(NobuPos, targetPos);
            // If the target is within max range but outside min range, add them to our list
            if (d <= NPMax && d > NPMin) { UnitsInRange.Add(unit); }
        }

        // If we've found at least 1 unit, return the list
        if (UnitsInRange.Count > 0) { return UnitsInRange; }
        else { return null; }
    }

    /// <summary>
    /// CheckAOEHP() takes a list of CharManager references and counts the total HP of
    /// all units in the list.
    /// </summary>
    /// <param name="unitList"> List of units to go through </param>
    /// <returns> Total HP sum of those units </returns>
    private int NP_CheckAOEHP(List<CharacterManager> unitList)
    {
        int totalHP = 0; // Start at 0
        // Foreach loop count the HP
        foreach (CharacterManager unit in unitList) { totalHP += unit.character.ReturnCurrentHP(); }
        return totalHP;
    }

    /// <summary>
    /// ActivateNP() is a simple function telling the unit to activate their
    /// special attack
    /// </summary>
    private void ActivateNP()
    {
        useAbility.UseNoblePhantasm_Target(0, NPTarget);
        FullAction(); // Our NP, TTW, uses a full action to activate
    }

    // Activation Methods ====================================================================

    /// <summary>
    /// ActivateCharisma() is a simple function telling the unit to
    /// activate their Charisma skill.
    /// </summary>
    private void ActivateCharisma()
    {
        useAbility.UseAbility(0);
        CharismaFlag = false; // Once we've used it, set the flag to false
    }

    /// <summary>
    /// CleanLists() clears all lists, resets the flags, and finds all
    /// enemies on the field for the next turn.
    /// </summary>
    private void CleanLists()
    {
        NPTarget = null;
        CharismaFlag = false;
        UnitsOnField.Clear();
        MeleeTargets.Clear();
        RangedTargets.Clear();
        CloseAllies.Clear();

        FindAllEnemies();
    }

    /// <summary>
    /// FindAllEnemies() looks for and saves the CharManager reference to
    /// each unit with the "Player" tag currently in the scene.
    /// </summary>
    private void FindAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject enemy in enemies)
        {
            CharacterManager unit = enemy.GetComponent<CharacterManager>();
            if (unit != null) { UnitsOnField.Add(unit); }
        }
    }

    // Turn Functions ========================================================================
    public override void EndTurn()
    {
        turnUnit = false;
        charMan.master.NextTurn();
    }

    public override void StartTurn()
    {
        charMan.unitEffects.Clean_StartOfTurn();
        turnUnit = true;
        state = State.Idle;
        ResetActionCount();
        ResetAbilityUses();
    }
}
