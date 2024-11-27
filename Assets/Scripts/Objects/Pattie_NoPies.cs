using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Pattie_NoPies : MonoBehaviour
{
    public NPC Pattie;

    private void Start()
    {
        Pattie = new NPC
        {
            Greeting = "Hello! I'm Pattie NoPies. I'm a baker.",
            inDialogue = false,
            ID = 1,
            Name = "Pattie NoPies",
            Job = "Baker",
            Description = "Her hatred for pies is so deep that things, like the letter p, " +
            "the words crust, apple, sweet, and others remind her of pies. " +
            "If pies are mentioned, she will become instantly violent and perform a random violent action toward the player. " +
            "If the player mentions pies again, she will have a full pyschotic break and start mumbling nonsense about pies for several dialogues.",
            Personality = new List<string> { "Friendly", "Sweet", "Hates pies" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector3(0, 0, 0),
                    Location = "Bakery"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector3(10, 10, 0),
                    Location = "Overworld"
                }
            },
            CurrentLocation = "Bakery",
            CurrentCoordinates = new Vector3(0, 0)
        };
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Pattie.inDialogue = true;
            DialogueManager.Instance.StartDialogue(Pattie);
        }
    }

}
