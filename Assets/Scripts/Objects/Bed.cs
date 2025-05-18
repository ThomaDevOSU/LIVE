using UnityEngine;

public class Bed : MonoBehaviour
{
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
        if (!_canSleep || !collision.CompareTag("Player")) return;
        if (!InputManager.Instance.GetAction("Interact").triggered
            || MenuManager.Instance.isPaused
            || DialogueManager.Instance.isTalking)
            return;

        // Capture today's tasks for use in End of Day Summary
        GameClock.Instance.CaptureTodaysCompletedTasks();

        Debug.Log("Player is sleeping…");

        // Get EndDayMenu everytime
        var mgrGO = MenuManager.Instance.gameObject;
        var endMenuTrans = mgrGO.transform.Find("EndDayMenu");
        if (endMenuTrans != null)
        {
            endMenuTrans.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Bed: Could not find EndDayMenu!");
        }
    }
}
