using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// DialogueManager is the big dialogue script that handles all dialogue in a dialogue scene.
/// Like stated in the other scripts, this game uses a visual novel-style dialogue system, which we try to replicate
/// </summary>
public enum DialogueSequence { None, Options, NextDialogue, NewScene }
public class DialogueManager : MonoBehaviour
{
    // Scene control
    public string scene;
    // Text control
    private readonly List<char> punctuation = new List<char> { '.', ',', '!', '?' };
    public GameObject TextPanel;
    public GameObject namePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    // Images
    public Image background;
    public Image portraitRight;
    public Image portraitMiddle;
    public Image portraitLeft;
    // Options control
    private DialogueSequence sequenceFlag;
    private Dialogue currentDialogue;
    private DialogueOptionPanel optionsPanel;
    // Dialogue Queue
    public Dialogue FirstDialogue;
    private Queue<DialogueMember> dialogueQueue;
    private DialogueMember lastSentence;

    /// <summary>
    /// Start() prepares the manager's queue, makes sure we have a reference to the options panel and runs its init() function
    /// Finally, begin the dialogue.
    /// </summary>
    private void Start()
    {
        dialogueQueue = new Queue<DialogueMember>(); // Create the queue
        // Find and init the options panel
        if (optionsPanel == null) { optionsPanel = FindObjectOfType<DialogueOptionPanel>(); }
        optionsPanel.init(this);
        StartDialogue(FirstDialogue); // Start the dialogue
    }

    /// <summary>
    /// StartDialogue() opens the first dialogue and begins reading each DialogueMember from it.
    /// At the same time, it 
    /// </summary>
    /// <param name="dialogue"></param>
    internal void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        CleanManager(); // Empty the queue

        SequenceHandler(currentDialogue.sequence); // Check if the dialogue has options or not
        foreach (DialogueMember member in dialogue.dialogue) { EnqueueDialogue(member); }

        DisplayNextSentence();
    }

    /// <summary>
    /// SequenceHandler() handles what happens after the current dialogue is finished 
    /// based on the the amount of options the current dialogue has saved
    /// </summary>
    /// <param name="dialogues"> List of dialogues saved by the current dialogue; serves as options for the player to pick from </param>
    private void SequenceHandler(List<Dialogue> dialogues)
    {
        // If there's no dialogue after our current, set the flag to none and move to the next scene in the game
        if (dialogues == null || dialogues.Count == 0)
        {
            sequenceFlag = DialogueSequence.None;
            return; // Return
        }

        // If there's only 1 option after our current dialogue, set the flag to simply move onto it when we're ready
        if (dialogues.Count == 1) { sequenceFlag = DialogueSequence.NextDialogue; }
        // Else, let the manager know to ready itself to handle the player's dialogue options
        else { sequenceFlag = DialogueSequence.Options; }
    }

    /// <summary>
    /// DisplayNextSentence() presents an individual dialoguemember from our current dialogue.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (EndCheck()) // Check the count of the queue here
        {
            EndDialogue(); // Check the state of the DialogueSequence flag
            return; // Return to avoid the rest of the function
        }

        // Keep tabs on the last sentence we've grabbed
        lastSentence = dialogueQueue.Dequeue();
        // Erase portraits if the flag is set
        if (lastSentence.ErasePortraits) { ClearAllPortraits(); }

        BackgroundControl(lastSentence); // Handle the BG here
        NarratorCheck(lastSentence); // Check if we're dealing with a narrator here

        // Stop all coroutines
        // if we skip through dialogue, this ensures we don't have coroutines overlapping
        StopAllCoroutines();
        // Type the sentence in
        StartCoroutine(TypeSentence(lastSentence));
    }

    /// <summary>
    /// TypeSentence() is a coroutine function that types the text portion of a given
    /// dialogue member into the scene's text box. It is typed in letter by letter for more
    /// dramatic effect.
    /// </summary>
    /// <param name="sentence"></param>
    /// <returns></returns>
    IEnumerator TypeSentence(DialogueMember sentence)
    {
        dialogueText.text = ""; // Start with an empty text field
        // Go through the text letter by letter
        foreach (char letter in sentence.text.ToCharArray())
        {
            // Wait a tiny fraction of time before adding the char.
            yield return new WaitForSeconds(0.025f);
            dialogueText.text += letter;

            // If the last letter was a punctuation, wait a bit longer for a pause for our reader.
            if (CheckPunctuation(letter)) { yield return new WaitForSeconds(0.25f); }
        }
    }

    /// <summary>
    /// DisplayNextDialogue() is used by the dialogue scene's skip button.
    /// It skips the current dialogue and loads up the next one in queue, 
    /// or any dialogue options the current dialogue might have.
    /// </summary>
    public void DisplayNextDialogue() { EndDialogue(); }

    /// <summary>
    /// CheckPunctuation() takes a character variable and checks if it's contained by the punctuation list
    /// above; if so, return true. Else, false.
    /// </summary>
    /// <param name="letter"> char variable to check </param>
    /// <returns> bool flag on whether it is contained by the list or not </returns>
    private bool CheckPunctuation(char letter)
    {
        if (punctuation.Contains(letter)) { return true; }
        else { return false; }
    }

    /// <summary>
    /// FadeBackgroundIn() is a coroutine function that fades the
    /// BG sprite in from a black background to the base brightness and colour
    /// </summary>
    /// <param name="BG"></param>
    /// <returns></returns>
    IEnumerator FadeBackgroundIn(Sprite BG)
    {
        background.color = new Color(1f, 1f, 1f, 0f);
        background.sprite = BG;

        for (float f = 0.05f; f <= 1f; f += 0.05f)
        {
            Color colour = background.color;
            colour.a = f;
            background.color = colour;
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// EnqueueDialogue() simply takes a DialogueMember and adds it to our dialogue queue
    /// </summary>
    /// <param name="member"> DialogueMember to add to the queue </param>
    private void EnqueueDialogue(DialogueMember member) { dialogueQueue.Enqueue(member); }

    /// <summary>
    /// NarratorCheck() checks the dialogue member if the current "frame" is
    /// being narrated rather than being spoken by a character on screen.
    /// </summary>
    /// <param name="sentence"></param>
    private void NarratorCheck(DialogueMember sentence)
    {
        if (sentence.Narrator || sentence.actor == null) // If the flag is set to true...
        {
            namePanel.SetActive(false); // Turn the name panel off
            // In case the narrator flag is set and we have an actor in the actor field, place the actor's image on the screen
            if (sentence.actor != null && sentence.actor.portrait != null)
            {
                SpriteHandler(sentence.actor.portrait, sentence.PortraitPosition, sentence.layer);
            }
            DarkenAllPortraits(); // Darken all portraits
        }
        else // If this is not a narrated line
        {
            namePanel.SetActive(true); // Turn the name panel on
            nameText.text = sentence.actor.name; // Set the name panel's text to the actor's name
            // Set the actor's sprite on the screen if they have one
            if (sentence.actor.portrait != null)
            {
                SpriteHandler(sentence.actor.portrait, sentence.PortraitPosition, sentence.layer);
            }
            // if there's no picture (such as the protagonist speaking), darken all portraits
            else { DarkenAllPortraits(); }
        }
    }

    /// <summary>
    /// BackgroundControl() sets the background up, following the information
    /// found in the given DialogueMember
    /// </summary>
    /// <param name="sentence"> DialogueMember to read </param>
    private void BackgroundControl(DialogueMember sentence)
    {
        // If the EraseBG flag is active, set the background to dark
        if (sentence.EraseBG) { DarkBackground(); }

        if (sentence.background != null)
        {
            // If the background is not null, check if we should fade the background in
            if (sentence.FadeBGIn == true) { StartCoroutine(FadeBackgroundIn(sentence.background)); }
            // if not, simply set it without fancy effects
            else { SetBackground(sentence.background); }
        }
    }

    /// <summary>
    /// SpriteHandler() receives a sprite and finds out where to place it in the scene, based on the information
    /// found in the DialogueMember we're currently looking at.
    /// </summary>
    /// <param name="sprite"> Sprite to place on the screen </param>
    /// <param name="position"> Position enum of the sprite on the screen </param>
    /// <param name="layer"> Position enum of the sprite on the hierarchy </param>
    private void SpriteHandler(Sprite sprite, Position position, Layer layer)
    {
        switch (position)
        {
            case Position.Left:
                SH_Left(sprite, layer);
                break;
            case Position.Middle:
                SH_Middle(sprite, layer);
                break;
            case Position.Right:
                SH_Right(sprite, layer);
                break;
            case Position.None: // If we don't want the sprite to appear
            default:
                break;
        }
    }

    /// <summary>
    /// The SH_X() functions, short for SpriteHandler_Position, each handle their respective image
    /// position, right, middle, or left.
    /// </summary>
    /// <param name="sprite"> Sprite to set as our image </param>
    /// <param name="layer"> Layer enum telling us where to place the sprite </param>
    private void SH_Left(Sprite sprite, Layer layer)
    {
        // If there is no sprite, set the portrait to null and make it fully transparent
        if (sprite == null) { ClearSprite(portraitLeft); }
        // If there is a sprite, set the portrait to the given sprite and make it fully opaque
        else
        {
            portraitLeft.sprite = sprite;
            portraitLeft.color = Color.white;
        }
        // Darken the other sprites
        DarkenSprite(portraitMiddle);
        DarkenSprite(portraitRight);
        // Handle the position of the image, layer-wise
        ImageLayer(portraitLeft, layer);
    }
    private void SH_Middle(Sprite sprite, Layer layer)
    {
        // If there is no sprite, set the portrait to null and make it fully transparent
        if (sprite == null) { ClearSprite(portraitMiddle); }
        // If there is a sprite, set the portrait to the given sprite and make it fully opaque
        else
        {
            portraitMiddle.sprite = sprite;
            portraitMiddle.color = Color.white;
        }
        // Darken the other sprites
        DarkenSprite(portraitLeft);
        DarkenSprite(portraitRight);
        // Handle the position of the image, layer-wise
        ImageLayer(portraitMiddle, layer);
    }
    private void SH_Right(Sprite sprite, Layer layer)
    {
        // If there is no sprite, set the portrait to null and make it fully transparent
        if (sprite == null) { ClearSprite(portraitRight); }
        // If there is a sprite, set the portrait to the given sprite and make it fully opaque
        else
        {
            portraitRight.sprite = sprite;
            portraitRight.color = Color.white;
        }
        // Darken the other sprites
        DarkenSprite(portraitLeft);
        DarkenSprite(portraitMiddle);
        // Handle the position of the image, layer-wise
        ImageLayer(portraitRight, layer);
    }

    /// <summary>
    /// ImageLayer() sets the position of a given image; infront of the rest, behind them, or in the middle
    /// We do this by moving the image around in the sibling position of the hierarchy.
    /// </summary>
    /// <param name="image"> Image to set to position </param>
    /// <param name="layer"> Enum flag signifying where to place the image </param>
    private void ImageLayer(Image image, Layer layer)
    {
        // Switch case
        switch (layer)
        {
            // The last sibling is rendered last, thus it appears in front of the rest
            case Layer.Front:
                image.transform.SetAsLastSibling();
                break;
            // Set the image as the second sibling to be rendered, so it appears
            // in front of the first but behind the last
            case Layer.Middle:
                image.transform.SetSiblingIndex(1);
                break;
            // Set as first sibling, so it's rendered first and appears in the back
            case Layer.Back:
                image.transform.SetAsFirstSibling();
                break;
        }
    }

    /// <summary>
    /// ClearSprite() erases a given image and sets it colour to clear or transluscent.
    /// This is to remove a character's portrait from a scene.
    /// </summary>
    /// <param name="image"> image to erase </param>
    private void ClearSprite(Image image)
    {
        image.sprite = null;
        image.color = Color.clear;
    }

    /// <summary>
    /// ClearAllPortraits() removes all portraits from a frame by calling the ClearSprite()
    /// function on all 3 of them.
    /// </summary>
    private void ClearAllPortraits()
    {
        ClearSprite(portraitLeft);
        ClearSprite(portraitMiddle);
        ClearSprite(portraitRight);
    }

    /// <summary>
    /// DarkenSprite() darkens a given image halfway to black.
    /// Used to signify a character that is still in a dialogue, but not currently speaking
    /// </summary>
    /// <param name="image"> image to darken </param>
    private void DarkenSprite(Image image) { if (image.sprite != null) { image.color = new Color(0.5f, 0.5f, 0.5f); } }

    /// <summary>
    /// DarkenAllPortraits() calls the DarkenSprite() function on all 3 actor portraits
    /// Useful for when the narrator is speaking
    /// </summary>
    private void DarkenAllPortraits()
    {
        DarkenSprite(portraitLeft);
        DarkenSprite(portraitMiddle);
        DarkenSprite(portraitRight);
    }

    /// <summary>
    /// SetBackground() receives a sprite to place as the frame's background
    /// </summary>
    /// <param name="BG"> sprite to use as background </param>
    private void SetBackground(Sprite BG)
    {
        background.sprite = BG; // Save the sprite
        // Set the sprite's colour to white so we can actually see it
        background.color = Color.white;
    }
    /// <summary>
    /// DarkBackground() turns the background black; useful for suspenseful scenes or whatever
    /// </summary>
    private void DarkBackground() { background.color = Color.black; }

    /// <summary>
    /// CleanManager() empties the dialogue queue
    /// </summary>
    private void CleanManager()  { dialogueQueue.Clear(); }

    /// <summary>
    /// EndCheck() checks if we've hit the end of a dialogue; 
    /// that is, there's no more dialogue members in the dialogue to present
    /// </summary>
    /// <returns> bool flag if it's empty (true) or not (false) </returns>
    private bool EndCheck() { return (dialogueQueue.Count == 0); }

    /// <summary>
    /// ActivatePanel() sets the textPanel back to active, such as after the player has 
    /// selected their dialogue options.
    /// </summary>
    internal void ActivatePanel() { if (TextPanel != null) { TextPanel.SetActive(true); } }

    /// <summary>
    /// EndDialogue() checks the sequence flag at the end of a dialogue to find out what to do next.
    /// </summary>
    private void EndDialogue()
    {
        // Switch case to check the flag
        switch (sequenceFlag)
        {
            // If set to NextDialogue, load up the next dialogue and begin reading it
            case DialogueSequence.NextDialogue:
                StartDialogue(currentDialogue.sequence[0]);
                break;
            // If Options, call the options panel to handle the current dialogue's options
            case DialogueSequence.Options:
                optionsPanel.UpdatePanel(currentDialogue.sequence);
                TextPanel.SetActive(false); // Set the text panel to inactive so it's out of the way
                break;
            // If set to none, end the conversation and move onto the next frame
            case DialogueSequence.None:
            default:
                if (currentDialogue.nextScene) { SceneManager.LoadScene(scene); }
                break;
        }
    }
}
