using UnityEngine;
using TMPro;

public class GameClockUI : MonoBehaviour
{
    private TextMeshProUGUI clockTMP;
    private int lastDisplayedMinute = -1;           // Store last displayed
    private void Awake()
    {
        clockTMP = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        /*
        This is for my reference:
        1 Minute: 24f * 14f
        5 Minutes: 24f * 12f
        10 Minutes: 24f * 6f
        30 Minutes: 24f * 2f
        */

        // Every seccond for now as it bugs
        float GameTime = (GameClock.Instance.gameDayDuration / (24f * 60f));

        // Update dat UI
        InvokeRepeating(nameof(UpdateClock), 0f, GameTime);
    }

    private void UpdateClock()
    {
        // Get the GameClock singleton and set dat time up
        if (GameClock.Instance != null)
        {
            // Get the time
            string formattedTime = GameClock.Instance.GetFormattedTime();
            int currentMinute = GetMinutesFromFormattedTime(formattedTime);

            // Only update clock if five
            if (currentMinute % 5 == 0 && currentMinute != lastDisplayedMinute)
            {
                clockTMP.text = formattedTime;
                lastDisplayedMinute = currentMinute;
            }
        }
    }

    private int GetMinutesFromFormattedTime(string time)
    {
        // Needed this to help
        string[] timeParts = time.Split(':');
        return int.Parse(timeParts[1]);
    }

    /*
    Prewrote this for the future. Incase we wanted 12 hour formatting.

    private string Convert12Hour(string time24)
    {
        string[] timeParts = time24.Split(':');
        int hour = int.Parse(timeParts[0]);
        string minute = timeParts[1];
        string period = hour >= 12 ? "PM" : "AM";
        int hour12 = (hour % 12 == 0) ? 12 : hour % 12;
        return $"{hour12}:{minute} {period}";
    }
    */
}


