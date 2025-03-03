using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Teddy_Cat : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Teddy.
    /// </summary>
    public NPC Teddy;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Teddy = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),

            Greeting = new string[]
            {
                "Oh, it’s you. Again.",
                "Unless you brought treats, I fail to see the point of this interaction.",
                "Speak quickly. My time is valuable.",
                "What do you want, human?",
                "You may admire me, but please do so from a respectable distance.",
                "I was in the middle of something… important. Probably.",
                "You are, at best, a mild distraction. Proceed.",
            },
            inDialogue = false,
            ID = 8,
            Name = "Teddy",
            Job = "Cat",
            Description = "Teddy, who insists on being called Theodore, is an intelligent but aloof cat " +
            "who roams Babel on his own terms. He has no official owner but tolerates Ronny, who feeds him scraps. " +
            "Though he claims to despise Garbanzo, he secretly watches out for him, making sure the dog doesn’t " +
            "get into trouble. Teddy enjoys observing the town, judging humans from a safe distance, and engaging " +
            "in silent intellectual battles with Esmeralda.",
            Personality = new List<string> { "Aloof", "Intelligent", "Secretly Caring", "Judgmental" },
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "Park Swings",
                    time = 8,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Post Office Entrance",
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
            CurrentLocation = "Restaurant",
            CurrentCoordinates = new Vector2(2, 2)
        };
        Teddy.agent.updateRotation = false;
        Teddy.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Teddy);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Teddy.inDialogue = true;
            Debug.Log("Teddy in dialogue");
            DialogueManager.Instance.StartDialogue(Teddy);
        }
    }
}

