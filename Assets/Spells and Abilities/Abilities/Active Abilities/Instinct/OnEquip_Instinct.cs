using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Instinct/On Equip")]
public class OnEquip_Instinct : Trigger_OnEquip
{
    [SerializeField]
    private Parameter CRT;
    [SerializeField]
    private Parameter EVA;

    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        // Grab the effect's bonus
        int bonus = effect.GetBonus();
        // Grab the effect's target
        CharacterManager target = effect.GetTarget();
        // Apply the effects
        target.character.parameterContainer.Sum(CRT, bonus);
        target.character.parameterContainer.Sum(EVA, bonus);
        // return
        return true;
    }

    // Unused functions
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        throw new System.NotImplementedException();
    }

    public override void RankRead(Rank rank)
    {
        throw new System.NotImplementedException();
    }
}
