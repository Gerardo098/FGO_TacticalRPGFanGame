using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialogue is a scriptable object that contains a list of DialogueMembers (see below).
/// This list is then read by the dialogue system and presented to the player as a visual novel styled cutscene.
/// </summary>
public enum Position { None, Left, Middle, Right }
public enum Layer { Front, Middle, Back }

[CreateAssetMenu(menuName = "Data/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string optionText; // Option text, if this dialogue is itself an optional branch
    public bool nextScene; // Flag tells us when to move to next scene
    public List<Dialogue> sequence; // Options and "next scene"
    public DialogueMember[] dialogue; // List of dialogue members
}

/// <summary>
/// A DialogueMembers is a "frame" in a dialogue, comparable to a single frame in film.
/// Each dialoguemember allows the option to control what actor is "talking", their position,
/// the background, and what is being said.
/// </summary>
[System.Serializable]
public class DialogueMember
{
    // Actor control
    public Actor actor; // Actor in the scene
    public bool Narrator; // Narrator flag; this means there's no "actor", instead the "narrator" is speaking

    // Picture control
    public Position PortraitPosition; // Where we want the portrait in the scene; left, middle, right?
    public Layer layer; // What layer we want the portrait on; behind the other actors or infront of them?
    public bool ErasePortraits; // Erase flag; clear all portraits from the frame
    
    // BG control
    public Sprite background; // Background for this frame
    public bool FadeBGIn; // Fade BG flag; fade the background in so it looks betetr
    public bool EraseBG; // Like ErasePortraits, a flag that clears the background

    // Text control - what is being said in this scene
    [TextArea(3, 10)]
    public string text;
}