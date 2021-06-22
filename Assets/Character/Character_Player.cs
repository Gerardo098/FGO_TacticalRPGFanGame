using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character_Player is the CharacterRoot subclass used by all player characters.
/// </summary>
public class Character_Player : Character_Root
{
    /// <summary>
    /// Init() is called to have the unit's stat values and formulas prepared
    /// </summary>
    public override void Init()
    {
        InitValues();
        InitFormulas();
    }

    /// <summary>
    /// StartTurn() for player characters
    /// </summary>
    public override void StartTurn()
    {
        // Check if there are ANY enemies left in the scene:
        PlayerStatusCheck(unitTeam);

        TurnTriggers(Timing.TurnStart); // Check all start-of-turn triggers

        // Hand the current unit to the turn canvas control
        charMan.master.turnCanvasControl.ActivateTCC(this);
        charMan.master.turnCanvasControl.CanvasON(); // Turn canvas ON
        turnUnit = true; // Turn unit, baby
        ResetActionCount(); // Set # of actions per turn
        ResetAbilityUses(); // Reset activation limits on abilities
    }

    /// <summary>
    /// EndTurn() for player characters
    /// </summary>
    public override void EndTurn()
    {
        turnUnit = false; // No longer turn unit
        charMan.master.turnCanvasControl.CanvasOFF(); // Turn canvas OFF

        TurnTriggers(Timing.TurnEnd); // Check all end-of-turn triggers

        // Turn key components off to avoid weird bugs
        this.gameObject.GetComponent<Movement_Player>().enabled = false;
        this.gameObject.GetComponent<AttackAction>().enabled = false;
        this.gameObject.GetComponent<CastingAction>().enabled = false;
        charMan.master.NextTurn(); // Change turn unit
    }
}
