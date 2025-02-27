using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

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
            agent = GetComponent<NavMeshAgent>(),

            Greeting = new string[]
            {
                "Oh, hello dear! Have you eaten today? I have some candies if you’d like one.",
                "You remind me of someone… oh, never mind, it’ll come to me later.",
                "What a lovely day, isn’t it? I love watching the town go by.",
                "Do I know you? I feel like I should… Oh, well! It’s nice to see you anyway!",
                "Have you seen Will? He was just here a moment ago…",
                "Come sit with me, dear. I have plenty of stories to share.",
                "You look like you have a good head on your shoulders. Have I told you about the old general store?",
                "Oh, I remember you! Or… I think I do. Either way, it’s nice to see a friendly face.",
            },
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
                    waypoint = "Park Bench",
                    time = 8,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Park Bench",
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
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(6, 6)
        };
        Mabel.agent.updateRotation = false;
        Mabel.agent.updateUpAxis = false;
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

