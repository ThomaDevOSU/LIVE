using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

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
        Will = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Ah, hello there! Always good to see a friendly face.",
                "Oh! You know, this reminds me of a story… Oh, but where are my manners? How are you?",
                "Welcome, welcome! How’s your day treating you?",
                "Ah, another fine day in Babel, wouldn’t you say?",
                "Good to see you! If you ever need anything, don’t hesitate to ask.",
                "Oh, you just missed Mabel—she was telling the most wonderful story. But tell me, how are you?",
                "You know, this town wouldn’t be the same without folks like you.",
                "Ah, reminds me of the good old days! But enough about me, what’s on your mind?",
            },
            inDialogue = false,
            ID = 9,
            Name = "Will",
            Job = "Mayor",
            Description = "Will is the well-meaning and deeply caring mayor of Babel. He has lived in the town his entire life " +
            "and enjoys reminiscing about its history, sometimes in long-winded tangents. Though a bit socially awkward, " +
            "he is beloved by the community and works hard to support it. He dotes on his wife, Mabel, and is protective " +
            "of her as her memory fades.",
            Personality = new List<string> { "Kind", "Talkative", "Sentimental", "Dedicated" },
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
                    waypoint = "Town Hall Entrance",
                    time = 10,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "House 4",
                    time = 14,
                    location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Town Hall",
            CurrentCoordinates = new Vector2(4, 4)
        };
        Will.agent.updateRotation = false;
        Will.agent.updateUpAxis = false;
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

