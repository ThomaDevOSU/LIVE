using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Mabel_Retired : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Mabel.
    /// </summary>
    public NPC Mabel;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Mabel = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Mabel_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 10,
            Name = "Mabel",
            Job = "Retired",
            Description = "Mabel is a kind and wise elderly woman who once ran the general store in Babel. " +
            "She has always been the town’s warm-hearted matriarch, offering advice and sweet candies to anyone who visits her. " +
            "Though her memory is fading, she still loves reminiscing about the past and enjoys watching the town's young people grow." +
            "Adores her husband Will.",
            Personality = new List<string> { "Gentle", "Forgetful", "Nostalgic", "Caring" },
            Schedule = new List<ScheduleEntry>
            {
              new() { waypoint = "Park Bench 1", time = 8, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 9, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 10, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 11, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 12, location = "Overworld" },
              new() { waypoint = "HospitalCounter2", time = 13, location = "Hospital" },
              new() { waypoint = "HospitalCounter2", time = 14, location = "Hospital" },
              new() { waypoint = "HospitalBed", time = 15, location = "Hospital" },
              new() { waypoint = "HospitalBed", time = 16, location = "Hospital" },
              new() { waypoint = "HospitalOffice1", time = 17, location = "Hospital" },
              new() { waypoint = "Park Bench 1", time = 18, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 19, location = "Overworld" },
              new() { waypoint = "Park Bench 1", time = 20, location = "Overworld" },
              new() { waypoint = "House 5-1", time = 21, location = "Overworld" },
              new() { waypoint = "House 5-1", time = 22, location = "Overworld" },
              new() { waypoint = "House 5-1", time = 23, location = "Overworld" },
              new() { waypoint = "House 5-1", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(6, 6)
        };
        Mabel.agent.updateRotation = false;
        Mabel.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Mabel);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Mabel.inDialogue = true;
            Debug.Log("Mabel in dialogue");
            DialogueManager.Instance.StartDialogue(Mabel);
        }
    }
}

