using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mana Burst Flame increases the user's armour against fire damage 
/// and increases their damage by a set amount for that turn.
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Mana Burst (Flame)/On Hit (Target)")]
public class OnHitTarget_MBFlame : Trigger_OnHit_Target
{
    int bonus; // Bonus ARM

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null, return false
        if (instance == null) { return false; }

        // Else, check the instance type
        if (instance.GetType() == typeof(AttackInstance))
        {
            ApplyEffect(effect, (AttackInstance)instance);
            return true; // return
        }
        if (instance.GetType() == typeof(SpellInstance))
        {
            ApplyEffect(effect, (SpellInstance)instance);
            return true; // return
        }

        return true;
    }

    /// <summary>
    /// ApplyEffect() hands the ability instance the ARM bonus against Fire damage
    /// </summary>
    /// <param name="effect"> AbilityEffect source of the bonus </param>
    /// <param name="instance"> Ability instance to be changed </param>
    private void ApplyEffect(AbilityEffect effect, SpellInstance instance)
    {
        Element element = instance.ReturnElement();
        if (element == Element.Fire)
        {
            bonus = effect.GetBonus();
            instance.SetARMBonus(bonus);
        }
    }

    /// <summary>
    /// ApplyEffect() hands the ability instance the ARM bonus against Fire damage
    /// </summary>
    /// <param name="effect"> AbilityEffect source of the bonus </param>
    /// <param name="instance"> Ability instance to be changed </param>
    private void ApplyEffect(AbilityEffect effect, AttackInstance instance)
    {
        List<string> tags = instance.ReturnWeapon().tags;
        if (tags.Contains("Fire"))
        {
            bonus = effect.GetBonus();
            instance.SetARMBonus(bonus);
        }
    }

    public override void RankRead(Rank rank)
    {
        switch (rank)
        {
            case Rank.E:
                bonus = 1;
                break;
            case Rank.D:
                bonus = 2;
                break;
            case Rank.C:
                bonus = 3;
                break;
            case Rank.B:
                bonus = 4;
                break;
            case Rank.A:
                bonus = 5;
                break;
            case Rank.EX:
                bonus = 6;
                break;
            default:
                bonus = 0;
                break;
        }
    }
}