using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

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

        currencyText.text       = $"{data.currency}";
        scoreText.text          = $"{data.score}";
        if (data.completedTasks != null && data.completedTasks.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var t in data.completedTasks)
            {
                sb.AppendLine("• " + t.TaskDescription);
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
