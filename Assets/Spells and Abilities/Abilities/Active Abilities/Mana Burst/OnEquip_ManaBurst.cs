using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Mana Burst/On Equip")]
public class OnEquip_ManaBurst : Trigger_OnEquip
{
    [SerializeField]
    private Parameter STR;

    public override bool Activate(AbilityEffect effect, CharacterManager unit = null)
    {
        int bonus = effect.GetBonus(); // Grab the bonus from the effect
        // Grab the effect's target
        CharacterManager target = effect.GetTarget();
        // Apply the buff
        target.character.parameterContainer.Sum(STR, bonus);
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
