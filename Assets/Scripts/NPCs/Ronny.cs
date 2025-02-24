using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Ronny_Chef : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Ronny.
    /// </summary>
    public NPC Ronny;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Ronny = new NPC
        {
            Greeting = "You hungry? If not, you should be.",
            inDialogue = false,
            ID = 3,
            Name = "Ronny",
            Job = "Chef",
            Description = "Ronny is the gruff but talented owner and head chef of Ronny’s Round-Up, the town’s go-to diner. " +
            "He learned to cook from his grandmother, valuing instinct over strict recipes. His no-nonsense attitude hides a deep passion for food " +
            "and a soft spot for helping others. He secretly admires Pattie but gets flustered around her. He often gives cryptic cooking advice " +
            "and refuses to share his exact recipes. Despite his gruff exterior, he cares deeply about his staff, including Jessica, and keeps an eye on Garbanzo and Teddy, " +
            "even if the dog annoys him by running into his kitchen.",
            Personality = new List<string> { "Gruff but kind", "Passionate cook", "Secretly shy", "Protective" },
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
            CurrentLocation = "Restaurant",
            CurrentCoordinates = new Vector2(3, 3)
        };
        NPCManager.Instance.AddNPC(Ronny);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Ronny.inDialogue = true;
            Debug.Log("Ronny in dialogue");
            DialogueManager.Instance.StartDialogue(Ronny);
        }
    }
}

