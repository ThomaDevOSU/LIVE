using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Ace_Soccer : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Ace.
    /// </summary>
    public NPC Ace;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Ace = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Hey! Wanna play some soccer? Or at least pass the ball back if I kick it to you?",
                "Yo, you ever try scoring a goal against Garbanzo? He’s got unreal defense.",
                "What’s up? I’m warming up—wanna join? Or just stand there and admire my footwork?",
                "I swear, I was THIS close to breaking my juggling record. You believe me, right?",
                "Oh, it’s you! Thought you were gonna be Isabella for a second—dodged a lecture on jaywalking!",
                "You know, I bet you’d be pretty decent at soccer if you practiced with me more. Just sayin’.",
                "Gotta keep moving! Standing still is for people who don’t have dreams. Or at least, for people who don’t have a soccer ball.",
                "Hey! Ever seen Garbanzo dribble? It’s weirdly impressive. We gotta get him on a team."
            },
            inDialogue = false,
            ID = 13,
            Name = "Ace",
            Job = "None",
            Description = "Ace is an energetic young man who spends most of his time playing soccer in the park. " +
            "He was sent to Babel by his parents, who saw him as a burden, though he tries to act like it doesn’t bother him. " +
            "He finds comfort in soccer, playing with Garbanzo, and talking to Mabel. Sheriff Isabella constantly scolds him for jaywalking, which he finds ridiculous." +
            "His Aunt Esmeralda takes care of him but they rarely spend time together.",
            Personality = new List<string> { "Energetic", "Optimistic", "Restless", "Playful" },
            Schedule = new List<ScheduleEntry>
            {
                new() { waypoint = "Park Slide", time = 8, location = "Overworld" },
                new() { waypoint = "Park Swings", time = 9, location = "Overworld" },
                new() { waypoint = "Tire Swing", time = 10, location = "Overworld" },
                new() { waypoint = "Park Top Right", time = 11, location = "Overworld" },
                new() { waypoint = "Bakery Entrance", time = 12, location = "Overworld" },
                new() { waypoint = "BakeryCounter", time = 13, location = "Bakery" },
                new() { waypoint = "Cafe Entrance", time = 14, location = "Overworld" },
                new() { waypoint = "CafeTable", time = 15, location = "Overworld" },
                new() { waypoint = "Park Bench", time = 16, location = "Overworld" },
                new() { waypoint = "Overworld", time = 17, location = "Overworld" },
                new() { waypoint = "House 2", time = 18, location = "Overworld" },
                new() { waypoint = "House 2", time = 19, location = "Overworld" },
                new() { waypoint = "House 2", time = 20, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(5, 5)
        };

        // Change these to true (?) after testing
        Ace.agent.updateRotation = false;
        Ace.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Ace);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Ace.inDialogue = true;
            Debug.Log("Ace in dialogue");
            DialogueManager.Instance.StartDialogue(Ace);
        }
    }
}

