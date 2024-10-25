using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject DialogueMenu; // Our dialogue menu
    public Queue<string> sentences; // Sentences to be displayed

    public TMP_Text dialogueText; // Text that will recieve response

    public GameObject offlinePanel; // For when GPT does not work 
    public GameObject choicePanel; // For when GPT does not work
    public GameObject dialogueButton; // For when GPT does not work

    public GameObject onlinePanel; // For when GPT works
    public TMP_InputField inputField; // For when GPT works

    private DialogueEntry currentDialogue; // The current plugged dialogue

    private bool skipSentence; // Whether we skip to the end of the sentence or move on

    public bool isTalking = false; // Are we currently talking to someone?

    private void Awake() // Singleton
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

    public void Update()
    {
        if (isTalking && Input.GetButtonDown("Jump")) // Skip sentence if we press button
        {
            skipSentence = true;
        }
    }


    public void SkipSentence() // For buttons to call on dialogue window 
    {
        skipSentence=true;
    }

    public void StopDialogue() // Disables dialogue menu and sets not talking
    {
        isTalking = false;
        sentences.Clear();
        currentDialogue = null;
        dialogueText.text = "";
        DialogueMenu.SetActive(false);
    }

    public void StartDialogue(DialogueEntry dialouge) // Starts Dialogue process
    {
        isTalking = true;
        DialogueMenu.SetActive(true);
        destroyChildren();

        currentDialogue = dialouge; // Set our dialogue variable

        foreach (string s in dialouge.sentences) // Load up our Queue
        {
            sentences.Enqueue(s);
        }

        DisplayNextSentence(); // Begin the displaying
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) // Check if no sentence are left
        {
            EndDialogue(); // End dialogue
            return;
        }

        string sentence = sentences.Dequeue(); // Load the sentence

        StopAllCoroutines();  // Stop previous typing effect if any.

        StartCoroutine(TypeSentence(sentence)); // Start typing the loaded sentence
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = ""; // Clear the text 

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.08f);  // Adjustable typing speed.

            if (skipSentence) // If we opt to skip the sentence
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

    void EndDialogue()
    {
        if (GameManager.Instance.isOnline())
        {
            onlinePanel.SetActive(true);
        }
        else if (currentDialogue.hasChoices)
        {
            DisplayChoices(currentDialogue.choices);
        }
        else 
        {
            StopDialogue();
        }
    }

    void DisplayChoices(DialogueChoice[] choices) // Disaplays choice buttons
    {
        
        foreach (var choice in choices)
        {
            GameObject button = Instantiate(dialogueButton, choicePanel.transform);
            button.GetComponentInChildren<TMP_Text>().text = choice.choiceText;
            button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
        }

        choicePanel.SetActive(true);
    }

    void OnChoiceSelected(DialogueChoice choice) // This will simply start the response dialogue
    {
        StartDialogue(choice.nextDialogue);
    }

    void destroyChildren() 
    {
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);  // Clear previous choices.
        }
    }

    public void sendData() // This will be the function that initiates the API call
    {
        onlinePanel.SetActive(false);
    }

}
