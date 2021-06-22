using UnityEngine;

/// <summary>
/// A unit that gains this effect is immediately healed a random amount from a given range
/// </summary>
[CreateAssetMenu(menuName = "Effect Triggers/Spell - Healing/On Equip")]
public class OnEquip_SpellHealing : Trigger_OnEquip
{
    public Parameter MAN; // Mana
    int dieAmount;
    int dieSize;

    // This activate() is unused
    public override bool Activate(AbilityEffect effect, CharacterManager unit = null) { return false; }

    public override bool Activate(AbilityEffect effect, AbilityInstance instance = null)
    {
        // Check if instance is null; if so, return false
        if (instance == null) { return false; }
        // Check if the instance is of type EffectInstance; if not, return false
        if (instance.GetType() != typeof(EffectInstance)) { return false; }

        // Grab the effect instance
        EffectInstance power = (EffectInstance)instance;
        // Grab the target for the effect and the information for healing effects
        CharacterManager healTarget = power.ReturnTarget();
        power.ReturnHealingInfo(out int bonus, out Reroll reroll);
        // Grab the source unit's MAN; this spell is boosted by a higher mana score
        // Add their score to the bonus;
        CharacterManager healSource = power.ReturnSource();
        bonus += healSource.character.GetParameter(MAN);

        // Read the rank to find
        RankRead(effect.GetCurrentRank());
        int amountHealed = RollingManager.CustomRoll(dieAmount, dieSize, bonus, reroll);
        // If the amount is higher than 0 (don't know how it could not be), heal the target
        if (amountHealed > 0) { healTarget.character.IncreaseHealth(amountHealed); }

        return true;
    }

    // Use RankRead() to find how much the effect heals
    public override void RankRead(Rank rank)
    {
        switch (rank)
        {
            case Rank.E:
                dieAmount = 1;
                dieSize = 4;
                break;
            case Rank.D:
                dieAmount = 1;
                dieSize = 6;
                break;
            case Rank.C:
                dieAmount = 1;
                dieSize = 8;
                break;
            case Rank.B:
                dieAmount = 1;
                dieSize = 10;
                break;
            case Rank.A:
                dieAmount = 1;
                dieSize = 12;
                break;
            case Rank.EX:
                dieAmount = 2;
                dieSize = 6;
                break;
            default:
                dieAmount = 0;
                dieSize = 0;
                break;
        }
    }
}