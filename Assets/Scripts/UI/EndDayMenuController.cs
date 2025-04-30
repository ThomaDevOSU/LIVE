using UnityEngine;

public class EndDayMenuController : MonoBehaviour
{   
    public GameObject panelRoot;

    public void OnContinue()
    {
        // Hide Menu
        panelRoot.SetActive(false);
        Time.timeScale = 1f;

        // Advance the Day
        GameClock.Instance.GoToSleep();
    }
}


