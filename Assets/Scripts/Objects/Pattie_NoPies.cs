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
        Debug.Log("Pattie_NoPies script started");
        Pattie = new NPC
        {
            Greeting = "Hello! I'm Pattie NoPies. I'm a baker.",
            inDialogue = false,
            ID = 1,
            Name = "Pattie NoPies",
            Job = "Baker",
            Description = "Pattie is a friendly middle aged women with a slight edge. She has 2 grown children and is a widow." +
            "She saw the recent addition to the town, Alex, baking sweets in his newly opened coffee shop and is afraid of the competition" +
            "because the pastries looked amazing. She acts like she doesn't respect Alex but he reminds her of her children" +
            "Pattie hates pies because of trauma during a reality TV show where a celebrity chef who she refuses to name said mean things about her pie." +
            "and he said mean things about her pies. She will hint that it was gordon ramsey but will never admit it.",
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
            CurrentCoordinates = new Vector3(0, 0, 0)
        };
        NPCManager.Instance.AddNPC(Pattie);
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
