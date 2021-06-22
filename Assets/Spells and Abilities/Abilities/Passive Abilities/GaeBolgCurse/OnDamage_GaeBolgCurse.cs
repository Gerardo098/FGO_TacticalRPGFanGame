using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit damaged by a weapon possessing this effect must roll a saving throw against it or suffer
/// the Cursed Barbs status effect for a turn.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Gae Bolg Curse/On Damage (Source)")]
public class OnDamage_GaeBolgCurse : Trigger_OnDamage_Source
{
    // The status effect
    public AbilityEffect curse;
    
    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null or NOT of type Attack, return false
        if (instance == null || instance.GetType() != typeof(AttackInstance)) { return false; }
        AttackInstance attack = (AttackInstance)instance;

        // Prepare the variables for the passive effect instance
        CharacterManager source = attack.ReturnSource();
        CharacterManager target = attack.ReturnTarget();
        Weapon GaeBolg = attack.ReturnWeapon();
        Rank weaponRank = GaeBolg.rank;

        // Set the curse's Rank
        curse.SetOgRank(weaponRank);

        // Create the new instance
        EffectInstance newInstance = new EffectInstance(curse, target, source);
        // Hand the target the new instance
        target.AbilityInstanceRead(newInstance);
        // Return true
        return true;
    }

    // Unused Abstract Methods =========================================================
    public override void RankRead(Rank rank) { return; }
}
