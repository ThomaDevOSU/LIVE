using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

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
        Ava = new NPC
        {
            agent = GetComponent<NavMeshAgent>(),
            animator = GetComponent<Animator>(),
            Greeting = new string[]
            {
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_1", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_2", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_3", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_4", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_5", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_6", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_7", isUI.NOTUI),
                LocalizationManager.Instance.GetLocalizedValue("Ava_Greeting_8", isUI.NOTUI)
            },
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
            Schedule = new List<ScheduleEntry>
            {
                new()
                {
                    waypoint = "House 3",
                    time = 8,
                    location = "Overworld"
                },
                new()
                {
                    waypoint = "House 2",
                    time = 10,
                    location = "Overwold"
                },
                new()
                {
                    waypoint = "Overworld",
                    time = 14,
                    location = "Overworld"
                }
            },
            messages = new List<Message>(),
            CurrentLocation = "Information Center",
            CurrentCoordinates = new Vector2(5, 5)
        };
        Ava.agent.updateRotation = false;
        Ava.agent.updateUpAxis = false;
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
