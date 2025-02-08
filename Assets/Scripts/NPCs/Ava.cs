using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers a dialogue interaction when the player stays within a collider and presses the designated button.
/// </summary>
public class Ava_Informer : MonoBehaviour
{
    /// <summary>
    /// The NPC instance representing Ava.
    /// </summary>
    public NPC Ava;

    /// <summary>
    /// List of messages associated with the NPC.
    /// </summary>
    public List<Message> messages = new List<Message>();

    /// <summary>
    /// Initializes the NPC instance and adds it to the NPC manager.
    /// </summary>
    private void Start()
    {
        Debug.Log("Ava script started");
        Ava = new NPC
        {
            Greeting = "Oh my gosh, did you hear? Oh wait, let me tell you!",
            inDialogue = false,
            ID = 15,
            Name = "Ava",
            Job = "Informer",
            Description = "Ava is the town’s self-appointed information hub. She runs the Information Center, which is supposed to focus on the town's history" +
            ", but really, she just loves gossip. " +
            "Bubbly and charming, she acts ditzy but remembers everything about everyone. " +
            "She adores talking to people, making sure she always has the latest news. " +
            "Her favorite pastime is helping Isabella spread conspiracy theories, even though she doesn’t believe them. " +
            "She loves chatting with Mark since he listens, even if he forgets half of what she says. " +
            "She is determined to uncover Jessica’s future plans and believes Ace has a juicy backstory. " +
            "While she finds Esmeralda spooky, she secretly enjoys spreading rumors about her being a witch.",
            Personality = new List<string> { "Talkative", "Nosy", "Energetic", "Observant" },
            Schedule = new ScheduleEntry[]
            {
                new ScheduleEntry
                {
                    Coordinates = new Vector2(5, 5),
                    Location = "Information Center"
                },
                new ScheduleEntry
                {
                    Coordinates = new Vector2(8, 8),
                    Location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Information Center",
            CurrentCoordinates = new Vector2(5, 5)
        };
        NPCManager.Instance.AddNPC(Ava);
    }

    /// <summary>
    /// Triggers the dialogue interaction when the player stays within the collider and presses the designated button.
    /// </summary>
    /// <param name="collision">The collider that the player is interacting with.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButton("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking)
        {
            Ava.inDialogue = true;
            Debug.Log("Ava in dialogue");
            DialogueManager.Instance.StartDialogue(Ava);
        }
    }
}
