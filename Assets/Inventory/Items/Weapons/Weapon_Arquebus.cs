using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapons/Ranged/Arquebus")]
public class Weapon_Arquebus : Weapon
{
    public override void RankRead()
    {
        switch (rank)
        {
            case Rank.E:
                number = 2;
                size = 8;
                break;
            case Rank.D:
                number = 2;
                size = 10;
                break;
            case Rank.C:
                number = 2;
                size = 12;
                break;
            case Rank.B:
                number = 4;
                size = 6;
                break;
            case Rank.A:
                number = 3;
                size = 10;
                break;
            case Rank.EX:
                number = 3;
                size = 12;
                break;
            default: // Somehow have no rank? damage is 0
                number = 0;
                size = 0;
                break;
        }
    }
}
