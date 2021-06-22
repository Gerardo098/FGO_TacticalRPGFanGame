using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character_ZombieCaster is the character subclass used by the zombie caster unit
/// </summary>
public class Character_ZombieCaster : Character_Root
{
    // Action state enum
    private enum State { Idle, Moving, Attacking, AOE, Summon }
    // List of units in range of this unit's attack
    private List<CharacterManager> targetsInRange;
    // List of units in range of this unit's area-of-effect spell
    private List<CharacterManager> possibleAOE;
    private State state = State.Idle; // Action state flag
    private Casting_Enemy castAction; // Casting script reference

    /// <summary>
    /// Init() override
    /// </summary>
    public override void Init()
    {
        // Init stats
        InitValues();
        InitFormulas();

        // Create the lists
        targetsInRange = new List<CharacterManager>();
        possibleAOE = new List<CharacterManager>();
        // Find the character's casting script from the character manager
        castAction = (Casting_Enemy)charMan.castingAction;
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

        switch (state)
        {
            // Check for a target in spell range
            // If so, cast the spell
            // Else move towards the closest target

            // If idle, check for enemies within range
            case State.Idle:
                //Debug.Log("Unit is idle.");
                CheckSpellRange(castAction.ReturnSpell(0));
                break;
            // If attacking, find a suitable target
            case State.Attacking:
                //Debug.Log("Unit is casting a spell.");
                castAction.EnemyCast_ST(SelectTarget(), castAction.ReturnSpell(0));
                state = State.Idle;
                break;
            // If casting an AOE spell, find a suitable group of targets
            case State.AOE:
                castAction.EnemyCast_AOE(possibleAOE, castAction.ReturnSpell(1));
                state = State.Idle;
                break;
            // If moving, enable the movement script
            case State.Moving:
                //Debug.Log("Unit is moving.");
                charMan.movementScript.enabled = true;
                state = State.Idle;
                break;
            // If Summoning minions, cast the spell and end the turn
            case State.Summon:
                //Debug.Log("Unit is summoning zombies.");
                castAction.EnemyCast_NoTarget(castAction.ReturnSpell(2));
                state = State.Idle;
                EndTurn(); // Summoning more friends costs a whole turn
                break;
        }
    }

    /// <summary>
    /// CheckSpellRange() looks for all player-controlled units on the field
    /// and saves them in the targets in range list.
    /// But if there are no minions on the field, summon more of those instead.
    /// </summary>
    private void CheckSpellRange(Spell spell)
    {
        // Quickly check if the unit should summon more creatures
        if (!CheckMinionCount())
        {
            state = State.Summon; // We can summon more friends
            return; // Return to skip the rest
        }

        targetsInRange.Clear(); // Clear the list of targets
        float attackRange = spell.ReturnRange(); // Get the spell's max range

        // Find all gameobjects with the Player tag and find their distance from this unit
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject possibleTarget in enemies)
        {
            float d = Vector3.Distance(transform.position, possibleTarget.transform.position);
            if (d <= attackRange)
            {
                // If a target is in range, save them to the list
                targetsInRange.Add(possibleTarget.GetComponent<CharacterManager>());
            }
        }

        FindAOE(); // Look for any potential worthwhile AOE casts

        // Target(s) in range, change the state
        if (targetsInRange.Count > 0) { state = State.Attacking; }
        else if (possibleAOE.Count > 0) { state = State.AOE; }
        else { state = State.Moving; } // If there's no one, just move to position
    }

    /// <summary>
    /// CheckMinionCount() looks for the amount of "Basic Zombies" in the scene.
    /// If there are enough, then simply return true.
    /// However, if their numbers are too low, let the AI know.
    /// </summary>
    /// <returns> bool flag representing minion numbers </returns>
    private bool CheckMinionCount()
    {
        // List to count # of minions
        List<CharacterManager> currentMinions = new List<CharacterManager>();
        // Find all ally units on the field
        GameObject[] units = GameObject.FindGameObjectsWithTag("NPC");
        foreach(GameObject unit in units)
        {
            // Grab the CharManager script and find the unit's tags
            CharacterManager character = unit.GetComponent<CharacterManager>();
            List<string> charaTags = character.character.characterTags;
            // If those ally units are "Minion" and "BasicZombie", add them to the list
            if (charaTags.Contains("Minion") && charaTags.Contains("BasicZombie")) { currentMinions.Add(character); }
        }
        // If our minion count is 3 or less, we have to summon more
        if (currentMinions.Count < 3) { return false; }
        return true; // If our numbers are good, we don't need more
    }

    /// <summary>
    /// FindAOE() checks all of our possible targets to see if it is worth it for our
    /// zombie caster to use an AOE spell or not.
    /// For a worthwhile AOE spell, the AOE should cover 3+ enemy units
    /// </summary>
    private void FindAOE()
    {
        // Clear the list
        possibleAOE.Clear();

        // List of the current potential target's AOE units
        List<CharacterManager> current = new List<CharacterManager>();
        // List of lists of potential AOEs
        List<List<CharacterManager>> AOE = new List<List<CharacterManager>>();

        // Foreach target in range, find the units around them
        foreach (CharacterManager unit in targetsInRange)
        {
            current.Clear(); // Clear it for each unit
            Collider[] possibles = Physics.OverlapSphere(unit.transform.position, castAction.ReturnSpell(1).ReturnAoE());

            // For each collider we've found
            foreach (Collider collider in possibles)
            {
                CharacterManager potential = collider.GetComponent<CharacterManager>();
                // If potential is not null AND their team equals that of the current unit, add them to the list
                if (potential != null && CompareTeams(unit, potential)) { current.Add(potential); }
            }
            // After checking all possibilities, see if the count is 3 enemies or more in the potential AOE cast
            if (current.Count >= 3) { AOE.Add(current); }
        }

        // Select a random AOE list of targets from our AOE list-list
        if (AOE.Count != 0) { possibleAOE = AOE[Random.Range(0, AOE.Count)]; }
    }

    /// <summary>
    /// SelectTarget() picks a random target for our unit's attack
    /// </summary>
    /// <returns> reference to the target unit </returns>
    private CharacterManager SelectTarget()
    {
        // If we only have 1 target in range, then that's who we attack
        if (targetsInRange.Count == 1) { return targetsInRange[0]; }
        else // If we have more, the zombie picks 1 at random
        {
            int index = Random.Range(0, targetsInRange.Count);
            return targetsInRange[index];
        }
    }

    public override void EndTurn()
    {
        turnUnit = false;
        charMan.master.NextTurn();
    }

    public override void StartTurn()
    {
        charMan.unitEffects.Clean_StartOfTurn();
        state = State.Idle;
        ResetActionCount();
        ResetAbilityUses();
        turnUnit = true;
    }

}