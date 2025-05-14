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
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jacob_Greeting_8", isUI.NOTUI)
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
              new() { waypoint = "FireBarracks", time = 8, location = "FireStation" },
              new() { waypoint = "FireBarracks", time = 9, location = "FireStation" },
              new() { waypoint = "FireEntrance", time = 10, location = "FireStation" },
              new() { waypoint = "FireTruck", time = 11, location = "FireStation" },
              new() { waypoint = "FireTruck", time = 12, location = "FireStation" },
              new() { waypoint = "Cafe Entrance", time = 13, location = "Overworld" },
              new() { waypoint = "CafeLine", time = 14, location = "Cafe" },
              new() { waypoint = "CafeCounter", time = 15, location = "Cafe" },
              new() { waypoint = "Fire Station Entrance", time = 16, location = "Overworld" },
              new() { waypoint = "Fire Station Entrance", time = 17, location = "Overworld" },
              new() { waypoint = "FireTruck", time = 18, location = "FireStation" },
              new() { waypoint = "FireTruck", time = 19, location = "FireStation" },
              new() { waypoint = "FireTruck", time = 20, location = "FireStation" },
              new() { waypoint = "FireTruck", time = 21, location = "FireStation" },
              new() { waypoint = "FireEntrance", time = 22, location = "FireStation" },
              new() { waypoint = "FireBarracks", time = 23, location = "FireStation" },
              new() { waypoint = "FireBarracks", time = 24, location = "FireStation" }
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

