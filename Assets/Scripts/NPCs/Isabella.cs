using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Isabella_Police : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Isabella.
    /// </summary>
    public NPC Isabella;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Isabella = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Isabella_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 12,
            Name = "Isabella",
            Job = "Sheriff",
            Description = "Isabella is the town’s overly enthusiastic Sheriff, constantly searching for conspiracies where none exist. " +
            "She takes her job very seriously—even if the town doesn’t really need her to. " +
            "She is convinced that Teddy the cat is a trained spy and that Esmeralda is running an underground potion lab. " +
            "Despite her eccentric theories, she genuinely wants to protect the town.",
            Personality = new List<string> { "Energetic", "Suspicious", "Dramatic", "Dedicated" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "PoliceStationOffice", time = 8, location = "PoliceStation" },
               new() { waypoint = "PoliceStationOffice", time = 9, location = "PoliceStation" },
               new() { waypoint = "Police Patrol 1", time = 10, location = "Overworld" },
               new() { waypoint = "Police Patrol 2", time = 11, location = "Overworld" },
               new() { waypoint = "Police Patrol 3", time = 12, location = "Overworld" },
               new() { waypoint = "Police Patrol 4", time = 13, location = "Overworld" },
               new() { waypoint = "Police Patrol 3", time = 14, location = "Overworld" },
               new() { waypoint = "Police Patrol 2", time = 15, location = "Overworld" },
               new() { waypoint = "Police Patrol 1", time = 16, location = "Overworld" },
               new() { waypoint = "Police Station Entrance", time = 17, location = "Overworld" },
               new() { waypoint = "PoliceStationGarage", time = 18, location = "PoliceStation" },
               new() { waypoint = "TownHallEntrance", time = 19, location = "TownHall" },
               new() { waypoint = "TownHallOffice2", time = 20, location = "TownHall" },
               new() { waypoint = "TownHallOffice2", time = 21, location = "TownHall" },
               new() { waypoint = "House 7", time = 22, location = "Overworld" },
               new() { waypoint = "Greenhouse Entrance", time = 23, location = "Overworld" },
               new() { waypoint = "Greenhouse Entrance", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Police Station",
            CurrentCoordinates = new Vector2(3, 3)
        };
        Isabella.agent.updateRotation = false;
        Isabella.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Isabella);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Isabella.inDialogue = true;
            Debug.Log("Isabella in dialogue");
            DialogueManager.Instance.StartDialogue(Isabella);
        }
    }
}

