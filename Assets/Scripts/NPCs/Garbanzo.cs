using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Garbanzo_Dog : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Garbanzo.
    /// </summary>
    public NPC Garbanzo;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Garbanzo script started");
        Garbanzo = new NPC
        {
            Greeting = "Woof! Play? Stick? Ball? Teddy?",
            inDialogue = false,
            ID = 7,
            Name = "Garbanzo",
            Job = "Dog",
            Description = "Garbanzo is a simple-minded but endlessly enthusiastic golden retriever. " +
            "He is technically Mark’s dog but roams freely around Babel. He adores everyone but is obsessed " +
            "with Teddy the cat, constantly chasing him in an effort to play. His other favorite pastime is " +
            "playing soccer with Ace, where he mysteriously seems to understand the rules. He rarely speaks " +
            "more than a few words but expresses pure joy in every movement.",
            Personality = new List<string> { "Energetic", "Loyal", "Innocent", "Playful" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 5),
                    Location = "Park"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(3, 3),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(5, 5)
        };
        NPCManager.Instance.AddNPC(Garbanzo);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Garbanzo.inDialogue = true;
            Debug.Log("Garbanzo in dialogue");
            DialogueManager.Instance.StartDialogue(Garbanzo);
        }
    }
}
