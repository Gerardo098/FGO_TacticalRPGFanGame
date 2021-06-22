using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Shapeshift/On Equip")]
public class OnEquip_Shapeshift : Trigger_OnEquip
{
    [SerializeField]
    private Parameter ARM;

    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        int bonus = effect.GetBonus(); // Grab the bonus from the effect
        // Grab the effect's target
        CharacterManager target = effect.GetTarget();
        // Apply the effect
        target.character.parameterContainer.Sum(ARM, bonus);
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
