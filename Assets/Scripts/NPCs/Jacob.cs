using UnityEngine;
using System.Collections.Generic;

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
        Debug.Log("Jacob script started");
        Jacob = new NPC
        {
            Greeting = "Stay strong, stay fit! Have you been lifting today?",
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
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(12, 12),
                    Location = "Fire Station"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(15, 15),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Fire Station",
            CurrentCoordinates = new Vector2(12, 12)
        };
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

