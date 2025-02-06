using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Initiates dialogue when the player stays within a collider and presses the designated button.
/// </summary>
public class Alex_Barista : MonoBehaviour
{
    public NPC Alex;
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Alex script started");
        Alex = new NPC
        {
            Greeting = "Hello! I'm Alex. I'm a barista.",
            inDialogue = false,
            ID = 2,
            Name = "Alex",
            Job = "Barista",
            Description = "Alex is a youngish friendly barista who loves coffee and enjoys chatting with customers. " +
            "He is always ready to share his knowledge. He has recently been experimenting with baked goods to sell with the coffee." +
            "He is a very caring person and is always making sure people are healthy, safe, and not overconsuming caffeine.",
            Personality = new List<string> { "Friendly", "Gentle", "Knowledgeable", "Coffee Lover" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 5),
                    Location = "Coffee Shop"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(15, 15),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Coffee Shop",
            CurrentCoordinates = new Vector2(5, 5)
        };
        NPCManager.Instance.AddNPC(Alex);
    }
    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision"></param>
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

