using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Elijah_Postmaster : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Elijah.
    /// </summary>
    public NPC Elijah;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Elijah = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Ah, another fleeting moment in the grand passage of time.",
                "Mmm. Have you ever considered the impermanence of all things?",
                "Oh, it�s you. What brings you to this fleeting intersection of existence?",
                "Words are but ripples in the vast ocean of thought. What is it you seek?",
                "Hmph. A visitor. I suppose conversation is inevitable.",
                "The weight of the world is light when one learns to let go. But I digress�",
                "Silence is a virtue, but I suppose I can make an exception.",
                "If you�re here for small talk, I regret to inform you that I am ill-equipped.",
            },
            inDialogue = false,
            ID = 14,
            Name = "Elijah",
            Job = "Postmaster",
            Description = "Elijah is a quiet, deeply philosophical man who spends most of his time reflecting on life. " +
            "Transferred to Babel as postmaster, he finds solace in solitude, reading, and contemplating existence. " +
            "He rarely speaks, and when he does, it�s often with a profound or cryptic statement. " +
            "He has an unexpected bond with Mabel, admiring her ability to live in the moment, and often engages in silent intellectual battles with Teddy the cat. " +
            "Esmeralda fascinates him, though he suspects she understands more about the universe than she lets on. " +
            "He is frequently interrogated by Isabella, who is convinced he is hiding something, though he answers only in riddles.",
            Personality = new List<string> { "Philosophical", "Stoic", "Observant", "Minimalist" },
            Schedule = new List<ScheduleEntry>
            {
                new ScheduleEntry
                {
                    waypoint = "Post Office Entrance",
                    time = 8,
                    location = "Overworld"
                },
                new ScheduleEntry
                {
                    waypoint = "Cafe Table",
                    time = 10,
                    location = "Overworld"
                },
                new ScheduleEntry
                {
                    waypoint = "Post Office Entrance",
                    time = 14,
                    location = "Overworld"
                }
            }
        };
        Elijah.agent.updateRotation = false;
        Elijah.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Elijah);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Elijah.inDialogue = true;
            Debug.Log("Elijah in dialogue");
            DialogueManager.Instance.StartDialogue(Elijah);
        }
    }
}

