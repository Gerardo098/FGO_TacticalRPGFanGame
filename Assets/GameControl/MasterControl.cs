using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  MasterControl is the script that handles the most important scripts in a battle scene,
///  inlcuding the camera, the turn manager, and canvas control.
///  
///  In addition, we keep all "public" enum flags stored here for ease of access.
/// </summary>

// Enums
public enum Rank { None, E, D, C, B, A, EX };
public enum Reroll { Disadvantage, No, Advantage }
public enum ActionType { Free, Single, Full }
public enum StatusDuration { Instant, SourceNextTurn, TargetNextTurn, StartNextTurn, StartSaveEnds, EndSaveEnds, TurnCount, Infinite };
public enum Timing { None, OnEquip, OnUnequip, RoundStart, RoundEnd, TurnStart, TurnEnd, OnAttack_Source, OnAttack_Target, OnHit_Source, OnHit_Target, OnDamage_Source, OnDamage_Target }
public enum ActivationTiming { Melee, Spell, NP, Status, Other }
public enum ActivationTiming_Melee { OnAttack, OnTarget, OnHit, OnStruck }
public enum ActivationTiming_Spell { OnCast, OnTarget, OnHit, OnStruck }
public enum ActivationTiming_NP { OnActivation, OnTarget, OnHit, OnStruck }
public enum ActivationTiming_Other { TurnEnd, TurnStart }
public enum Element { Air, Basic, Earth, Ether, Fire, ImaginaryNumbers, Water };
public enum TargetType { Hostile, Friendly, Self, Any };
public enum EffectClass { StatusEffect, PassiveAbility, TriggeredEffect }

// The MasterControl class itself
public class MasterControl : MonoBehaviour
{
    public TacticsCamera GameCamera;
    public RollingManager rollingManager;
    public TurnManager turnManager;
    public TurnCanvasControl turnCanvasControl;
    public GameOverCanvas gameOverCanvas;
    public VictoryCanvas victoryCanvas;

    // Nodes used by all pathfinding and movement scripts
    private GameObject[] nodes;

    /// <summary>
    /// Awake() function, ensures that MasterControl is the *first* script to be initialized.
    /// In the function, we also initialize the GameCamera and TurnManager scripts.
    /// </summary>
    void Awake()
    {
        // Prepare all the nodes for the movement scripts
        nodes = GameObject.FindGameObjectsWithTag("Node");

        // Init functions
        GameCamera.Init();
        turnManager.Init(this);
    }

    /// <summary>
    /// GameOver() is called by the TurnManager script whenever the good team (the PC's characters)
    /// are all slain in battle.
    /// </summary>
    public void Victory()
    {
        turnCanvasControl.CanvasOFF();
        victoryCanvas.Init();
    }

    /// <summary>
    /// GameOver() is called by the TurnManager script whenever the good team (the PC's characters)
    /// are all slain in battle.
    /// </summary>
    public void GameOver()
    {
        turnCanvasControl.CanvasOFF();
        gameOverCanvas.Init();
    }

    /// <summary>
    /// GetNodes() returns the array of all nodes on the field.
    /// </summary>
    /// <returns> Array of all gameobjects in the scene with the "Node" tag </returns>
    public GameObject[] GetNodes() { return nodes; }

    // The following scripts allow a character manager's scripts to let the turn manager know when a change in turn order occurs

    /// <summary>
    /// KillUnit(), as the name implies, receives a charactermanager to remove from the scene.
    /// </summary>
    /// <param name="unitToKill"></param>
    public void KillUnit(CharacterManager unitToKill) { turnManager.KillUnit(unitToKill); }

    /// <summary>
    /// NextTurn() moves the turn unit flag from the current unit to the next
    /// </summary>
    public void NextTurn() { turnManager.NextTurn(); }

    /// <summary>
    /// ReturnUnitList() returns a list of all units in the scene
    /// </summary>
    /// <returns> List of CharacterManager instances </returns>
    public List<CharacterManager> ReturnUnitList() { return turnManager.unitsInScene; }
}
