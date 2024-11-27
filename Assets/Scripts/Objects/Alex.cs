using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Alex_Barista : MonoBehaviour
{
    public NPC Alex;

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
                    Coordinates = new Vector3(5, 5, 0),
                    Location = "Coffee Shop"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector3(15, 15, 0),
                    Location = "Overworld"
                }
            },
            CurrentLocation = "Coffee Shop",
            CurrentCoordinates = new Vector3(5, 5, 0)
        };
        NPCManager.Instance.AddNPC(Alex);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Alex.inDialogue = true;
            DialogueManager.Instance.StartDialogue(Alex);
        }
    }
}

