using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

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
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Halt! State your business� just kidding. Or am I?",
                "You�re not up to anything suspicious, are you?",
                "If you see anything unusual, report it to me immediately!",
                "I've got my eye on you� and everyone else.",
                "Do you believe in coincidences? Because I don�t.",
                "The town may seem quiet� too quiet.",
                "Any leads on the case? No? That�s exactly what they want us to think.",
                "Stay alert. You never know who�or what�is watching.",
            },
            inDialogue = false,
            ID = 12,
            Name = "Isabella",
            Job = "Sheriff",
            Description = "Isabella is the town�s overly enthusiastic Sheriff, constantly searching for conspiracies where none exist. " +
            "She takes her job very seriously�even if the town doesn�t really need her to. " +
            "She is convinced that Teddy the cat is a trained spy and that Esmeralda is running an underground potion lab. " +
            "Despite her eccentric theories, she genuinely wants to protect the town.",
            Personality = new List<string> { "Energetic", "Suspicious", "Dramatic", "Dedicated" },
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "Park Slide",
                    time = 8,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Overworld",
                    time = 10,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Overworld",
                    time = 14,
                    location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Police Station",
            CurrentCoordinates = new Vector2(3, 3)
        };
        Isabella.agent.updateRotation = false;
        Isabella.agent.updateUpAxis = false;
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

