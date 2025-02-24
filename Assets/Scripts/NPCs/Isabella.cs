using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Isabella_Police : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Isabella.
    /// </summary>
    public NPC Isabella;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Isabella = new NPC
        {
            Greeting = "Halt! Wait… never mind, I thought you were someone else. Or maybe you are? Hmm…",
            inDialogue = false,
            ID = 12,
            Name = "Isabella",
            Job = "Sheriff",
            Description = "Isabella is the town’s overly enthusiastic Sheriff, constantly searching for conspiracies where none exist. " +
            "She takes her job very seriously—even if the town doesn’t really need her to. " +
            "She is convinced that Teddy the cat is a trained spy and that Esmeralda is running an underground potion lab. " +
            "Despite her eccentric theories, she genuinely wants to protect the town.",
            Personality = new List<string> { "Energetic", "Suspicious", "Dramatic", "Dedicated" },
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
            CurrentLocation = "Police Station",
            CurrentCoordinates = new Vector2(3, 3)
        };
        NPCManager.Instance.AddNPC(Isabella);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Isabella.inDialogue = true;
            Debug.Log("Isabella in dialogue");
            DialogueManager.Instance.StartDialogue(Isabella);
        }
    }
}

