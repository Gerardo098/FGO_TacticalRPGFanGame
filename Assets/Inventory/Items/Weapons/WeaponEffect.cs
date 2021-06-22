using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * WeaponEffect is a way to grant weapons special effects
 * For example, Balmung's anti-dragon properties
 * and Gae Bulg's anti-healing curse.
 */
public abstract class WeaponEffect : ScriptableObject
{
    public abstract void RankRead();
}

public abstract class OnHitEffect : WeaponEffect
{
    /*
     * OnHit method
     */
    public abstract void OnHit();
}

public abstract class OnAttackEffect : WeaponEffect
{
    internal enum OnAttackType { Advantage, Bonus };
    [SerializeField]
    internal OnAttackType onAttack;

    public abstract int GrantAdvantage(CharacterManager unit, CharacterManager target = null);
    public abstract int GrantBonus(CharacterManager unit, CharacterManager target = null);
}