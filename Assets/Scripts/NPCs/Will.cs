using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Will_Mayor : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Will.
    /// </summary>
    public NPC Will;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Will script started");
        Will = new NPC
        {
            Greeting = "Ah, another fine day in Babel! Have I ever told you about the time we built this town from the ground up?",
            inDialogue = false,
            ID = 9,
            Name = "Will",
            Job = "Mayor",
            Description = "Will is the well-meaning and deeply caring mayor of Babel. He has lived in the town his entire life " +
            "and enjoys reminiscing about its history, sometimes in long-winded tangents. Though a bit socially awkward, " +
            "he is beloved by the community and works hard to support it. He dotes on his wife, Mabel, and is protective " +
            "of her as her memory fades.",
            Personality = new List<string> { "Kind", "Talkative", "Sentimental", "Dedicated" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(4, 4),
                    Location = "Town Hall"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(8, 8),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Town Hall",
            CurrentCoordinates = new Vector2(4, 4)
        };
        NPCManager.Instance.AddNPC(Will);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Will.inDialogue = true;
            Debug.Log("Will in dialogue");
            DialogueManager.Instance.StartDialogue(Will);
        }
    }
}

