using UnityEngine;

public class Bed : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Near bed. Press E to attempt sleep.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (InputManager.Instance.GetAction("Interact").triggered)  GameClock.Instance.TrySleep();
    }
}
