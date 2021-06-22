using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect Triggers/Burned/On Start Turn")]
public class OnStartTurn_Burned : Trigger_OnTurnStart
{
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        CharacterManager target = effect.GetTarget();
        effect.GetDice(out int size, out int amount);
        int damage = RollingManager.CustomRoll(amount, size);

        target.character.ReduceHealth(damage);

        return true;
    }

    public override void RankRead(Rank rank) { return; }
}
