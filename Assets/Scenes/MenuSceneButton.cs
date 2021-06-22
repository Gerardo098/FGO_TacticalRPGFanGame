using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// MenuSceneButton is a simple script handling the loading of a specific scene of the
/// player's choosing when clicked. This is so the player can skip scenes they've already played.
/// </summary>
public class MenuSceneButton : MonoBehaviour
{
    // Name of the scene this button will try to load
    public string scene;
    public string sceneName; // More readable name for the button text
    public TextMeshProUGUI text; // button text

    /// <summary>
    /// InitButton() sets the button's name to the sceneName variable
    /// </summary>
    public void InitButton() { text.text = sceneName; }

    /// <summary>
    /// LoadScene() is called when the button is clicked,
    /// loading the scene referred to by our scene variable.
    /// </summary>
    public void LoadScene() { if (scene != null) { SceneManager.LoadScene(scene); } }
}
