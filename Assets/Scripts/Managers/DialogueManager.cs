using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages dialogue interactions, including displaying sentences, handling player input, and interacting with the GPTService.
/// </summary>
public class DialogueManager : MonoBehaviour
{

    public static DialogueManager Instance;

    public NPC currentNPC;

    /// <summary>
    /// The UI panel for displaying the dialogue menu.
    /// </summary>
    public GameObject DialogueMenu;

    /// <summary>
    /// Queue of sentences to display during dialogue.
    /// </summary>
    public Queue<string> sentences;

    /// <summary>
    /// TextMeshPro element for displaying dialogue text.
    /// </summary>
    public TMP_Text dialogueText;

    /// <summary>
    /// UI panel shown in offline mode.
    /// </summary>
    public GameObject offlinePanel;

    /// <summary>
    /// Panel for displaying choice buttons in offline mode
    /// </summary>
    public GameObject choicePanel;

    /// <summary>
    /// Button for progressing dialogue in offline mode.
    /// </summary>
    public GameObject dialogueButton;

    /// <summary>
    /// UI panel shown when in online mode.
    /// </summary>
    public GameObject onlinePanel;

    /// <summary>
    /// Input field for player text input when in online mode.
    /// </summary>
    public TMP_InputField inputField;

    /// <summary>
    /// Holds the GPTService's response to the player's input.
    /// </summary>
    private string response;

    /// <summary>
    /// The current dialogue entry being displayed.
    /// </summary>
    private DialogueEntry currentDialogue;

    /// <summary>
    /// Flag indicating whether to skip the current sentence.
    /// </summary>
    private bool skipSentence = false;

    /// <summary>
    /// Indicates whether the player is currently in a dialogue interaction.
    /// </summary>
   public bool isTalking = false;

    /// <summary>
    /// Ensures only one instance of DialogueManager exists and persists across scenes. Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sentences = new Queue<string>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Monitors player input to skip sentences, send input, or stop dialogue.
    /// </summary>
    public void Update()
    {
        if (isTalking && Input.GetMouseButton(1)) // Right-click to skip dialogue
        {
            skipSentence = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string input = inputField.text;
            inputField.text = "";
            sendData(input);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopDialogue();
        }
    }

    /// <summary>
    /// Stops the dialogue, clears data, and hides the dialogue menu.
    /// </summary>
    public void StopDialogue()
    {
        currentNPC.inDialogue = false;
        currentNPC = null;
        isTalking = false;
        sentences.Clear();
        currentDialogue = null;
        dialogueText.text = "";
        DialogueMenu.SetActive(false);
    }

    /// <summary>
    /// Starts a new dialogue interaction. Currently only works in online mode. Offline mode will require this method take a DialogueEntry as a parameter.
    /// </summary>
    public void StartDialogue(NPC npc)
    {
        currentNPC = npc;
        isTalking = true;
        DialogueMenu.SetActive(true);
        destroyChildren();


        DialogueEntry dialogue = new DialogueEntry
        {
            sentences = new string[] {  currentNPC.Greeting } // Placeholder greeting
        };

        currentDialogue = dialogue;

        // This queue will be populated with dialogue from the DialogueEntry. This is for offline mode.
        // Currently the dialogue will only contain the most recent response from the GPTService.
        foreach (string s in dialogue.sentences)
        {
            sentences.Enqueue(s);
        }
        DisplayNextSentence();
    }

    /// <summary>
    /// Displays the next sentence in the dialogue queue.
    /// </summary>
    private void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    /// <summary>
    /// Types out a sentence character by character with an adjustable typing speed.
    /// Allows skipping to the end of the sentence.
    /// </summary>
    /// <param name="sentence">The sentence to type out.</param>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.08f); // Typing speed.

            if (skipSentence)
            {
                skipSentence = false;
                dialogueText.text = sentence;
                break;
            }
        }

        while (!skipSentence) { yield return new WaitForSeconds(0.1f); }
        skipSentence = false;
        DisplayNextSentence();
    }

    /// <summary>
    /// Clears all children objects in the choice panel. This is an offline mode method.
    /// </summary>
    void destroyChildren()
    {
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Sends the player's input to the GPTService and handles the response.
    /// </summary>
    /// <param name="input">The player's input text.</param>
    public void sendData(string input)
    {
        StopAllCoroutines();
        GPTService.Instance.response = null;
        StartCoroutine(GPTService.Instance.ApiCall(input, currentNPC));
        StartCoroutine(WaitForResponse());
    }

    /// <summary>
    /// Waits for a response from the GPTService and processes it.
    /// </summary>
    IEnumerator WaitForResponse()
    {
        while (GPTService.Instance.response == null) { yield return new WaitForSeconds(0.1f); }
        response = GPTService.Instance.response;

        if (response == null)
        {
            Debug.Log("GPT failed to respond");
        }
        else
        {
            sentences.Enqueue(response);
            DisplayNextSentence();
        }
    }
}
