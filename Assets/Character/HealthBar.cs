using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The HealthBar class handles changing a unit's HP bar whenever their HP value changes.
/// 
/// We use a slider to represent how full their HP is.
/// Created using an example by Code Monkey on youtube then built upon 
/// https://www.youtube.com/watch?v=cR8jP8OGbhM
/// </summary>

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar; // healthbar graphic
    private float updateSpeed = 0.5f; // How fast the slider moves

    /// <summary>
    /// Init() saves our ChangeHealth() function as an action in the character script's OnHealthChange variable.
    /// </summary>
    internal void Init()
    {
        GetComponentInParent<CharacterManager>().character.OnHealthChange += ChangeHealth;
    }

    /// <summary>
    /// ChangeHealth() activates a coroutine to change the slider a certain percentage
    /// </summary>
    /// <param name="HPAmount"> percentage (taken in as decimal) of the total HP we have at the moment </param>
    private void ChangeHealth(float HPAmount) { StartCoroutine(ChangePercent(HPAmount)); }

    /// <summary>
    /// ChangePercent() is a coroutine function that fills/empties the HP slider bit by bit 
    /// according to the updateSpeed float.
    /// </summary>
    /// <param name="HPAmount"> percentage (taken in as decimal) of the total HP we have at the moment </param>
    /// <returns> IEnumerator </returns>
    private IEnumerator ChangePercent(float HPAmount)
    {
        // Current amount in the slider
        float preChange = healthBar.fillAmount;
        float elapsed = 0f; // Time elapsed

        // In the while loop, we use Lerp to gradually fill/empty the health bar
        // rather than have it instantly change.
        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            healthBar.fillAmount = Mathf.Lerp(preChange, HPAmount, elapsed / updateSpeed);
            yield return null;
        }
        // the healthBar's fill amount is then replaced by our new current amount
        healthBar.fillAmount = HPAmount;
    }

    /// <summary>
    /// LateUpdate() is used to have the healthbar face the camera
    /// </summary>
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
