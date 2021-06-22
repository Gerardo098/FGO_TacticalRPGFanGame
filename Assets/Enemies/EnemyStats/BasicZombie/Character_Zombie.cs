using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character_Zombie is the character subclass used by the basic zombie unit
/// </summary>
public class Character_Zombie : Character_Root
{
    // Action state enum
    private enum State { Idle, Moving, Attacking }

    private Attack_Enemy attack; // Attack script reference
    // List of units in range of this unit's attack
    private List<CharacterManager> targetsInRange;
    private State state = State.Idle; // Action state flag

    /// <summary>
    /// Init() override
    /// </summary>
    public override void Init()
    {
        // Init stats
        InitValues();
        InitFormulas();

        // Find the character's attack script from the character manager
        attack = (Attack_Enemy)charMan.attackAction;
        // Create a new list
        targetsInRange = new List<CharacterManager>();
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
        // If there are no player units on the field, end the turn
        if (!PlayerStatusCheck(unitTeam)) { EndTurn(); }

        // Switch case checking the state flag
        // Perform an action depending on the enum state
        switch (state)
        {
            // If idle, check for enemies within range
            case State.Idle:
                //Debug.Log("Unit is Idle.");
                CheckMeleeRange();
                break;
            // If attacking, find a suitable target
            case State.Attacking:
                //Debug.Log("Unit is Attacking.");
                attack.EnemyAttack(SelectMeleeTarget());
                state = State.Idle; // return to idle
                break;
            // If moving, enable the movement script
            case State.Moving:
                //Debug.Log("Unit is Moving.");
                charMan.movementScript.enabled = true;
                state = State.Idle; // return to idle
                break;
        }
    }

    /// <summary>
    /// CheckMeleeRange() looks for all player-controlled units on the field
    /// and saves them in the targets in range list
    /// </summary>
    private void CheckMeleeRange()
    {
        targetsInRange.Clear(); // Clean the list; don't want peiple sneaking in
        // Find the range a unit has to be within to be attacked
        float attackRange = charMan.equipment.mainHandSlot.GetRange();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject possibleTarget in enemies)
        {
            // If a possible target is within our attack range, save them to our list
            float d = Vector3.Distance(transform.position, possibleTarget.transform.position);
            if (d <= attackRange)
            {
                targetsInRange.Add(possibleTarget.GetComponent<CharacterManager>());
            }
        }

        // If our targets in range list is not empty, we can switch to attacking someone
        if (targetsInRange.Count > 0) { state = State.Attacking; }
        else { state = State.Moving; } // Else, make a move action
    }

    /// <summary>
    /// SelectMeleeTarget() picks a random unit from our targets in range list
    /// as the target for our attack action and returns it
    /// </summary>
    /// <returns> randomly selected CharManager reference </returns>
    private CharacterManager SelectMeleeTarget()
    {
        // If we only have 1 target in range, then that's who we attack
        if (targetsInRange.Count == 1) { return targetsInRange[0]; }
        else // If we have more, the zombie picks 1 at random
        {
            int index = Random.Range(0, targetsInRange.Count);
            return targetsInRange[index];
        }
    }

    /// <summary>
    /// EndTurn() sets the turnUnit flag to false and lets the master look
    /// for the next unit
    /// </summary>
    public override void EndTurn()
    {
        turnUnit = false;
        charMan.master.NextTurn();
    }

    /// <summary>
    /// StartTurn cleans the unit of effects, sets them as the turn unit, 
    /// and resets their action and ability uses.
    /// </summary>
    public override void StartTurn()
    {
        charMan.unitEffects.Clean_StartOfTurn();
        turnUnit = true;
        state = State.Idle;
        ResetActionCount();
        ResetAbilityUses();
    }
}
