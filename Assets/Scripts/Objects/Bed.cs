using UnityEngine;

public class Bed : MonoBehaviour
{
    public GameObject endDayMenu;
    bool _canSleep = false;     // Added this to prevent the menu from being opened even when not sleeping

    private void OnEnable()
    {
        GameClock.Instance.OnSleepPending += () => _canSleep = true;
        GameClock.Instance.OnDayStart    += () => _canSleep = false;
    }

    private void OnDisable()
    {
        GameClock.Instance.OnSleepPending -= () => _canSleep = true;
        GameClock.Instance.OnDayStart    -= () => _canSleep = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _canSleep)
        {
            Debug.Log("Near bed. Press E to sleep.");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (! _canSleep) return;
        if (!collision.CompareTag("Player")) return;

        if (InputManager.Instance.GetAction("Interact").triggered
            && !MenuManager.Instance.isPaused
            && !DialogueManager.Instance.isTalking)
        {
            Debug.Log("Player is sleeping...");
            endDayMenu.SetActive(true);
        }
    }
}
