using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RollingManager is a class that holds a series of static functions allowing all other
/// functions to access a die roll whenever they need to, without needed to reference this script or
/// the master script.
/// </summary>
public class RollingManager : MonoBehaviour
{
    /*
     * HERE BE THE STATIC FUNCTIONS!
     */

    // Basic Roll methods ==========

    /// <summary>
    /// BasicRoll() rolls 3 6-sided die and returns the result.
    /// The overloads of this function can receive additional bonuses/penalties to add/subtract from
    /// the roll, all calling the root function.
    /// </summary>
    /// <returns></returns>
    public static int BasicRoll() { return Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7); }
    public static int BasicRoll(CharacterManager unit) { return CharismaGrab(unit) + BasicRoll(); }
    public static int BasicRoll(int bonus) { return BasicRoll() + bonus; }
    public static int BasicRoll(CharacterManager unit, int bonus) { return CharismaGrab(unit) + bonus + BasicRoll(); }
    public static int BasicRoll(Reroll reroll)
    {
        switch (reroll)
        {
            case Reroll.Advantage:
                return BasicRoll_Advantage();
            case Reroll.Disadvantage:
                return BasicRoll_Disadvantage();
            default:
                return BasicRoll();
        }
    }
    public static int BasicRoll(CharacterManager unit, Reroll reroll)
    {
        switch (reroll)
        {
            case Reroll.Advantage:
                return CharismaGrab(unit) + BasicRoll_Advantage();
            case Reroll.Disadvantage:
                return CharismaGrab(unit) + BasicRoll_Disadvantage();
            default:
                return CharismaGrab(unit) + BasicRoll();
        }
    }
    public static int BasicRoll(int bonus, Reroll reroll)
    {
        switch (reroll)
        {
            case Reroll.Advantage:
                return bonus + BasicRoll_Advantage();
            case Reroll.Disadvantage:
                return bonus + BasicRoll_Disadvantage();
            default:
                return bonus + BasicRoll();
        }
    }
    public static int BasicRoll(CharacterManager unit, int bonus, Reroll reroll)
    {
        switch (reroll)
        {
            case Reroll.Advantage:
                return CharismaGrab(unit) + bonus + BasicRoll_Advantage();
            case Reroll.Disadvantage:
                return CharismaGrab(unit) + bonus + BasicRoll_Disadvantage();
            default:
                return CharismaGrab(unit) + bonus + BasicRoll();
        }
    }

    /// <summary>
    /// The Advantage and Disadvantage functions for basic roll both roll two basic rolls,
    /// and return the greater or lesser result, respectively.
    /// </summary>
    /// <returns> Result of the rolls </returns>
    private static int BasicRoll_Advantage()
    {
        int roll1 = BasicRoll();
        int roll2 = BasicRoll();

        if (roll1 > roll2) { return roll1; }
        else { return roll2; }
    }
    private static int BasicRoll_Disadvantage()
    {
        int roll1 = BasicRoll();
        int roll2 = BasicRoll();

        if (roll1 < roll2) { return roll1; }
        else { return roll2; }
    }

    // Custom Roll methods ==========

    /// <summary>
    /// The custom roll methods allow us to roll a different pool of dice other than 3 die and/or 6-sided die.
    /// Much like the BasicRoll above, the overloads receive additional bonuses and penalties, and make use of the
    /// root CustomRoll() function.
    /// </summary>
    /// <param name="number"> The number of dice to roll </param>
    /// <param name="size"> The size of the die; so X-sided </param>
    /// <returns> int result of the roll </returns>
    public static int CustomRoll(int number, int size) 
    {
        if (number <= 0 || size <= 0) { return 0; } // Can't roll when either is 0

        size += 1;
        int roll = 0;
        // 1 roll per # of dice
        for (int i = 0; i < number; i++) { roll += Random.Range(1, size); }
        // Return
        return roll;
    }
    public static int CustomRoll(CharacterManager unit, int number, int size)
    {
        return CustomRoll(number, size) + CharismaGrab(unit);
    }
    public static int CustomRoll(int number, int size, int bonus)
    {
        return CustomRoll(number, size) + bonus;
    }
    public static int CustomRoll(CharacterManager unit, int number, int size, int bonus)
    {
        if (number <= 0 || size <= 0) { return 0; } // Can't roll when either is 0
        return CustomRoll(number, size) + CharismaGrab(unit) + bonus;
    }
    public static int CustomRoll(CharacterManager unit, int number, int size, Reroll reroll)
    {
        if (number <= 0 || size <= 0) { return 0; } // Can't roll when either is 0
        int roll = CharismaGrab(unit);

        switch (reroll)
        {
            case Reroll.Advantage:
                return roll + CustomRoll_Advantage(number, size);
            case Reroll.Disadvantage:
                return roll + CustomRoll_Disadvantage(number, size);
            default:
                return roll + CustomRoll(number, size);
        }
    }
    public static int CustomRoll(int number, int size, int bonus, Reroll reroll)
    {
        if (number <= 0 || size <= 0) { return 0; } // Can't roll when either is 0

        switch (reroll)
        {
            case Reroll.Advantage:
                return bonus + CustomRoll_Advantage(number, size);
            case Reroll.Disadvantage:
                return bonus + CustomRoll_Disadvantage(number, size);
            default: // If the flag is set to No, just do a normal roll
                return bonus + CustomRoll(number, size);
        }
    }
    public static int CustomRoll(CharacterManager unit, int number, int size, int bonus, Reroll reroll)
    {
        if (number <= 0 || size <= 0) { return 0; } // Can't roll when either is 0
        bonus += CharismaGrab(unit);

        switch (reroll)
        {
            case Reroll.Advantage:
                return bonus + CustomRoll_Advantage(number, size);
            case Reroll.Disadvantage:
                return bonus + CustomRoll_Disadvantage(number, size);
            default: // If the flag is set to No, just do a normal roll
                return bonus + CustomRoll(number, size);
        }
    }

    /// <summary>
    /// The Advantage and Disadvantage functions for custom roll both roll work
    /// exactly like the basic roll ones, only we can choose the amount of die and
    /// their size. We return the greater or lesser result, respectively.
    /// </summary>
    /// <returns> Result of the rolls </returns>
    private static int CustomRoll_Advantage(int number, int size)
    {
        int roll1 = CustomRoll(number, size);
        int roll2 = CustomRoll(number, size);

        if (roll1 > roll2) { return roll1; }
        else { return roll2; }
    }
    private static int CustomRoll_Disadvantage(int number, int size)
    {
        int roll1 = CustomRoll(number, size);
        int roll2 = CustomRoll(number, size);

        if (roll1 < roll2) { return roll1; }
        else { return roll2; }
    }

    /// <summary>
    /// CharismaGrab() checks if a unit is under the effects of Charisma for the sake of
    /// grabbing it's bonuses.
    /// </summary>
    /// <param name="unit"> unit performing the roll </param>
    /// <returns> Bonus from the status effect </returns>
    private static int CharismaGrab(CharacterManager unit)
    {
        if (unit.unitEffects.charisma)
        {
            Status_Charisma charisma = unit.unitEffects.CharismaCheck();
            int bonus = charisma.GetBonus(); // Save the bonus here
            unit.unitEffects.RemoveEffect(charisma); // Remove this effect; only works on 1 roll per turn
            return bonus;
        }
        else { return 0; }
    }
}

