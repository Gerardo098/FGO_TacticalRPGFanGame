using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TurnManager is the class that handles nearly everything to do with the turn-based system used
/// by our game's combat.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public List<CharacterManager> unitsInScene; // All units in the scene
    public List<CharacterManager> initiativeOrder; // Units in the scene, organized in initiative order
    public CharacterManager currentUnit; // Unit whose turn it currently is
    public int totalGoodTeam; // # of units in the good team
    public int totalEvilTeam; // # of units in the evil team

    private MasterControl masterControl; // Reference to the master control
    private int newTurnCount = 0; // # of turns before we loop around to the beginning
    private int turnIndex = -1; // Index used to find the current turn unit

    // TODO: initiative system; grab all "units", get their intiatives then place them in a queue/list in order from highest to lowest
    // Function that rolls a character's initiative found here (or in the character script themselves)

    /// <summary>
    /// Init() function; intializes the entire TurnManager script and gets it up and running
    /// </summary>
    /// <param name="_masterControl"> Reference of the MasterControl script </param>
    public void Init(MasterControl _masterControl)
    {
        // Save the Master Controller
        masterControl = _masterControl;

        // Count up the number of all protag. and antag. units on the field
        CountTeams();

        // Roll initiative for all units.
        RollInitiative();

        // Receive the new turn count from the # of units in play.
        newTurnCount = initiativeOrder.Count;

        // Start counting turns.
        SelectFirstUnit();
    }

    /// <summary>
    /// NextTurn() passes the turn unit status from the current unit to the next
    /// </summary>
    internal void NextTurn() { SelectActiveUnit(); }

    /// <summary>
    /// RollInitiative() finds the turn order of all the units on the field. Ties are allowed.
    /// In the spirit of TRPGs, we find a unit's initiative with 3d6 + Agility.
    /// Highest initiative goes first, and descends.
    /// </summary>
    private void RollInitiative()
    {
        // Initiate the initativeOrder list
        initiativeOrder = new List<CharacterManager>();

        // For each unit in the scene...
        foreach (CharacterManager unit in unitsInScene)
        {
            // Find and save the unit's initiative then add them to the initiative order list
            int roll = RollingManager.BasicRoll(GrabAgility(unit.character, unit.AGL));
            unit.character.SetInitiative(roll);
            initiativeOrder.Add(unit);
        }
        // Sort the units by their initiative values here (highest to lowest)
        SortInitiative();
    }

    /// <summary>
    /// This override of RollInitative() handles a new unit being added to the scene. 
    /// Initiative is rolled for this unit and then added to the initiative list
    /// </summary>
    /// <param name="unit"> New unit to be added to the scene </param>
    private void RollInitative(CharacterManager unit)
    {
        int roll = RollingManager.BasicRoll(GrabAgility(unit.character, unit.AGL));
        unit.character.SetInitiative(roll);
    }

    /// <summary>
    /// SortInitative() simply sorts the initative order list from highest to lowest
    /// </summary>
    private void SortInitiative()
    {
        // GetInitiative() grabs the character's initiative value
        initiativeOrder.Sort((a, b) => a.character.GetInitiative().CompareTo(b.character.GetInitiative()));
    }

    /// <summary>
    /// CountTeams() simply counts up the number of units on the good (player) and evil teams.
    /// These results are used by the TurnManager to see whether the player has won/lost.
    /// </summary>
    private void CountTeams()
    {
        // Initate both team counts at 0
        totalGoodTeam = 0;
        totalEvilTeam = 0;
        GrabAllUnits(); // Find all units in the scene

        // Run each character's MassInit() function
        foreach (CharacterManager character in unitsInScene) { character.MassInit(masterControl); }
        TeamCheck(unitsInScene); // Find a team for all units in the scene
    }

    /// <summary>
    /// GrabAllUnits() does exactly that; it grabs all units in the scene and saves them in the
    /// unitsInScene list.
    /// </summary>
    private void GrabAllUnits()
    {
        // Find all gameobjects with either the Player or NPC tag.
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] NPCList = GameObject.FindGameObjectsWithTag("NPC");
        // Initialize the unitsInScene list.
        unitsInScene = new List<CharacterManager>();

        // Use a foreach loop on both arrays to add every unit into the list
        foreach (GameObject PC in playerList)
        {
            CharacterManager PCManager = PC.GetComponent<CharacterManager>();
            if (PCManager != null) { unitsInScene.Add(PCManager); }
        }
        foreach (GameObject NPC in NPCList)
        {
            CharacterManager NPCManager = NPC.GetComponent<CharacterManager>();
            if (NPCManager != null) { unitsInScene.Add(NPCManager); }
        }
    }

    /// <summary>
    /// SelectActiveUnit() finds the unit that should be current turn unit at this moment.
    /// </summary>
    private void SelectActiveUnit()
    {
        TeamCheck(initiativeOrder); // Look at the amount of units in each team
        // If any team has 0 units in it, check the gamestate.
        if (totalEvilTeam == 0 || totalGoodTeam == 0) 
        { 
            GameState(totalGoodTeam, totalEvilTeam);
            return; // Return
        }

        // Make all units stand on their current tiles - this is to fix navmesh collisions
        AnchorAllUnits();

        // if for some reason we don't have anyone as our current unit, run SelectFirstUnit()
        if (currentUnit == null) { SelectFirstUnit(); }

        else // If not, grab the next unit
        {
            turnIndex += 1; // Move the index up 1
            newTurnCount = initiativeOrder.Count; // Keep newTurnCount up to date

            // If the index would move beyond the length of the initiativeOrder array, set it back to 0
            if (turnIndex >= newTurnCount)
            {
                // TODO - Start of the Round Functions
                turnIndex = 0;
                RoundStart(); // Run the start-of-round functions and abilities
            }

            currentUnit = initiativeOrder[turnIndex]; // The new turn unit
            AttachCamera(); // Give the current unit the camera
            currentUnit.unitEffects.Clean_EffectsBySource(); // Check effects for any that might trigger
            currentUnit.character.StartTurn(); // Start the next unit's turn
        }
    }

    /// <summary>
    /// SelectFirstUnit() sets the first unit in the initiative order list as the current turn unit
    /// </summary>
    private void SelectFirstUnit()
    {
        turnIndex = 0; // Set turn index back to 0
        currentUnit = initiativeOrder[0]; // First unit in the list is now the current unit
        AttachCamera();
        RoundStart(); // Run all start of the round functions
        currentUnit.character.StartTurn(); // Start this unit's turn
    }

    /// <summary>
    /// 
    /// </summary>
    /*
     * AttachCamera() calls the GetOffset() function from the camera's script
     * In turn, the camera follows the current turn unit
     */
     private void AttachCamera()
    {
        // Attach camera to the current turn unit
        masterControl.GameCamera.FocusTarget(currentUnit.transform);
    }

    /// <summary>
    /// KillUnit() receives a unit and removes it from the lists and from the scene
    /// Also, updates all necessary variables.
    /// </summary>
    /// <param name="unitToKill"> Unit to remove from the scene </param>
    public void KillUnit(CharacterManager unitToKill)
    {
        // Remove the unit from both lists
        initiativeOrder.Remove(unitToKill);
        unitsInScene.Remove(unitToKill);

        Destroy(unitToKill.gameObject); // Remove the unit from the scene

        TeamCheck(unitsInScene); // Check whether a team has been defeated or not

        newTurnCount = initiativeOrder.Count; // Keep newTurnCount up to date
    }

    /// <summary>
    /// AddNewUnit() receives a unit and adds it to both lists, and updates all necessary variables
    /// </summary>
    /// <param name="unit"> New unit to add to the scene </param>
    /// <param name="source"> Unit that summoned this creature, if any </param>
    public void AddNewUnit(CharacterManager unit, CharacterManager source = null)
    {
        // Init() all classes the creature has
        unit.MassInit(masterControl);

        // If the unit was summoned by a "source" unit, then it shares the source's Initiative
        if (source != null) { unit.character.SetInitiative(source.character.GetInitiative()); }
        else { RollInitative(unit); } // If not, roll its initiative

        // Add the unit to both lists
        unitsInScene.Add(unit);
        initiativeOrder.Add(unit); 
        // Keep newTurnCount up to date
        newTurnCount = initiativeOrder.Count;
        SortInitiative(); // Reshuffle the InitiativeOrder to avoid any strange effects
    }

    /// <summary>
    /// TeamCheck() couns the number of Good and Evil aligned units in the scene for
    /// the sake of checking the game state.
    /// </summary>
    /// <param name="unitList"> list of units in the scene </param>
    private void TeamCheck(List<CharacterManager> unitList)
    {
        // Variables to count teams
        int good = 0;
        int evil = 0;
        foreach (CharacterManager unit in unitList)
        {
            if (unit.character.unitTeam == UnitTeam.Good) { good += 1; }
            else { evil += 1; }
        }

        // If we have one team with 0 members, check the game state (just in case)
        if (good == 0 || evil == 0) { GameState(good, evil); }

        // Save the counts
        totalGoodTeam = good;
        totalEvilTeam = evil;
    }

    /// <summary>
    /// GameState() checks whether the player has achieved victory (no enemy units in the scene) or lost
    /// (all friendly units have been slain). 
    /// </summary>
    /// <param name="good"> count of all team "good" units </param>
    /// <param name="evil"> count of all team "evil" units </param>
    private void GameState(int good, int evil)
    {
        // Do the game over screen here (check good first)
        if (GameState_Good(good)) 
        { 
            masterControl.GameOver();
            CleanTurnManager();
        }
        // Do the victory screen here
        if (GameState_Evil(evil))
        {
            masterControl.Victory();
            CleanTurnManager();
        }
    }

    /// <summary>
    /// These functions take the count of a "team" and simply check whether it's less than or equal to 0.
    /// We return true if that's the case, false otherwise.
    /// </summary>
    /// <returns> Bool flag signifying the state of the team </returns>
    private bool GameState_Good(int goodUnits)
    {
        if (goodUnits <= 0) { return true; }
        else { return false; }
    }
    private bool GameState_Evil(int evilUnits)
    {
        if (evilUnits <= 0) { return true; }
        else { return false; }
    }

    /// <summary>
    /// CleanTurnManager() is used when a victory or game over screen happens; this is to keep all units from doing anything
    /// just in case to avoid any problems.
    /// </summary>
    private void CleanTurnManager()
    {
        foreach (CharacterManager character in initiativeOrder)
        {
            // Make every unit NOT the turn unit by switching the flag to false
            character.character.turnUnit = false;
        }
        // Clear both lists
        initiativeOrder.Clear();
        unitsInScene.Clear();
    }

    /// <summary>
    /// AnchorAllUnits() goes through all the units in the scene and calls their AnchorUnit() function
    /// </summary>
    private void AnchorAllUnits()
    {
        foreach (CharacterManager unit in unitsInScene)
        {
            unit.AnchorUnit();
        }
    }


    /// <summary>
    /// GrabAgility() takes a character and returns their agility score for initiative roll counting.
    /// </summary>
    /// <param name="character"> unit whose AGL score we need </param>
    /// <param name="agility"> parameter to search for </param>
    /// <returns> int containting the unit's current AGL score </returns>
    private int GrabAgility(Character_Root character, Parameter agility)
    {
        // Call the unit's Get() function from their parameter container script
        character.parameterContainer.Get(agility, out int AGL);
        return AGL;
    }

    /// <summary>
    /// RoundStart() runs all functions that would activate every time the first unit in initiative order would become turn unit
    /// </summary>
    private void RoundStart()
    {
        // TODO - Other start-of-round effects and whatnot
        MPGeneration(); // Give everyone their mana for the round
    }

    /// <summary>
    /// MPGeneration() is called at the start of each round; each unit receives MP equal to their MP generation stat.
    /// </summary>
     private void MPGeneration()
    {
        foreach (CharacterManager character in initiativeOrder){ character.character.MPGeneration(); }
    }
}