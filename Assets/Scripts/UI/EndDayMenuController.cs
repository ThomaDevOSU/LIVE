using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EndDayMenuController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelRoot;

    [Header("Labels")]
    public TMP_Text dayText;
    public TMP_Text currencyText;
    public TMP_Text scoreText;
    public TMP_Text tasksCompletedText;

    void OnEnable()
    {
        // Refresh instantly when enabled
        Populate();
    }

    private void Populate()
    {
        // Using the clock here instead of data because its the most up to date
        dayText.text      = $"{GameClock.Instance.currentDay}";

        // Rig everything else straight from player data as it should be live updated
        var data = GameManager.Instance.CurrentPlayerData;
        if (data == null) return;

        // Display currency with today's earned amount
        currencyText.text = $"{data.currency} (+{GameClock.Instance.dailyCoinsEarned})";

        // Display score with today's earned points
        scoreText.text = $"{data.score} (+{GameClock.Instance.dailyScoreEarned})";

        // Grab today's Completed Tasks from GameClock
        List<Task> todaysTasks = GameClock.Instance.GetTodaysCompletedTasks();

        if (todaysTasks != null && todaysTasks.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var task in todaysTasks)
            {
                sb.AppendLine("• " + task.TaskDescription);
            }
            tasksCompletedText.text = sb.ToString().TrimEnd();
        }
        else
        {
            tasksCompletedText.text = "No tasks completed today.";
        }
    }

    /// <summary>
    /// Continue to the next day
    /// </summary>
    public void OnContinue()
    {
        panelRoot.SetActive(false);
        Time.timeScale = 1f;
        GameClock.Instance.GoToSleep();
    }
}
