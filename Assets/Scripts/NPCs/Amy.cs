using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Amy_Doctor : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Amy.
    /// </summary>
    public NPC Amy;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Amy script started");
        Amy = new NPC
        {
            Greeting = "You’re not sick, are you? Make sure you're getting enough rest!",
            inDialogue = false,
            ID = 5,
            Name = "Amy",
            Job = "Doctor",
            Description = "Amy is the town’s dedicated doctor, formerly a renowned physician in the city. She left her high-pressure hospital job " +
            "to open a small clinic in Babel, where she could focus on truly helping people. Highly skilled and serious, Amy sometimes struggles to " +
            "relax, often overworking herself. She loves her husband Mark despite his forgetful and carefree nature, and while she respects her daughter Jessica, " +
            "she wishes Jessica had a clearer plan for the future. Amy has a friendly but skeptical rivalry with Esmeralda over traditional medicine vs. natural remedies.",
            Personality = new List<string> { "Serious", "Compassionate", "Overworks herself", "Highly skilled" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(3, 5),
                    Location = "Doctor's Office"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(6, 7),
                    Location = "Park"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(4, 6),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Clinic",
            CurrentCoordinates = new Vector2(3, 5)
        };
        NPCManager.Instance.AddNPC(Amy);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Amy.inDialogue = true;
            Debug.Log("Amy in dialogue");
            DialogueManager.Instance.StartDialogue(Amy);
        }
    }
}

