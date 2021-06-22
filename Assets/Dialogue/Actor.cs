using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Actor is a "character" in this game's VN-style dialogue system.
/// Each actor stores the character's name and portrait sprite.
/// </summary>
[CreateAssetMenu(menuName = "Data/Actor")]
public class Actor : ScriptableObject
{
    public new string name;
    public Sprite portrait;
}
