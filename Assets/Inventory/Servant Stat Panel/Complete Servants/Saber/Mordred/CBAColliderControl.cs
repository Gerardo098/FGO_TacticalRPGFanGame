using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CBAColliderControl is a class handling the collider 
/// for the special attack "Clarent Blood Arthur".
/// 
/// The collider moves forward in a straight line, applying a NP instance 
/// of CBA onto all units in its path.
/// The collider exists long enough to travel 100m / 100 unity "units" in 5 seconds.
/// </summary>
public class CBAColliderControl : MonoBehaviour
{
    public float moveSpeed; // How fast the collider moves
    private CharacterManager source; // Source unit that used the special attack
    private Ability NP; // The Noble Phantasm in question

    //public GameObject target;
    private Vector3 velocity = new Vector3(); // Velocity of the collider
    private Vector3 heading = new Vector3(); // Direction of the collider
    private float count; // Time count

    /// <summary>
    /// Init() function that sets the collider's script up
    /// </summary>
    /// <param name="_source"> source unit </param>
    /// <param name="target"> target object we're aiming at </param>
    /// <param name="_NP"> NP in question </param>
    public void init(CharacterManager _source, GameObject target, Ability _NP)
    {
        // Save the source unit and ability
        source = _source;
        NP = _NP;
        count = 0; // Init the time counter

        // Calculate the heading and velocity
        heading = target.transform.position - transform.position;
        velocity = heading * moveSpeed;
    }

    /// <summary>
    /// In the update function, the collider is moved ahead 
    /// </summary>
    void Update()
    {
        transform.forward = heading;
        transform.position += velocity * Time.deltaTime;
        count += Time.deltaTime;
        // Once we've traveled for 5 seconds, use up all the source unit's actions
        // and destroy the collider
        if (count >= 5f)
        {
            source.character.FullAction();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when another collider is hit.
    /// We check if the collider has a CharacterManager script (if it's a unit).
    /// If so, give that unit an instance of our NP to handle.
    /// </summary>
    /// <param name="other"> Other collider that our own struck in its travels </param>
    private void OnTriggerEnter(Collider other)
    {
        // Check for the script CharacterManager
        CharacterManager unit = other.GetComponent<CharacterManager>();
        if (unit != null) // If the CharMan script is not null, hand them the NP
        {
            CBA_Instance CBA = new CBA_Instance(source, unit, NP);
            unit.AbilityInstanceRead(CBA);
        }
    }
}
