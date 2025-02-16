using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Pattie_Baker : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Pattie.
    /// </summary>
    public NPC Pattie;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Pattie_Baker script started");
        Pattie = new NPC
        {
            Greeting = "Hello, dear! Looking for something sweet? Just don�t mention pies.",
            inDialogue = false,
            ID = 1,
            Name = "Pattie NoPies",
            Job = "Baker",
            Description = "Pattie is a warm and friendly baker who is well-loved in Babel for her delicious treats�except pies, which she refuses to make. " +
            "Once a contestant on a reality cooking show, she suffered a humiliating critique from a famous British chef, leaving her with a deep aversion to pies. " +
            "She finds joy in mentoring young bakers like Alex, even though she pretends to see him as competition. She enjoys community gatherings, " +
            "offering free pastries to those in need, and keeping the town well-fed with her kindness and humor.",
            Personality = new List<string> { "Friendly", "Empathetic", "Loyal", "Avoids talking about pies" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(0, 0),
                    Location = "Bakery"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(8, 12),
                    Location = "Town Square"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(15, 5),
                    Location = "Ronny's Round-Up"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Bakery",
            CurrentCoordinates = new Vector2(0, 0)
        };
        NPCManager.Instance.AddNPC(Pattie);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Pattie.inDialogue = true;
            Debug.Log("Pattie in dialogue");
            DialogueManager.Instance.StartDialogue(Pattie);
        }
    }
}

