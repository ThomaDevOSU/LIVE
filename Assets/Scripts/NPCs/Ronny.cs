using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Ronny_Chef : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Ronny.
    /// </summary>
    public NPC Ronny;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Ronny = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ronny_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 3,
            Name = "Ronny",
            Job = "Chef",
            Description = "Ronny is the gruff but talented owner and head chef of Ronny’s Round-Up, the town’s go-to diner. " +
            "He learned to cook from his grandmother, valuing instinct over strict recipes. His no-nonsense attitude hides a deep passion for food " +
            "and a soft spot for helping others. He secretly admires Pattie but gets flustered around her. He often gives cryptic cooking advice " +
            "and refuses to share his exact recipes. Despite his gruff exterior, he cares deeply about his staff, including Jessica, and keeps an eye on Garbanzo and Teddy, " +
            "even if the dog annoys him by running into his kitchen.",
            Personality = new List<string> { "Gruff but kind", "Passionate cook", "Secretly shy", "Protective" },
            Schedule = new List<ScheduleEntry>
                       {
                          new() { waypoint = "House 2-1", time = 8, location = "Overworld" },
                          new() { waypoint = "Cafe Table 1", time = 9, location = "Overworld" },
                          new() { waypoint = "Cafe Table 1", time = 10, location = "Overworld" },
                          new() { waypoint = "Cafe Table 1", time = 11, location = "Overworld" },
                          new() { waypoint = "Bakery Entrance", time = 12, location = "Overworld" },
                          new() { waypoint = "Bakery Entrance", time = 13, location = "Overworld" },
                          new() { waypoint = "BakeryLine", time = 14, location = "Bakery" },
                          new() { waypoint = "BakeryCounter", time = 15, location = "Bakery" },
                          new() { waypoint = "GroceryAisle2", time = 16, location = "GroceryStore" },
                          new() { waypoint = "GroceryAisle2", time = 17, location = "GroceryStore" },
                          new() { waypoint = "GroceryAisle1", time = 18, location = "GroceryStore" },
                          new() { waypoint = "GroceryAisle2", time = 19, location = "GroceryStore" },
                          new() { waypoint = "GroceryAisle2", time = 20, location = "GroceryStore" },
                          new() { waypoint = "House 2-1", time = 21, location = "Overworld" },
                          new() { waypoint = "House 2-1", time = 22, location = "Overworld" },
                          new() { waypoint = "House 2-1", time = 23, location = "Overworld" },
                          new() { waypoint = "House 2-1", time = 24, location = "Overworld" }
                       },
            messages = new List<Message>(),
            CurrentLocation = "Restaurant",
            CurrentCoordinates = new Vector2(3, 3)
        };
        Ronny.agent.updateRotation = false;
        Ronny.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Ronny);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Ronny.inDialogue = true;
            Debug.Log("Ronny in dialogue");
            DialogueManager.Instance.StartDialogue(Ronny);
        }
    }
}

