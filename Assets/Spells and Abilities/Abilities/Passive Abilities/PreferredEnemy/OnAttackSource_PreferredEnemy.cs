using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unit or weapon with this ability recieves advantage to attack rolls against
/// all targets with a specifed tag(s)
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Preferred Enemy/On Attack (Source)")]
public class OnAttackSource_PreferredEnemy : Trigger_OnAttack_Source
{
    public List<string> preyTags;

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // If instance is null OR is not of type AttackInstance, return false
        if (instance == null) { return false; }
        if (instance.GetType() != typeof(AttackInstance)) { return false; }
        AttackInstance attack = (AttackInstance)instance;

        // Find the tags belonging to the attack's target unit
        List<string> targetTags = attack.ReturnTargetTags();
        Reroll reroll = Reroll.Advantage;

        foreach (string tag in preyTags)
        {
            // If a tag is found in the list, change the attack instance's advantage
            if (targetTags.Contains(tag)) { attack.ChangeATKAdvantage(reroll); }
        }

        // Something happened, return true
        return true;
    }

    // Unused override actions
    public override void RankRead(Rank rank) { return; }
}
