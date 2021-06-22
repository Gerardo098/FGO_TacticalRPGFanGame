using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack_Player is the subclass of AttackAction used by player-controlled characters 
/// </summary>
public class Attack_Player : AttackAction
{
    /// <summary>
    /// Update(); in this update function, we check on whether the unit is attacking
    /// or not. If they are not, we check button clicks for a target. If they are, we run the
    /// Attack() script from AttackAction.
    /// </summary>
    void Update()
    {
        // TODO
        // FindSelectable()  - create an "aura" that shows how what units you can target
        if (!attacking) { CheckMouse(); } // If not attacking, check our mouse click
        else { Attack(); } // If attacking, then attack
    }

    /// <summary>
    /// CheckMouse() uses a raycasthit to check where we've clicked on the screen
    /// in accordance to the camera's position.
    /// </summary>
    public void CheckMouse()
    {
        // Check our mouse click
        if (Input.GetMouseButtonUp(0))
        {
            // Cast a ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // If we've hit something, check if it has a CharacterManager script
                if (hit.collider.GetComponent<CharacterManager>())
                {
                    // If so, grab the target's character manager script.
                    CharacterManager pTarget = hit.collider.GetComponent<CharacterManager>();

                    // Calculate the distance 
                    float distance = Vector3.Distance(charMan.transform.position, pTarget.transform.position);
                    CheckWeaponRange();
                    Debug.Log("Distance to target = " + distance);

                    // If the target is within range AND is of the opposite team, we can attack
                    if (distance <= attackRange && !charMan.character.CompareTeams(pTarget))
                    {
                        attacking = true; // The character will now attack
                        target = pTarget;
                    }
                    else
                    {
                        // TESTING - if we're out of range, plz tell us that
                        Debug.Log("Illegal target or target out of range.");
                    }
                }
            }
        }
    }
}
