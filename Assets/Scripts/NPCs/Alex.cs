using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Alex_Barista : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Alex.
    /// </summary>
    public NPC Alex;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Alex = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Alex_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 2,
            Name = "Alex",
            Job = "Barista",
            Description = "Alex is a passionate barista and the owner of the Babbling Bean café. He left behind the big city to create a cozy space where people " +
            "can connect over great coffee. Enthusiastic about brewing techniques, he’s always experimenting with new flavors and pastries—though baking is still a work in progress. " +
            "He looks up to Pattie for baking guidance but teases her about her refusal to make pies, unaware of her past. " +
            "Friendly and energetic, Alex enjoys chatting with customers and making sure everyone gets just the right drink for their mood.",
            Personality = new List<string> { "Friendly", "Enthusiastic", "Health-conscious", "Passionate about coffee" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "CafeBack", time = 8, location = "Cafe" },
               new() { waypoint = "CafeFridge", time = 9, location = "Cafe" },
               new() { waypoint = "CafeClerk", time = 10, location = "Cafe" },
               new() { waypoint = "CafeClerk", time = 11, location = "Cafe" },
               new() { waypoint = "CafeClerk", time = 12, location = "Cafe" },
               new() { waypoint = "CafeBack", time = 13, location = "Cafe" },
               new() { waypoint = "CafeClerk", time = 14, location = "Cafe" },
               new() { waypoint = "CafeFridge", time = 15, location = "Cafe" },
               new() { waypoint = "BakeryLine", time = 16, location = "Bakery" },
               new() { waypoint = "BakeryCounter", time = 17, location = "Bakery" },
               new() { waypoint = "CafeClerk", time = 18, location = "Cafe" },
               new() { waypoint = "CafeFridge", time = 19, location = "Cafe" },
               new() { waypoint = "CafeBack", time = 20, location = "Cafe" },
               new() { waypoint = "CafeBack", time = 21, location = "Cafe" },
               new() { waypoint = "House 3", time = 22, location = "Overworld" },
               new() { waypoint = "House 3", time = 23, location = "Overworld" },
               new() { waypoint = "House 3", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Babbling Bean Café",
            CurrentCoordinates = new Vector2(5, 5)
        };
        Alex.agent.updateRotation = false;
        Alex.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Alex);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Alex.inDialogue = true;
            Debug.Log("Alex in dialogue");
            DialogueManager.Instance.StartDialogue(Alex);
        }
    }
}
