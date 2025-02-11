using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Ace_Soccer : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Ace.
    /// </summary>
    public NPC Ace;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Ace script started");
        Ace = new NPC
        {
            Greeting = "Hey! Wanna play some soccer? Or at least pass the ball back if I kick it to you?",
            inDialogue = false,
            ID = 13,
            Name = "Ace",
            Job = "None",
            Description = "Ace is an energetic young man who spends most of his time playing soccer in the park. " +
            "He was sent to Babel by his parents, who saw him as a burden, though he tries to act like it doesn’t bother him. " +
            "He finds comfort in soccer, playing with Garbanzo, and talking to Mabel. Sheriff Isabella constantly scolds him for jaywalking, which he finds ridiculous." +
            "His Aunt Esmeralda takes care of him but they rarely spend time together.",
            Personality = new List<string> { "Energetic", "Optimistic", "Restless", "Playful" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 5),
                    Location = "Park"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(10, 10),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(5, 5)
        };
        NPCManager.Instance.AddNPC(Ace);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Ace.inDialogue = true;
            Debug.Log("Ace in dialogue");
            DialogueManager.Instance.StartDialogue(Ace);
        }
    }
}

