using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The MPBar class handles changing a unit's MP bar whenever their current MP value changes.
/// 
/// We use a slider to represent how full their MP is.
/// Created using an example by Code Monkey on youtube then built upon 
/// https://www.youtube.com/watch?v=cR8jP8OGbhM
/// </summary>
public class MPBar : MonoBehaviour
{
    [SerializeField]
    private Image mpBar; // healthbar graphic
    private float updateSpeed = 0.5f; // How fast the slider moves

    /// <summary>
    /// Init() saves our ChangeHealth() function as an action in the character script's OnHealthChange variable.
    /// </summary>
    internal void Init()
    {
        GetComponentInParent<CharacterManager>().character.OnMPChange += ChangeMP;
    }

    /// <summary>
    /// ChangeHealth() activates a coroutine to change the slider a certain percentage
    /// </summary>
    /// <param name="MPAmount"> percentage (taken in as decimal) of the total HP we have at the moment </param>
    private void ChangeMP(float MPAmount) { StartCoroutine(ChangePercent(MPAmount)); }

    /// <summary>
    /// ChangePercent() is a coroutine function that fills/empties the HP slider bit by bit 
    /// according to the updateSpeed float.
    /// </summary>
    /// <param name="MPAmount"> percentage (taken in as decimal) of the total HP we have at the moment </param>
    /// <returns> IEnumerator </returns>
    private IEnumerator ChangePercent(float MPAmount)
    {
        // Current amount in the slider
        float preChange = mpBar.fillAmount;
        float elapsed = 0f; // Time elapsed

        // In the while loop, we use Lerp to gradually fill/empty the health bar
        // rather than have it instantly change.
        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            mpBar.fillAmount = Mathf.Lerp(preChange, MPAmount, elapsed / updateSpeed);
            yield return null;
        }
        // the healthBar's fill amount is then replaced by our new current amount
        mpBar.fillAmount = MPAmount;
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
