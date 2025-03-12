using System.Collections;
using UnityEngine;
using TMPro;

public class TaskListUI : MonoBehaviour
{
    private TMP_Text taskListText;  // Reference TMP as I almost forgot and was getting errors

    void Start()
    {
        taskListText = GetComponent<TMP_Text>();

        // As TaskManager is a singleton, pass if not relevant
        if (TaskManager.Instance == null)
        {
            Debug.LogError("TaskManager instance not found.");
            return;
        }

        // Get task notification
        TaskManager.Instance.AddTaskCompletionListener(_ => UpdateActiveTaskUI());

        StartCoroutine(DisplayActiveTaskAfterLoad());
    }

    IEnumerator DisplayActiveTaskAfterLoad()
    {
        yield return new WaitUntil(() => TaskManager.Instance.GetActiveTask() != null);
        UpdateActiveTaskUI();
        // Added to fix not updating on day end
        GameClock.Instance.OnDayStart += () => UpdateActiveTaskUI();
    }

    // Wait until load, get active task and display
    public void UpdateActiveTaskUI()
    {
        Task activeTask = TaskManager.Instance.GetActiveTask();

        if (activeTask != null)
        {
            string taskDisplay = activeTask.T_TaskDescription;
            taskListText.text = taskDisplay;
        }
        else
        {
            taskListText.text = "No active tasks.";
        }
    }
}

