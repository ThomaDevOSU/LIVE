using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Mabel_Retired : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Mabel.
    /// </summary>
    public NPC Mabel;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Mabel = new NPC
        {
            Greeting = "Oh, hello dear! Have I met you before? Oh well, it's always nice to chat!",
            inDialogue = false,
            ID = 10,
            Name = "Mabel",
            Job = "Retired",
            Description = "Mabel is a kind and wise elderly woman who once ran the general store in Babel. " +
            "She has always been the town’s warm-hearted matriarch, offering advice and sweet candies to anyone who visits her. " +
            "Though her memory is fading, she still loves reminiscing about the past and enjoys watching the town's young people grow." +
            "Adores her husband Will.",
            Personality = new List<string> { "Gentle", "Forgetful", "Nostalgic", "Caring" },
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "Bakery Counter",
                    time = 8,
                    location = "Bakery"
                },
                new ()
                {
                    waypoint = "Park Bench",
                    time = 10,
                    location = "Park"
                },
                new ()
                {
                    waypoint = "Pattie's Home",
                    time = 14,
                    location = "Pattie's Home"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(6, 6)
        };
        NPCManager.Instance.AddNPC(Mabel);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Mabel.inDialogue = true;
            Debug.Log("Mabel in dialogue");
            DialogueManager.Instance.StartDialogue(Mabel);
        }
    }
}

