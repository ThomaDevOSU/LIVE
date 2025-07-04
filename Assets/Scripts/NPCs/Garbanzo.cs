using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Garbanzo_Dog : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Garbanzo.
    /// </summary>
    public NPC Garbanzo;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Garbanzo = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Garbanzo_Greeting_7", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 7,
            Name = "Garbanzo",
            Job = "Dog",
            Description = "Garbanzo is a simple-minded but endlessly enthusiastic golden retriever. " +
            "He is technically Mark�s dog but roams freely around Babel. He adores everyone but is obsessed " +
            "with Teddy the cat, constantly chasing him in an effort to play. His other favorite pastime is " +
            "playing soccer with Ace, where he mysteriously seems to understand the rules. He rarely speaks " +
            "more than a few words but expresses pure joy in every movement.",
            Personality = new List<string> { "Energetic", "Loyal", "Innocent", "Playful" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "Tire Swing", time = 8, location = "Overworld" },
               new() { waypoint = "Tire Swing", time = 9, location = "Overworld" },
               new() { waypoint = "Tire Swing", time = 10, location = "Overworld" },
               new() { waypoint = "Park Bench 2", time = 11, location = "Overworld" },
               new() { waypoint = "Park Bench 2", time = 12, location = "Overworld" },
               new() { waypoint = "Park Bench 2", time = 13, location = "Overworld" },
               new() { waypoint = "Park Bench 2", time = 14, location = "Overworld" },
               new() { waypoint = "Park Top", time = 15, location = "Overworld" },
               new() { waypoint = "Park Bottom", time = 16, location = "Overworld" },
               new() { waypoint = "Park Top", time = 17, location = "Overworld" },
               new() { waypoint = "Park Bottom", time = 18, location = "Overworld" },
               new() { waypoint = "Park Top", time = 19, location = "Overworld" },
               new() { waypoint = "Park Bottom", time = 20, location = "Overworld" },
               new() { waypoint = "Park Top", time = 21, location = "Overworld" },
               new() { waypoint = "Park Bottom", time = 22, location = "Overworld" },
               new() { waypoint = "Park Top", time = 23, location = "Overworld" },
               new() { waypoint = "Park Bottom", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Park",
            CurrentCoordinates = new Vector2(5, 5)
        };
        Garbanzo.agent.updateRotation = false;
        Garbanzo.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Garbanzo);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Garbanzo.inDialogue = true;
            Debug.Log("Garbanzo in dialogue");
            DialogueManager.Instance.StartDialogue(Garbanzo);
        }
    }
}
