using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UseAbility_Character is the subclass of AbilityUseAction used by player-controlled characters 
/// </summary>
public class UseAbility_Character : AbilityUseAction
{
    public bool usingAction; // Bool flag to check if we're using an ability

    /// <summary>
    /// Update(); in this update function, we check on whether the unit is using an active 
    /// ability or not. If they are not, we check button clicks for a target. 
    /// If they are, we run the ActivateAbility() coroutine.
    /// </summary>
    void Update()
    {
        // TODO
        // FindSelectable()  - this version would create an "aura" that shows how far a target has to be for you to be
        // able to attack it

        // If not using an action, check our mouse for legal targets
        if (!usingAction) { CheckMouse_Type(); }
        // If we are, start the coroutine
        else
        {
            StartCoroutine(ActivateAbility());
            this.enabled = false;
        }
    }

    /// <summary>
    /// ActivateAbility() is a coroutine function activates the ability and spends the
    /// MP to activate it (if required)
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator ActivateAbility()
    {
        // Check if we can spend the MP necessary to activate the ability
        if (charMan.skillController.SpendMana(ability.GetMPCost()))
        {
            ability.Activate(charMan, currentTarget.gameObject); // Activate it
            AbilityActionCount(); // See if the ability costs actions ot use
        }
        // If we can't afford it, let the user know.
        else { Debug.Log("Unable to activate ability; not enough MP/HP to cast."); }

        usingAction = false; // No longer using an ability

        // Return
        yield return null;
    }

    /// <summary>
    /// CheckMouse() uses a raycasthit to check where we've clicked on the screen
    /// in accordance to the camera's position. This function checks for a 
    /// character of a certain team (good or evil)
    /// </summary>
    private void CheckMouse_Type()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Cast a ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // If the collider has a CharacterRoot script
                if (hit.collider.GetComponent<Character_Root>())
                {
                    // Save it, and calculate the distance
                    Character_Root pTarget = hit.collider.GetComponent<Character_Root>();
                    float distance = Vector3.Distance(charMan.character.gameObject.transform.position, pTarget.gameObject.transform.position);

                    bool targetable = false;
                    // Switch that looks at our current spell's target type
                    switch (ability.targetType)
                    {
                        case TargetType.Self:
                            // If we target outselves
                            targetable = pTarget == charMan.character;
                            break;
                        case TargetType.Friendly:
                            // If the target is friendly AND within range (can target ourselves as well)
                            targetable = (distance <= ability.GetRange() && pTarget.unitTeam == charMan.character.unitTeam);
                            break;
                        case TargetType.Hostile:
                            // If the target is hostile AND within range (can target ourselves as well)
                            targetable = (distance <= ability.GetRange() && pTarget.unitTeam != charMan.character.unitTeam);
                            break;
                    }

                    // If the target is a legal target, cast the spell
                    if (targetable)
                    {
                        usingAction = true; // The character will now cast the spell
                        currentTarget = pTarget;
                    }
                    else // else
                    {
                        Debug.Log("Illegal target for the current ability.");
                    }
                }
            }
        }
    }
}
