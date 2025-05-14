using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Will_Mayor : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Will.
    /// </summary>
    public NPC Will;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Will = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Will_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 9,
            Name = "Will",
            Job = "Mayor",
            Description = "Will is the well-meaning and deeply caring mayor of Babel. He has lived in the town his entire life " +
            "and enjoys reminiscing about its history, sometimes in long-winded tangents. Though a bit socially awkward, " +
            "he is beloved by the community and works hard to support it. He dotes on his wife, Mabel, and is protective " +
            "of her as her memory fades.",
            Personality = new List<string> { "Kind", "Talkative", "Sentimental", "Dedicated" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "Cafe Entrance", time = 8, location = "Overworld" },
               new() { waypoint = "CafeLine", time = 9, location = "Cafe" },
               new() { waypoint = "CafeCounter", time = 10, location = "Cafe" },
               new() { waypoint = "TownHallEntrance", time = 11, location = "TownHall" },
               new() { waypoint = "TownHallOffice1", time = 12, location = "TownHall" },
               new() { waypoint = "HospitalCounter1", time = 13, location = "Hospital" },
               new() { waypoint = "HospitalCounter1", time = 14, location = "Hospital" },
               new() { waypoint = "HospitalOffice2", time = 15, location = "Hospital" },
               new() { waypoint = "HospitalOffice2", time = 16, location = "Hospital" },
               new() { waypoint = "HospitalOffice2", time = 17, location = "Hospital" },
               new() { waypoint = "TownHallOffice1", time = 18, location = "TownHall" },
               new() { waypoint = "TownHallOffice1", time = 19, location = "TownHall" },
               new() { waypoint = "TownHallOffice1", time = 20, location = "TownHall" },
               new() { waypoint = "TownHallOffice1", time = 21, location = "TownHall" },
               new() { waypoint = "TownHallEntrance", time = 22, location = "TownHall" },
               new() { waypoint = "House 5-2", time = 23, location = "Overworld" },
               new() { waypoint = "House 5-2", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Town Hall",
            CurrentCoordinates = new Vector2(4, 4)
        };
        Will.agent.updateRotation = false;
        Will.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Will);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Will.inDialogue = true;
            Debug.Log("Will in dialogue");
            DialogueManager.Instance.StartDialogue(Will);
        }
    }
}

