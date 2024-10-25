using UnityEngine;

public class Door : MonoBehaviour
{
    public string scene;
    public string LocationKey;
    public TransitionType transitionType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("Near an interactable object. Press E to interact.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetButtonDown("Jump") && !MenuManager.Instance.isPaused && !DialogueManager.Instance.isTalking) // Only if we arent talking
        {
            //Debug.Log("Interacting with the door");
            GameManager.Instance.setLocation(LocationKey); // Set where we should end up
            TransitionManager.Instance.StartTransition(scene,transitionType);
        }
    }
}
