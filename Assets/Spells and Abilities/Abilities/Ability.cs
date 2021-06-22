using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability is the abstract class used by all active skills, used by both PC and NPC units.
/// </summary>
public abstract class Ability : ScriptableObject
{
    // Name of the ability
    public string AbilityName;

    // Rank - how strong the skill is
    [SerializeField]
    protected Rank OriginalRank;
    protected Rank CurrentRank;

    // Mp Cost
    protected int MPCost;

    // Range of the ability
    public float range;

    // Bonus + Penalty, if any
    protected int bonus;
    protected int penalty;

    // Die stuff - for rolling if needed
    protected int dieAmount;
    protected int dieSize;

    // DC - difficulty check, if necessary
    protected int DC;

    // Tags for the ability - used by other effects
    public List<string> tags;

    // Does the ability need a manually-selected target>
    public bool needTarget = false;

    // Is the ability a friendly ability (heal/buff) or a hostile one (debuff/penalty)
    public TargetType targetType = TargetType.Friendly;

    // How often the skill has been used 
    protected int currentUses;

    // How many times the skill can be used per turn
    [SerializeField]
    protected int maxUses;

    // The type of action necessary to use the ability
    [SerializeField]
    protected ActionType actionType = ActionType.Free;

    /// <summary>
    /// Init()
    /// </summary>
    public void Init() { CurrentRank = OriginalRank; }


    /// <summary>
    /// ResetUses() resets the current uses int to 0,
    /// letting the unit use the skill once again.
    /// </summary>
    public void ResetUses() { currentUses = 0; }

    /// <summary>
    /// CheckUses() makes sure that we're not using a skill more times per turn
    /// than we are allowed
    /// </summary>
    /// <param name="charMan"> unit attempting to activate the skill </param>
    /// <returns> bool flag on whether the skill can be used </returns>
    public bool CheckUses(CharacterManager charMan)
    {
        // If the unit does not have enough actions left to use the spell, cancel it
        if (!ReadActionType(charMan))
        {
            Debug.Log("Not enough actions available to activate this ability!");
            return false;
        }
        // If we've used it enough times, cancel it
        if (currentUses >= maxUses)
        {
            Debug.Log("Unable to activate skill; already activated the maximum number of times this turn!");
            return false;
        }

        // If not, then we can use the ability
        currentUses++;
        return true;
    }

    /// <summary>
    /// ReadActionType() checks the actiontype flag and looks at the unit's
    /// remaining action count for that turn. If we have enough actions,
    /// we are able to use the skill.
    /// </summary>
    /// <param name="charMan"> unit attempting to activate the skill </param>
    /// <returns> bool flag on whether the skill can be used </returns>
    private bool ReadActionType(CharacterManager charMan)
    {
        // If the action is a Full Action and we used one or more actions this turn
        if (actionType == ActionType.Full && !charMan.character.ActionUsage())
        {
            return false;
        }
        return true; // If not, return true
    }

    /// <summary>
    /// LookAtTarget() rotates the character horizontally to face the target of
    /// their ability, if any.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void LookAtTarget(Transform source, Transform target)
    {
        float targetX = target.position.x;
        float targetZ = target.position.z;
        float sourceY = source.position.y;

        Vector3 direction = new Vector3(targetX, sourceY, targetZ);
        source.LookAt(direction);
    }

    // Simple functions
    public string GetName() { return AbilityName + " [" + CurrentRank + "]"; }
    public int GetDC() { return DC; }
    public int GetMPCost() { return MPCost; }
    public float GetRange() { return range; }
    public Rank GetCurrentRank() { return CurrentRank; }
    public Rank GetOriginalRank() { return OriginalRank; }
    public ActionType ReturnActionType() { return actionType; }

    // Abstracts
    public abstract void RankRead();
    public abstract void Activate(CharacterManager charMan, GameObject target = null);
    public abstract void Deactivate();
    public abstract void NPEffect(NPInstance NP);
}