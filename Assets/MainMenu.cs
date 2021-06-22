using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// MainMenu is the script used by the main menu scene to start the game OR quit the application
/// </summary>
public class MainMenu : MonoBehaviour
{
    // List of scene select buttons
    public List<MenuSceneButton> sceneSelectButtons;

    /// <summary>
    /// StartGame() opens the first scene in the game
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Dialogue_00_Intro");
    }

    /// <summary>
    /// QuitGame() exits the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// SceneSelectInit() is called when the scene selection panel is activated.
    /// This makes sure all buttons have the correct names associated to them.
    /// </summary>
    public void SceneSelectInit()
    {
        foreach (MenuSceneButton button in sceneSelectButtons)
        {
            // Call the init function to set them up
            button.InitButton();
        }
    }
}
