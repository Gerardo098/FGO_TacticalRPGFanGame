using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ===============================================================================================
 * ParameterContainer class starts here
 * ===============================================================================================
 */

/// <summary>
/// ParameterContainer is a class that handles a unit's parameters (stats).
/// </summary>
public class ParameterContainer
{
    // All parameters are stored here
    public List<ParameterReference> parameterList;

    /// <summary>
    /// Creator function; initializes the list
    /// </summary>
    public ParameterContainer()
    {
        parameterList = new List<ParameterReference>();
    }

    /// <summary>
    /// Get() looks for the given parameter and applies + returns it's value to the state int parameter
    /// If the target does not possess that parameter, instead apply 0
    /// </summary>
    /// <param name="parameter"> Parameter to look for </param>
    /// <param name="state"> Int score stored by that parameter; returned via the out keyword </param>
    public void Get(Parameter parameter, out int state)
    {
        // Find the list
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameter);
        // If the parameter reference is null, fill state with 0
        if (pReference == null) { state = 0; }
        // Else, return the reference's parameter value
        else { state = pReference.pValue; }
    }

    /// <summary>
    /// GetParamReference() takes a parameter and finds the unit's reference to their own.
    /// This is done witha simple find function.
    /// </summary>
    /// <param name="parameter"> parameter to look for </param>
    /// <returns> parameter reference belonging to this unit </returns>
    public ParameterReference GetParamReference(Parameter parameter)
    {
        return parameterList.Find(x => x.parameterBase == parameter);
    }

    /// <summary>
    /// Subscribe() lets ys attach actions to a parameter reference's onChange value.
    /// This way we can "subscribe" other functions to our parameter references; we can check whether one has
    /// changed or not, and change the other accordingly
    /// </summary>
    /// <param name="action"> action function to subscribe </param>
    /// <param name="parameter"> parameter to subscribe to </param>
    public void Subscribe(Action action, Parameter parameter)
    {
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameter);
        pReference.onChange += action;
    }

    /// <summary>
    /// Overload of Subscribe()
    /// This version we redo the above; we link parameter reference based on whether one parameter
    /// is dependent on another, and if it changes, we change the "depender" via the action.
    /// </summary>
    /// <param name="action"> action function to subscribe </param>
    /// <param name="parameterD"> Dependent parameter </param>
    /// <param name="parameterS"> Parameter to subscribe to </param>
    public void Subscribe(Action<Parameter> action, Parameter parameterD, Parameter parameterS)
    {
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameterS);
        // Apply the action to the reference's recalculate variable
        if (pReference.recalculate == null) { pReference.recalculate = action; }
        // Check if dependents is null; if it is, initiate a new list for it
        if (pReference.dependents == null) { pReference.dependents = new List<Parameter>(); }
        // Finally, add the dependent parameter to the list of dependents
        pReference.dependents.Add(parameterD);
    }

    /// <summary>
    /// Sum() receives a parameter and an int.
    /// We then add the int value to the unit's parameter value, if any.
    /// If the unit does *not* have the parameter in question, we add it to their list of references.
    /// </summary>
    /// <param name="parameter"> Parameter to search for </param>
    /// <param name="sum"> Amount to add to the parameter </param>
    public void Sum(Parameter parameter, int sum)
    {
        // Find the index of the parameter 
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameter);
        // If the index is not -1, the parameter exists in the list and we call its sum function
        if (pReference != null)
        {
            ParameterReference reference = pReference;
            reference.Sum(sum);
        }
        // Else, we add the param to the list
        else { Add(parameter, sum); }
    }

    /// <summary>
    /// Overload of Sum()
    /// This version of sum receives an entire parameter container.
    /// With a for loop, we go through the given container and add each parameter and value individually.
    /// </summary>
    /// <param name="stats"> item parameter container in question </param>
    public void Sum(ItemParameterContainer stats)
    {
        for (int i = 0; i < stats.parameters.Count; i++)
        {
            Sum(stats.parameters[i], stats.integers[i]);
        }
    }

    /// <summary>
    /// Subtract() is, for all intents and purposes, the exact same as Sum(), only subtracting from a
    /// parameter instead.
    /// </summary>
    /// <param name="parameter"> Parameter to search for </param>
    /// <param name="sum"> Amount to subtract from the parameter </param>
    public void Subtract(Parameter parameter, int sum)
    {
        // Find the index of the parameter 
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameter);
        // If the index is not -1, the parameter exists in the list and we call its subtract function
        if (pReference != null)
        {
            ParameterReference reference = pReference;
            reference.Subtract(sum);
        }
        // Unlike sum, don't remove the now empty parameter; we might need it later.
    }
    /// <summary>
    /// Overload of Subtract()
    /// Again, like the above, similar to Sum(); we receive a container
    /// and we subtract each parameter and value individually.
    /// </summary>
    /// <param name="stats"> item parameter container in question </param>
    public void Subtract(ItemParameterContainer stats)
    {
        for (int i = 0; i < stats.parameters.Count; i++)
        {
            Subtract(stats.parameters[i], stats.integers[i]);
        }
    }

    /// <summary>
    /// Add() is a simple function that creates a new Parameter reference and saves it to our parameter list
    /// </summary>
    /// <param name="parameter"> Parameter to add </param>
    /// <param name="sum"> Value to assign to the reference </param>
    private void Add(Parameter parameter, int sum)
    {
        parameterList.Add(new ParameterReference(parameter, sum));
    }

    /// <summary>
    /// GetText() finds a parameter and returns the results of the reference's GetText() function.
    /// </summary>
    /// <param name="parameter"> Parameter to search for </param>
    /// <returns> string containing the reference's information </returns>
    internal string GetText(Parameter parameter)
    {
        ParameterReference pReference = parameterList.Find(x => x.parameterBase == parameter);
        return pReference.GetText();
    }
}

/*
 * ===============================================================================================
 * Character_Root class starts here
 * ===============================================================================================
 */

/// <summary>
/// Character_Root is the abstract class that other units' personal character scripts inherit from.
/// This Root version holds all the universal functions found on all units; specifics are thus found
/// on each units' own subclasses.
/// </summary>
public enum UnitType { Standard, Minion, Player, Heavy, Elite, Boss }
public enum UnitTeam { Good, Evil }

public abstract class Character_Root : MonoBehaviour
{
    // Parameters + Sub-Parameters
    public ParameterStructure parameterStructure; // Structure attached to the character
    public ParameterContainer parameterContainer; // Container holding all relevant parameters
    private int MaxHP; // Max HP that this unit can have
    private readonly int MaxMP = 100; // Max MP that this unit can have
    public int currentHP; // Current HP at a given moment
    public int currentMP; // Current MP at a given moment
    public Slider healthSlider; // HP slider, used to visually represent the unit's current HP
    public Slider mpSlider; // HP slider, used to visually represent the unit's current HP
    public event Action<float> OnHealthChange = delegate { }; // Action function used by our health slider
    public event Action<float> OnMPChange = delegate { }; // Action function used by our health slider
    public CharacterManager charMan; // Reference to the character manager script

    // Tags and flags
    public UnitType unitType; // What "level" of unit are we dealing with here
    public UnitTeam unitTeam; // What team does this unit belong to?
    public List<string> characterTags; // Tags, used by effects

    // Turn + Action Control
    public GameObject turnMenu; // The turn menu used by units on the players' turn
    public bool turnUnit = false; // Lets us know if this is the turn unit or not
    private int initiative = 0; // Unit's initiative; used by the turn manager
    private int maxActions; // Max # of actions this unit can perform in a single turn (usually 3)
    protected int actionCount = 0; // Total # of actions that unit can take during their turn; can go higher than maxAction

    // Init
    public abstract void Init();

    /// <summary>
    /// InitValues() grabs the parameter structure above and adds each parameter in it to our
    /// unit's list of stats.
    /// </summary>
    internal void InitValues()
    {
        // Create new container
        parameterContainer = new ParameterContainer();
        // For loop; go through all the parameters inside it
        for (int i = 0; i < parameterStructure.parameters.Count; i++)
        {
            // Grab each of the parameters in the parameter structure and add them to our container's list
            Parameter newParameter = parameterStructure.parameters[i];
            parameterContainer.parameterList.Add(new ParameterReference(newParameter, 0));
        }

        // Set the unit's Health value
        SetMaxHPMP();
        InitStatusBars();


        maxActions = 3; // The standard # of actions

    }

    private void InitStatusBars()
    {
        charMan.HPBar.Init(); // Initialize the HPBar here
        charMan.MPBar.Init();

        charMan.MPBarCanvas.SetActive(false);
        charMan.HPBarCanvas.SetActive(false); // Turn the canvas off here
    }

    /// <summary>
    /// InitFormulas(), follows directly after InitValues().
    /// This script looks through each reference in our parameter list for a formula script
    /// and activates it: calculating it and subcscribing the dependent parameters.
    /// </summary>
    internal void InitFormulas()
    {
        foreach (ParameterReference parameterReference in parameterContainer.parameterList)
        {
            // If a parameter has a formula, calculate the formula and add it to the parameter via the sum function
            if (parameterReference.parameterBase.formula)
            {
                parameterReference.Null(); // Reset the parameter's values to the default (0)
                Formula_Root formula = parameterReference.parameterBase.formula; // Grab the formula
                // Calculate the formula and add it to the parameter reference
                parameterContainer.Sum(parameterReference.parameterBase, formula.Calculate(parameterContainer));

                // Grab dependencies from each formula and call the subscribe function for each
                List<Parameter> references = parameterReference.parameterBase.formula.GetReferences();
                for (int i = 0; i < references.Count; i++)
                {
                    // Subscribe parameterBase to references[i] with the action RecalculateParameter
                    parameterContainer.Subscribe(RecalculateParameter, parameterReference.parameterBase, references[i]);
                }
            }
        }
    }

    /// <summary>
    /// RecalculateParameters() is the action function we save to a parameter, calling it whenever another 
    /// parameter that the former depends on changes to apply the new changes to this one.
    /// </summary>
    /// <param name="parameter"> Parameter to change </param>
    public void RecalculateParameter(Parameter parameter)
    {
        ParameterReference parameterReference = parameterContainer.GetParamReference(parameter);
        parameterReference.Null();
        // Add up all other sources 
        Formula_Root formula = parameterReference.parameterBase.formula;
        parameterContainer.Sum(parameterReference.parameterBase, formula.Calculate(parameterContainer));
    }

    /// <summary>
    /// SetMaxHP() is a way to set the unit's max HP depending on the type of unit is is.
    /// Max MP is universally 100 and starts at 0.
    /// </summary>
    private void SetMaxHPMP()
    {
        currentHP = MaxHP; // The unit starts at full HP
        currentMP = 0; // We start at 0 MP

        switch (unitType)
        {
            case UnitType.Standard:
                MaxHP = 25;
                break;
            case UnitType.Player:
                MaxHP = 100;
                break;
            case UnitType.Minion:
                MaxHP = 1;
                break;
            case UnitType.Elite:
                MaxHP = 50;
                break;
            case UnitType.Boss:
                MaxHP = 300;
                break;
            case UnitType.Heavy:
                MaxHP = 100;
                break;
            // The default is 1; we assume it's a "minion"
            default:
                MaxHP = 1;
                break;
        }
    }

    /// <summary>
    /// SetInitative() saves a given value as the unit's initiative in this scene
    /// </summary>
    /// <param name="_initiative"> value to be saved </param>
    public void SetInitiative(int _initiative) { initiative = _initiative; }

    /// <summary>
    /// GetInitative() returns out initative value
    /// </summary>
    /// <returns> this unit's initiative value </returns>
    public int GetInitiative() { return initiative; }

    /// <summary>
    /// CompareTeams() takes 1 unit's chara manager script and checks if that unit is on the same team as this one (good or evil)
    /// </summary>
    /// <param name="target"> target whose team we have to check </param>
    /// <returns> bool flag on whether we're on the same team or not </returns>
    internal bool CompareTeams(CharacterManager target)
    {
        // If this unit's and the target's teams match, return true
        if (target.character.unitTeam == unitTeam) { return true; }
        return false; // This unit is not of the same team as the target unit
    }
    /// <summary>
    /// Overload of CompareTeams()
    /// This one is exactly the same as the above, except it compares 2 separate units with each other.
    /// We ignore this unit's team.
    /// </summary>
    /// <param name="target1"> First unit to compare </param>
    /// <param name="target2"> Second unit to compare </param>
    /// <returns> bool flag on whether they're on the same team or not </returns>
    internal bool CompareTeams(CharacterManager target1, CharacterManager target2)
    {
        // If both units share the same team, return true
        if (target1.character.unitTeam == target2.character.unitTeam) { return true; }
        return false; // These 2 units do not share the same team
    }

    // TURN RELATED INFORMATION ======================================================================================
    /// <summary>
    /// ResetActionCount(), as the name implies, resets the unit's actions back to their max of 3.
    /// </summary>
    protected void ResetActionCount() { actionCount += maxActions; }
    /// <summary>
    /// ResetAbilityUses() calls on our skill controller's function of the 
    /// same name to reset our active abilities
    /// </summary>
    protected void ResetAbilityUses() { charMan.skillController.ResetActiveUses(); }
    /// <summary>
    /// ReduceActionCount() is called whenever a unit performs an action other than a free action
    /// This reduces their current total actions for the turn by 1.
    /// If necessary, the amount we reduce it by may be altered.
    /// </summary>
    /// <param name="amount"> Amount to reduce the unit's total actions for the turn by; default is 1 </param>
    public void ReduceActionCount(int amount = 1) {
        actionCount -= amount;
        // If the unit has 0 or less actions left, and is the turn unit, call the EndTurn() function
        if (actionCount <= 0 && turnUnit) { EndTurn(); }
    }

    /// <summary>
    /// A FullAction() uses up all actions for that turn (3+). This immediately ends the turn.
    /// Only used by certain abilities such as Noble Phantasms or other powerful effects.
    /// </summary>
    public void FullAction()
    {
        actionCount = 0;
        EndTurn();
    }

    /// <summary>
    /// ActionUsage() returns whether we're sitting on our full turn's worth of actions.
    /// Used by other functions to help calculate if we can perform a full action or not.
    /// </summary>
    /// <returns> bool flag on whether we're full on actions or not </returns>
    public bool ActionUsage() { return actionCount == maxActions; }

    /// <summary>
    /// TurnTriggers() handles our start and end of turn, as well as start and end of round, triggers for all
    /// of our abilities. As such, this is called at the start + end of the turn/round.
    /// </summary>
    /// <param name="timing"> Trigger timing we're looking to activate </param>
    protected void TurnTriggers(Timing timing)
    {
        // Switch checking the timing
        // NOTE - check the timing first; spent effects are spent to prevent them going off again
        switch (timing)
        {
            case Timing.TurnStart:
                charMan.unitEffects.Clean_StartOfTurn();
                break;
            case Timing.TurnEnd:
                charMan.unitEffects.Clean_EndOfTurn();
                break;
            case Timing.RoundStart:
                break;
            case Timing.RoundEnd:
                break;

            // If it's default, then we've run into a problem of sorts so just exit this function
            default:
                return;
        }

        // Check for and activate all the available triggers 
        charMan.unitEffects.SearchPassives(timing);
    }

    /// <summary>
    /// PlayerStatusCheck() is a function that checks for all player-controlled OR
    /// NPC units on the field. If there are only units of the specified team, return false. Else, return true.
    /// </summary>
    /// <param name="team"> team we're comparing against </param>
    /// <returns> bool flag if a unit NOT of the team is found </returns>
    protected bool PlayerStatusCheck(UnitTeam team)
    {
        GameObject[] objects;
        switch (team)
        {
            case UnitTeam.Evil:
                objects = GameObject.FindGameObjectsWithTag("Player");
                return CheckUnitArray(team, objects);
            case UnitTeam.Good:
                objects = GameObject.FindGameObjectsWithTag("NPC");
                return CheckUnitArray(team, objects);
            default:
                return false;
        }
    }

    /// <summary>
    /// In a foreach loop, if we find a unit that belongs to the good team, return true
    /// This means that the player team still has at least 1 unit left.
    /// </summary>
    /// <param name="team"> team we're comparing against </param>
    /// <param name="objects"> array of gameobjects found by PlayerStatusCheck </param>
    /// <returns> bool flag if a unit NOT of the team is found </returns>
    private bool CheckUnitArray(UnitTeam team, GameObject[] objects)
    {
        foreach (GameObject unit in objects)
        {
            CharacterManager manager = unit.GetComponent<CharacterManager>();
            if (manager != null && manager.character.unitTeam != team) { return true; }
        }
        return false;
    }

    // HP, MP AND DAMAGE GAINING AND RECEIVING =======================================================================

    /// <summary>
    /// MPGeneration() gives the unit a boost of MP equal to their current MANA score.
    /// </summary>
    public void MPGeneration()
    {
        parameterContainer.Get(charMan.MPGen, out int MANA);
        // TODO - Check for any bonuses to MP gen
        IncreaseMP(MANA); // Increase the MP here
    }

    /// <summary>
    /// IncreaseMP(), as the name implies, increases the unit's current mana points by the given amount.
    /// This amount is capped by the MaxMP.
    /// </summary>
    /// <param name="amount"> int amount to increase the current MP by </param>
    public void IncreaseMP(int amount)
    {
        currentMP += amount; // Increase the current HP
        // Keep the target from being overhealed
        if (currentMP > MaxMP) { currentMP = MaxMP; }
        StartCoroutine(ChangeMPPer()); // Activate the HP bar slider
    }
    /// <summary>
    /// ReduceMP() is IncreaseMP() but backwards; we reduce the unit's current MP by the given amount.
    /// </summary>
    /// <param name="amount"> int amount to reduce the current MP by </param>
    public void ReduceMP(int amount)
    {
        currentMP -= amount; // Decrease HP
        if (currentMP < 0) { currentMP = 0; }
        StartCoroutine(ChangeMPPer()); // Activate the MP bar slider
    }

    /// <summary>
    /// MPSpendCheck() is run whenever anything calls for a MP cost to check if the amount paid 
    /// will end up killing the unit (unspent + unpayable MP is taken directly from the HP pool).
    /// </summary>
    /// <param name="amountSpent"> Amount of HP we're willing to spend </param>
    /// <returns> bool flag on whether it's safe to spend the HP/MP </returns>
    internal bool MPSpendCheck(int amountSpent)
    {
        // Find the difference between our current MP and the amount we want to spend
        int difference_MP = amountSpent - currentMP;
        // If the unit would kill themselves by spending MP, let the system know not to let them do that
        if (currentHP - difference_MP <= 0) { return false; }
        return true; // Otherwise let them do whatever they want
    }
    
    /// <summary>
    /// ReturnCurrentHP() lets us check the unit's current HP
    /// </summary>
    /// <returns> Current HP amount </returns>
    public int ReturnCurrentHP() { return currentHP; }

    /// <summary>
    /// IncreaseHealth(), as the name implies, increases the unit's current HP by the given amount.
    /// This amount is capped by the MaxHP.
    /// </summary>
    /// <param name="amount"> int amount to increase the current HP by </param>
    public void IncreaseHealth(int amount)
    {
        currentHP += amount; // Increase the current HP
        // Keep the target from being overhealed
        if (currentHP > MaxHP) { currentHP = MaxHP; }
        StartCoroutine(ChangeHealthPer()); // Activate the HP bar slider
    }

    /// <summary>
    /// ReduceHealth() is IncreaseHealth() but backwards; we reduce the unit's current HP by the given amount.
    /// If the unit drops to 0 or less HP, kill the unit
    /// </summary>
    /// <param name="amount"> int amount to reduce the current HP by </param>
    public void ReduceHealth(int amount)
    {
        currentHP -= amount; // Decrease HP
        StartCoroutine(ChangeHealthPer()); // Activate the HP bar slider
        // If the unit drops to or below 0 HP, let the game know they're dead
        if (currentHP <= 0) { charMan.master.KillUnit(charMan); }
    }

    /// <summary>
    /// ChangeHealthPer() is a coroutine used to handle changing the HP bar slider whenever the unit
    /// gains or loses HP in the game.
    /// </summary>
    /// <returns> IEnumerator </returns>
    private IEnumerator ChangeHealthPer()
    {
        // Here we look at the health bar and change it
        // Turn the HP bar on
        charMan.HPBarCanvas.SetActive(true);
        float HPPercent = (float)currentHP / (float)MaxHP;
        OnHealthChange(HPPercent); // Send the new % to the health bar so it can update itself

        // Wait 1 second and turn the HP bar off
        yield return new WaitForSeconds(1f);
        charMan.HPBarCanvas.SetActive(false);
    }

    /// <summary>
    /// ChangeMPPer() is a coroutine used to handle changing the MP bar slider whenever the unit
    /// gains or loses MP in the game.
    /// </summary>
    /// <returns> IEnumerator </returns>
    private IEnumerator ChangeMPPer()
    {
        // Here we look at the health bar and change it
        // Turn the HP bar on
        charMan.MPBarCanvas.SetActive(true);
        float MPPercent = (float)currentMP / (float)MaxMP;
        OnMPChange(MPPercent); // Send the new % to the health bar so it can update itself

        // Wait 1 second and turn the HP bar off
        yield return new WaitForSeconds(1f);
        charMan.MPBarCanvas.SetActive(false);
    }

    // COMBAT RELATED INFO HERE =======================================

    /// <summary>
    /// GetAgility() and GetArmour() simply return the unit's value in those two parameters
    /// </summary>
    /// <returns> value of the parameter in question </returns>
    public int GetAgility()
    {
        parameterContainer.Get(charMan.AGL, out int value);
        return value;
    }
    public int GetArmour()
    {
        parameterContainer.Get(charMan.ARM, out int value);
        return value;
    }

    /// <summary>
    /// GetParameter is the "universal" way of getting any parameter's value, so long as we know what parameter we want.
    /// </summary>
    /// <param name="parameter"> Parameter whose value we are looking for </param>
    /// <returns> current value of said parameter </returns>
    public int GetParameter(Parameter parameter)
    {
        parameterContainer.Get(parameter, out int value);
        return value;
    }

    // HERE BE THE ABSTRACTS ==========================================
    /// <summary>
    /// These are handled by each unit in their respective subclasses
    /// </summary>
    public abstract void StartTurn();
    public abstract void EndTurn();
}