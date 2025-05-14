using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Jessica_Waitress : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Jessica.
    /// </summary>
    public NPC Jessica;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Jessica = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Jessica_Greeting_7", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 6,
            Name = "Jessica",
            Job = "Waitress",
            Description = "Jessica is a free-spirited young woman with a love for nature and a deep curiosity about the ocean. " +
            "She works at Ronny’s Round-Up, though her true dream is to become a marine biologist. She spends her free time in the park, " +
            "sketching birds and thinking about what lies beyond Babel. While she shares her father Mark’s laid-back attitude, she often clashes " +
            "with her mother Amy, who wants her to be more structured. She has a friendly and relaxed presence but sometimes feels stuck in her routine.",
            Personality = new List<string> { "Easygoing", "Dreamer", "Curious", "Independent" },
            Schedule = new List<ScheduleEntry>
{
          new() { waypoint = "Park Swings", time = 8, location = "Overworld" },
          new() { waypoint = "Park Swings", time = 9, location = "Overworld" },
          new() { waypoint = "Park Bench 2", time = 10, location = "Overworld" },
          new() { waypoint = "GroceryAisle1", time = 11, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 12, location = "GroceryStore" },
          new() { waypoint = "GroceryAisle1", time = 13, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 14, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 15, location = "GroceryStore" },
          new() { waypoint = "GroceryAisle1", time = 16, location = "GroceryStore" },
          new() { waypoint = "GroceryAisle1", time = 17, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 18, location = "GroceryStore" },
          new() { waypoint = "GroceryAisle1", time = 19, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 20, location = "GroceryStore" },
          new() { waypoint = "GroceryClerk", time = 21, location = "GroceryStore" },
          new() { waypoint = "House 4-3", time = 22, location = "Overworld" },
          new() { waypoint = "House 4-3", time = 23, location = "Overworld" },
          new() { waypoint = "House 4-3", time = 24, location = "Overworld" }
    },
            messages = new List<Message>(),
            CurrentLocation = "Restaurant",
            CurrentCoordinates = new Vector2(8, 3)
        };
        Jessica.agent.updateRotation = false;
        Jessica.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Jessica);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Jessica.inDialogue = true;
            Debug.Log("Jessica in dialogue");
            DialogueManager.Instance.StartDialogue(Jessica);
        }
    }
}

