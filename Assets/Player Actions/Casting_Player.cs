using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Casting_Player is the subclass of CastingAction used exclusively by player characters
/// to cast spells
/// </summary>
public class Casting_Player : CastingAction
{
    /// <summary>
    /// Update() is used to check whether the player is casting a spell or not.
    /// </summary>
    private void Update()
    {
        // TODO
        // FindSelectable()  - this version would create an "aura" that shows how far a target has to be for you to be
        // able to attack it

        if (!casting) // If the player is not casting, check their mouse click to see if they've targeted a unit
        {
            InitiateList(); // Ready list
            CheckMouse_Type(); // Find target(s)
        }
        // If the unit is casting, begin casting the spell
        else
        {
            // Call the coroutine to cast our spell
            StartCoroutine(Cast());
        }
    }

    /// <summary>
    /// InitiateList() keeps the target list ready to receive potential targets for our spells
    /// </summary>
    private void InitiateList()
    {
        // If the list is null, create a new one
        if (targetList == null) { targetList = new List<CharacterManager>(); }
        // If the list exists but is not empty, clear it
        if (targetList.Count > 0) { targetList.Clear(); }
    }

    /// <summary>
    /// CheckMouse_Type() checks whether the spell requires a specific target
    /// or can choose any target
    /// </summary>
    private void CheckMouse_Type()
    { 
        // Check the current spell's target type
        switch (currentSpell.targetType)
        {
            case TargetType.Any: // If the spell can target anything:
                StartCoroutine(TargetAny());
                break;
            default: // If the spell must target a specific object/unit:
                StartCoroutine(TargetSpecific());
                break;
        }
    }

    /// <summary>
    /// TargetAny() is a coroutine function that uses a raycasthit to check where we've clicked on the screen
    /// in accordance to the camera's position. This checks for *ANY* gameobject so long as it has a charManager.
    /// </summary>
    /// <returns> IEnumerator </returns>
    private IEnumerator TargetAny()
    {
        // Check our mouse click
        if (Input.GetMouseButtonUp(0))
        {
            // Cast a ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPosition = hit.collider.transform.position;
                LookAtTarget(gameObject.transform, hitPosition); // Turn towards the target

                // Calculate the distance 
                float distance = Vector3.Distance(characterManager.transform.position, hitPosition);
                if (distance <= currentSpell.ReturnRange()) // If our target is within range
                {
                    Collider[] foundColliders = Physics.OverlapSphere(hitPosition, currentSpell.ReturnAoE());
                    foreach (Collider collider in foundColliders)
                    {
                        // If there is a character within the sphere, add it to the target list
                        if (collider.GetComponent<CharacterManager>())
                        {
                            targetList.Add(collider.GetComponent<CharacterManager>());
                        }
                    }
                    // If we found at least 1 target
                    // The character will now cast the spell
                    if (targetList.Count > 0) { casting = true; }
                }
                else
                {
                    Debug.Log("Target location out of casting range.");
                }
            }
        }

        // Return
        yield return null;
    }

    /// <summary>
    /// TargetSpecific() is a coroutine function that uses a raycasthit to check where we've clicked on the screen
    /// in accordance to the camera's position. This function checks for a character of a certain team (good or evil)
    /// </summary>
    /// <returns> IEnumerator </returns>
    private IEnumerator TargetSpecific()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Cast a ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<CharacterManager>())
                {
                    CharacterManager pTarget = hit.collider.GetComponent<CharacterManager>();
                    // Calculate the distance 
                    float distance = Vector3.Distance(characterManager.transform.position, pTarget.transform.position);
                    bool targetable = false;
                    // Switch that looks at our current spell's target type
                    switch (currentSpell.targetType)
                    {
                        case TargetType.Self:
                            // If we target outselves
                            targetable = pTarget == characterManager.character;
                            break;
                        case TargetType.Friendly:
                            // If the target is friendly AND within range (can target ourselves as well)
                            targetable = (distance <= currentSpell.ReturnRange() && pTarget.character.unitTeam == characterManager.character.unitTeam);
                            break;
                        case TargetType.Hostile:
                            // If the target is hostile AND within range (can target ourselves as well)
                            targetable = (distance <= currentSpell.ReturnRange() && pTarget.character.unitTeam != characterManager.character.unitTeam);
                            break;
                    }

                    // If the target is a legal target, cast the spell
                    if (targetable)
                    {
                        targetList.Add(pTarget); // Add the target to the target list
                        casting = true; // The character will now cast the spell

                    }
                    else // else
                    {
                        Debug.Log("Illegal target for the current spell.");
                    }
                }
            }
        }
        // Return
        yield return null;
    }
}
