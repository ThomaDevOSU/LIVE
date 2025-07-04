using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Esmeralda_Pharmacist : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Esmeralda.
    /// </summary>
    public NPC Esmeralda;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Esmeralda = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Esmeralda_Greeting_7", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 11,
            Name = "Esmeralda",
            Job = "Pharmacist",
            Description = "Esmeralda is the town�s enigmatic pharmacist, known for her deep knowledge of both modern medicine and mysterious herbal remedies. " +
            "Rumors persist that she might be a witch, but she neither confirms nor denies them. " +
            "She speaks in cryptic riddles and often knows things she was never told, adding to her air of mystery." +
            "She takes care of her nephew Ace who was sent to Babel to overcome behavioral issues",
            Personality = new List<string> { "Mysterious", "Aloof", "Intelligent", "Cryptic" },

            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "HospitalPharma", time = 8, location = "Hospital" },
               new() { waypoint = "HospitalPharma", time = 9, location = "Hospital" },
               new() { waypoint = "Cafe Entrance", time = 10, location = "Overworld" },
               new() { waypoint = "CafeLine", time = 11, location = "Cafe" },
               new() { waypoint = "CafeCounter", time = 12, location = "Cafe" },
               new() { waypoint = "HospitalPharma", time = 13, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 14, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 15, location = "Hospital" },
               new() { waypoint = "HospitalPharma", time = 16, location = "Hospital" },
               new() { waypoint = "HospitalPharma", time = 17, location = "Hospital" },
               new() { waypoint = "HospitalPharma", time = 18, location = "Hospital" },
               new() { waypoint = "GroceryAisle3", time = 19, location = "GroceryStore" },
               new() { waypoint = "GroceryLine", time = 20, location = "GroceryStore" },
               new() { waypoint = "GroceryCounter", time = 21, location = "GroceryStore" },
               new() { waypoint = "House 6-1", time = 22, location = "Overworld" },
               new() { waypoint = "House 6-1", time = 23, location = "Overworld" },
               new() { waypoint = "House 6-1", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Pharmacy",
            CurrentCoordinates = new Vector2(2, 2)
        };
        Esmeralda.agent.updateRotation = false;
        Esmeralda.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Esmeralda);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Esmeralda.inDialogue = true;
            Debug.Log("Esmeralda in dialogue");
            DialogueManager.Instance.StartDialogue(Esmeralda);
        }
    }
}

