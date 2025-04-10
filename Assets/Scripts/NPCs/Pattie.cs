using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Pattie_Baker : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Pattie.
    /// </summary>
    public NPC Pattie;


    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Pattie = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Hello, sweetie! Can I get you something fresh from the oven?",
                "Oh, hi there! You hungry? I’ve got plenty of treats!",
                "Welcome in! What can I get you today? Something warm? Something sweet?",
                "Hey, nice to see you! I just pulled some cookies out of the oven, interested?",
                "Oh, if it isn’t my favorite customer! Okay, I say that to everyone, but I mean it!",
                "Come on in, sugar! I always have something good ready. Unless it’s pie. Then no.",
                "You look like you could use a snack!",
                "Ah, just in time! I was about to take a break, but I can always chat with a friendly face.",
            },
            inDialogue = false,
            ID = 1,
            Name = "Pattie",
            Job = "Baker",
            Description = "Pattie is a warm and friendly baker who is well-loved in Babel for her delicious treats—except pies, which she refuses to make. " +
            "Once a contestant on a reality cooking show, she suffered a humiliating critique from a famous British chef, leaving her with a deep aversion to pies. " +
            "She finds joy in mentoring young bakers like Alex, even though she pretends to see him as competition. She enjoys community gatherings, " +
            "offering free pastries to those in need, and keeping the town well-fed with her kindness and humor.",
            Personality = new List<string> { "Friendly", "Empathetic", "Loyal", "Avoids talking about pies" },
            Schedule = new List<ScheduleEntry>
            {
                new() { waypoint = "BakeryBack", time = 8, location = "Bakery" },
                new() { waypoint = "BakeryFry", time = 9, location = "Bakery" },
                new() { waypoint = "BakeryCounter", time = 10, location = "Bakery" },
                new() { waypoint = "BakeryClerk", time = 11, location = "Bakery" },
                new() { waypoint = "BakeryLine", time = 12, location = "Bakery" },
                new() { waypoint = "Bakery Table", time = 13, location = "Overworld" },
                new() { waypoint = "Cafe Entrance", time = 14, location = "Overworld" },
                new() { waypoint = "CafeCounter", time = 15, location = "Cafe" },
                new() { waypoint = "Park Bench", time = 16, location = "Overworld" },
                new() { waypoint = "Post Office Entrance", time = 17, location = "Overworld" },
                new() { waypoint = "Town Hall Entrance", time = 18, location = "Overworld" },
                new() { waypoint = "House 1", time = 19, location = "Overworld" },
                new() { waypoint = "House 1", time = 20, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Bakery",
            CurrentCoordinates = new Vector2(0, 0)
        };
        Pattie.agent.updateRotation = false;
        Pattie.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Pattie);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Pattie.inDialogue = true;
            Debug.Log("Pattie in dialogue");
            DialogueManager.Instance.StartDialogue(Pattie);
        }
    }
}

