using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Amy_Doctor : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Amy.
    /// </summary>
    public NPC Amy;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Amy = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Amy_Greeting_7", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 5,
            Name = "Amy",
            Job = "Doctor",
            Description = "Amy is the town�s dedicated doctor, formerly a renowned physician in the city. She left her high-pressure hospital job " +
            "to open a small clinic in Babel, where she could focus on truly helping people. Highly skilled and serious, Amy sometimes struggles to " +
            "relax, often overworking herself. She loves her husband Mark despite his forgetful and carefree nature, and while she respects her daughter Jessica, " +
            "she wishes Jessica had a clearer plan for the future. Amy has a friendly but skeptical rivalry with Esmeralda over traditional medicine vs. natural remedies.",
            Personality = new List<string> { "Serious", "Compassionate", "Overworks herself", "Highly skilled" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "HospitalEntrance", time = 8, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 9, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 10, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 11, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 12, location = "Hospital" },
               new() { waypoint = "HospitalReception", time = 13, location = "Hospital" },
               new() { waypoint = "HospitalReception", time = 14, location = "Hospital" },
               new() { waypoint = "HospitalDoctor", time = 15, location = "Hospital" },
               new() { waypoint = "HospitalDoctor", time = 16, location = "Hospital" },
               new() { waypoint = "HospitalDoctor", time = 17, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 18, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 19, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 20, location = "Hospital" },
               new() { waypoint = "HospitalEntrance", time = 21, location = "Hospital" },
               new() { waypoint = "House 4-2", time = 22, location = "Overworld" },
               new() { waypoint = "House 4-2", time = 23, location = "Overworld" },
               new() { waypoint = "House 4-2", time = 24, location = "Overworld" }
            },
            CurrentLocation = "Hospital",
            CurrentCoordinates = new Vector2(3, 5)
        };
        Amy.agent.updateRotation = false;
        Amy.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Amy);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Amy.inDialogue = true;
            Debug.Log("Amy in dialogue");
            DialogueManager.Instance.StartDialogue(Amy);
        }
    }
}

