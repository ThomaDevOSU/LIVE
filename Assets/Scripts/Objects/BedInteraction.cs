using UnityEngine;
using UnityEngine.UI;

public class BedInteraction : MonoBehaviour
{
    private bool isPlayerInRange = false;
    [SerializeField] private GameObject interactionPrompt; // UI prompt message

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false); // Hide prompt at start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure it's the player
        {
            isPlayerInRange = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true); // Show prompt
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false); // Hide prompt
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // Press E to interact
        {
            GameClock.Instance.ForceSleep(); // Test Trigger sleep routine
            //GameClock.Instance.GoToSleep(); // Trigger sleep routine
            Debug.Log("Player slept. New day begins.");
        }
    }
}
