using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Jacob_Firefighter : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Jacob.
    /// </summary>
    public NPC Jacob;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Jacob = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                "Hey! Have you been hitting the gym? No? Well, there's no time like the present!",
                "Fire safety and gains—two things I take very seriously!",
                "Need a spot?",
                "You ever think about how life is like a bench press? You just gotta push through!",
                "No fires today, which means more time for deadlifts!",
                "Hydration and protein—two keys to success. You keeping up with both?",
                "I don’t just fight fires, I fight weakness! You in?",
                "Strength isn’t just physical—it’s about discipline!",
            },
            inDialogue = false,
            ID = 16,
            Name = "Jacob",
            Job = "Firefighter",
            Description = "Jacob is the town’s Fire Chief, but since Babel has never had a major fire, he spends most of his time lifting weights. " +
            "He sees life through the lens of strength training, making almost every conversation about fitness. " +
            "Determined to get Ronny to lift, he constantly challenges him, though Ronny refuses. " +
            "He respects Mark’s relaxed lifestyle but doesn’t understand how someone can be so chill. " +
            "He tries to convince Amy that lifting is the ultimate form of stress relief. " +
            "Jacob finds Elijah’s philosophical outlook confusing, often countering his deep thoughts with flexing. " +
            "He loves picking up Garbanzo and spinning him, calling him 'Lightweight Champ'. " +
            "He tries to get Ace to lift, but Ace insists soccer is enough exercise.",
            Personality = new List<string> { "Energetic", "Dedicated", "Fitness-Obsessed" },
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "Fire Station Entrance",
                    time = 8,
                    location = "Overworld"
                },
                new ()
                {
                    waypoint = "Fire Station Entrance",
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
            CurrentLocation = "Fire Station",
            CurrentCoordinates = new Vector2(12, 12)
        };
        Jacob.agent.updateRotation = false;
        Jacob.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Jacob);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Jacob.inDialogue = true;
            Debug.Log("Jacob in dialogue");
            DialogueManager.Instance.StartDialogue(Jacob);
        }
    }
}

