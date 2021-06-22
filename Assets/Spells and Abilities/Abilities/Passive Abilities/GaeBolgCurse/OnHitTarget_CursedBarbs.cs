using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cursed Barbs imposes disadvantage to a healing ability's heal effect for a turn.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Cursed Barbs/On Hit (Target)")]
public class OnHitTarget_CursedBarbs : Trigger_OnHit_Target
{
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If the instance is not of type EffectInstance, return false
        if (instance.GetType() != typeof(EffectInstance)) { return false; }
        EffectInstance ability = (EffectInstance)instance;

        // Affect the instance here
        // Change the heal reroll to disadvantage
        ability.ChangeHealAdvantage(Reroll.Disadvantage);
        return true; // Return true
    }

    // Unused Abstract Methods =========================================================
    public override void RankRead(Rank rank) { return; }
}
