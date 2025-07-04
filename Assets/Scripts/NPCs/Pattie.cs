using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Pattie_Baker : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Pattie.
    /// </summary>
    public NPC Pattie;


    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Pattie = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Pattie_Greeting_8", isUI.NOTUI)
            },
            inDialogue = false,
            ID = 1,
            Name = "Pattie",
            Job = "Baker",
            Description = "Pattie is a warm and friendly baker who is well-loved in Babel for her delicious treats�except pies, which she refuses to make. " +
            "Once a contestant on a reality cooking show, she suffered a humiliating critique from a famous British chef, leaving her with a deep aversion to pies. " +
            "She finds joy in mentoring young bakers like Alex, even though she pretends to see him as competition. She enjoys community gatherings, " +
            "offering free pastries to those in need, and keeping the town well-fed with her kindness and humor.",
            Personality = new List<string> { "Friendly", "Empathetic", "Loyal", "Avoids talking about pies" },
            Schedule = new List<ScheduleEntry>
            {
               new() { waypoint = "BakeryBack", time = 8, location = "Bakery" },
               new() { waypoint = "Cafe Table 2", time = 9, location = "Overworld" },
               new() { waypoint = "BakeryClerk", time = 10, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 11, location = "Bakery" },
               new() { waypoint = "BakerySink", time = 12, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 13, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 14, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 15, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 16, location = "Bakery" },
               new() { waypoint = "BakeryClerk", time = 17, location = "Bakery" },
               new() { waypoint = "CafeLine", time = 18, location = "Cafe" },
               new() { waypoint = "CafeCounter", time = 19, location = "Cafe" },
               new() { waypoint = "BakeryFry", time = 20, location = "Bakery" },
               new() { waypoint = "BakerySink", time = 21, location = "Bakery" },
               new() { waypoint = "BakerySink", time = 22, location = "Bakery" },
               new() { waypoint = "House 2-2", time = 23, location = "Overworld" },
               new() { waypoint = "House 2", time = 24, location = "Overworld" }
            },
            messages = new List<Message>(),
            CurrentLocation = "Bakery",
            CurrentCoordinates = new Vector2(0, 0)
        };
        Pattie.agent.updateRotation = false;
        Pattie.agent.updateUpAxis = false;
        NPCManager.Instance.AddNPC(Pattie);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Pattie.inDialogue = true;
            Debug.Log("Pattie in dialogue");
            DialogueManager.Instance.StartDialogue(Pattie);
        }
    }
}

