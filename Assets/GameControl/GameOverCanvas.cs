using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameOverCanvas gives function to all buttons on the game over screen 
/// </summary>
public class GameOverCanvas : MonoBehaviour
{
    // Name of the scene this script is in; used by the restart button
    public string scene;

    // Init() simply sets the gameobject to active
    public void Init()
    {
        gameObject.SetActive(true);
    }

    // Restart() is used by the canvas' restart button, reloading the scene when clicked
    public void Restart()
    {
        SceneManager.LoadScene(scene);
    }

    // MainMenu() brings us back to the main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
