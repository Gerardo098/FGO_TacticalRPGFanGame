using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Noble Phantasm/Clarent Blood Arthur")]
public class NP_ClarentBloodArthur : Ability
{
    public GameObject beamHB;

    public override void Activate(CharacterManager charMan, GameObject target = null)
    {
        RankRead();
        LookAtTarget(charMan.transform, target.transform);
        GameObject beamBox = Instantiate(beamHB, charMan.transform);
        beamBox.GetComponent<CBAColliderControl>().init(charMan, target, this);
    }

    public override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public override void NPEffect(NPInstance NP)
    {
        CBA_Instance instance = (CBA_Instance)NP;
        List<string> targetTags = instance.ReturnTarget().character.characterTags;

        RankRead();
        DCRoll(instance, targetTags);
        DamageRoll(instance, targetTags);
    }

    private void DCRoll(CBA_Instance instance, List<string> targetTags)
    {
        CharacterManager target = instance.ReturnTarget();
        int BonusST = target.character.GetAgility() + instance.ReturnBonusST();
        int savingThrow;

        // If the target is a Pendragon, they suffer disadvantage to resisting CBA
        if (targetTags.Contains("Pendragon")) { savingThrow = RollingManager.BasicRoll(target, BonusST, Reroll.Disadvantage); }
        // If they're not a Pendragon, they have a regular saving throw roll
        else  { savingThrow = RollingManager.BasicRoll(target, BonusST); }

        // If the unit beat our DC, set the DC flag
        if (savingThrow > DC) { instance.SetDCFlag(true); }
    }

    private void DamageRoll(CBA_Instance instance, List<string> targetTags)
    {
        CharacterManager CBASource = instance.ReturnSource();
        // CBA works best against units with the King tag
        if (targetTags.Contains("King"))
        {
            instance.SetDamage(RollingManager.CustomRoll(CBASource, dieAmount, dieSize, Reroll.Advantage));
        }
        // Normal damage against other units though
        else { instance.SetDamage(RollingManager.CustomRoll(CBASource, dieAmount, dieSize)); }
    }


    public override void RankRead()
    {
        switch (CurrentRank)
        {
            case Rank.E:
                dieAmount = 5;
                dieSize = 10;
                DC = 20;
                MPCost = 50;
                break;
            case Rank.D:
                dieAmount = 6;
                dieSize = 10;
                DC = 21;
                MPCost = 60;
                break;
            case Rank.C:
                dieAmount = 7;
                dieSize = 10;
                DC = 22;
                MPCost = 70;
                break;
            case Rank.B:
                dieAmount = 8;
                dieSize = 10;
                DC = 23;
                MPCost = 80;
                break;
            case Rank.A:
                dieAmount = 9;
                dieSize = 10;
                DC = 24;
                MPCost = 90;
                break;
            case Rank.EX:
                dieAmount = 10;
                dieSize = 10;
                DC = 25;
                MPCost = 100;
                break;
            default:
                dieAmount = 0;
                dieSize = 0;
                DC = 0;
                MPCost = 0;
                break;
        }
    }
}

public class CBA_Instance : NPInstance
{
    private readonly Ability CBA;

    public CBA_Instance(CharacterManager _source, CharacterManager _target, Ability NP)
    {
        source = _source;
        target = _target;
        CBA = NP;
        sourceUnitTags = CBA.tags;
    }

    public override void HandleNP()
    {
        Effects_OnNP();
        CBA.NPEffect(this);
        InflictDamage();
    }

    internal void InflictDamage()
    {
        if (DCCleared) { Damage /= 2; }
        int ARM = target.character.GetArmour() + BonusARM;
        int total = Damage - ARM;
        if (total > 0)
        {
            //Debug.Log("Inflicted a total of " + total + " on the target!");
            target.character.ReduceHealth(total);
        }
    }
}
