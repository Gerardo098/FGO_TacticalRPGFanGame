using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// VictoryCanvas gives function to all buttons on the victory screen
/// </summary>
public class VictoryCanvas : MonoBehaviour
{
    // Name of the scene this script is in; used by the restart button
    public string scene;

    // Init() simply sets the gameobject to active
    public void Init()
    {
        gameObject.SetActive(true);
    }

    // Next() brings the player to the next scene
    public void Next()
    {
        SceneManager.LoadScene(scene);
    }

    // MainMenu() brings us back to the main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
