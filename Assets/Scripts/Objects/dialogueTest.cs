using UnityEngine;

public class dialogueTest : MonoBehaviour
{
    public DialogueEntry dialogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogue = new DialogueEntry()
        {
            sentences = new string[] { "Hello! This is a test!" },
            hasChoices = true,
            choices = new DialogueChoice[]
            { 
                new DialogueChoice()
                { 
                    choiceText = "This is a response!",
                    nextDialogue = new DialogueEntry()
                    {
                        sentences = new string[]{ "This is a response to that!" },
                        hasChoices= false
                    }
                },
                new DialogueChoice()
                {
                    choiceText = "I won't be responding",
                    nextDialogue = new DialogueEntry()
                    {
                        sentences = new string[]{ "Well thats not nice" },
                        hasChoices= false
                    }
                }
            }
        };
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking) // Only if we arent talking or paused
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
