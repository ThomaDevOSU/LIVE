using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Mark_GeneralStoreOwner : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Mark.
    /// </summary>
    public NPC Mark;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Mark script started");
        Mark = new NPC
        {
            Greeting = "Oh hey... Need something? Or are we just hanging around?",
            inDialogue = false,
            ID = 4,
            Name = "Mark",
            Job = "Merchant",
            Description = "Mark is the laid-back owner of Babel’s general store. He inherited the store but runs it in his own relaxed, " +
            "somewhat disorganized style. He’s philosophical, mixing hippie vibes with deep life lessons—often while half-napping behind the counter. " +
            "Mark is devoted to his wife Amy, whose serious nature contrasts his easygoing approach to life. He’s proud of his daughter Jessica but secretly " +
            "wishes she spent more time at home. Mark enjoys chatting with customers, even if he forgets what they were buying mid-conversation. " +
            "Garbanzo technically belongs to him, but he lets the dog wander freely, believing in 'free-range' pet ownership.",
            Personality = new List<string> { "Laid-back", "Philosophical", "Forgetful", "Family-oriented" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(2, 4),
                    Location = "Store"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(6, 7),
                    Location = "Square"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 3),
                    Location = "Park"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Store",
            CurrentCoordinates = new Vector2(2, 4)
        };
        NPCManager.Instance.AddNPC(Mark);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Mark.inDialogue = true;
            Debug.Log("Mark in dialogue");
            DialogueManager.Instance.StartDialogue(Mark);
        }
    }
}

