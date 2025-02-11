using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Alex_Barista : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Alex.
    /// </summary>
    public NPC Alex;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Alex_Barista script started");
        Alex = new NPC
        {
            Greeting = "Hey there! Need a caffeine boost, or just here for the vibes?",
            inDialogue = false,
            ID = 2,
            Name = "Alex",
            Job = "Barista",
            Description = "Alex is a passionate barista and the owner of the Babbling Bean café. He left behind the big city to create a cozy space where people " +
            "can connect over great coffee. Enthusiastic about brewing techniques, he’s always experimenting with new flavors and pastries—though baking is still a work in progress. " +
            "He looks up to Pattie for baking guidance but teases her about her refusal to make pies, unaware of her past. " +
            "Friendly and energetic, Alex enjoys chatting with customers and making sure everyone gets just the right drink for their mood.",
            Personality = new List<string> { "Friendly", "Enthusiastic", "Health-conscious", "Passionate about coffee" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 5),
                    Location = "Babbling Bean Café"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(10, 10),
                    Location = "Town Square"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(7, 3),
                    Location = "Bakery"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Babbling Bean Café",
            CurrentCoordinates = new Vector2(5, 5)
        };
        NPCManager.Instance.AddNPC(Alex);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Alex.inDialogue = true;
            Debug.Log("Alex in dialogue");
            DialogueManager.Instance.StartDialogue(Alex);
        }
    }
}
