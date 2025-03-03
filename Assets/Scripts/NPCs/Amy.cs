using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

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
        Amy = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            Greeting = new string[]
            {
                "Oh, hey! Are you staying hydrated? You should be.",
                "If you’re here because something hurts, let’s get it checked out.",
                "Please tell me you’re here for a check-up and not because of another ‘accident.’",
                "Feeling alright? You look fine, but I ask everyone just in case.",
                "If you want medical advice, I’m here. If you want life advice… maybe ask someone else.",
                "Try not to get sick, okay? This town has enough reckless people as it is.",
                "If you’re here because you saw Esmeralda first… please, just don’t tell me what she gave you.",
            },
            inDialogue = false,
            ID = 5,
            Name = "Amy",
            Job = "Doctor",
            Description = "Amy is the town’s dedicated doctor, formerly a renowned physician in the city. She left her high-pressure hospital job " +
            "to open a small clinic in Babel, where she could focus on truly helping people. Highly skilled and serious, Amy sometimes struggles to " +
            "relax, often overworking herself. She loves her husband Mark despite his forgetful and carefree nature, and while she respects her daughter Jessica, " +
            "she wishes Jessica had a clearer plan for the future. Amy has a friendly but skeptical rivalry with Esmeralda over traditional medicine vs. natural remedies.",
            Personality = new List<string> { "Serious", "Compassionate", "Overworks herself", "Highly skilled" },
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "House 2",
                    time = 8,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Hospital Entrance",
                    time = 10,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "House 2",
                    time = 14,
                    location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Clinic",
            CurrentCoordinates = new Vector2(3, 5)
        };
        Amy.agent.updateRotation = false;
        Amy.agent.updateUpAxis = false;
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

