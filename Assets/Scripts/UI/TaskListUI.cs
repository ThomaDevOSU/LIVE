using System.Collections;
using UnityEngine;
using TMPro;

public class TaskListUI : MonoBehaviour
{
    private TMP_Text taskListText;  // Reference TMP as I almost forgot and was getting errors
    private Task lastActiveTask = null; // Add a tracker for last task to help resource management

    void Start()
    {
        taskListText = GetComponent<TMP_Text>();

        // As TaskManager is a singleton, pass if not relevant
        if (TaskManager.Instance == null)
        {
            Debug.LogError("TaskManager instance not found.");
            return;
        }

        StartCoroutine(DisplayActiveTaskAfterLoad());
    }

    IEnumerator DisplayActiveTaskAfterLoad()
    {
        yield return new WaitUntil(() => TaskManager.Instance.GetActiveTask() != null);
        UpdateActiveTaskUI();
        lastActiveTask = TaskManager.Instance.GetActiveTask();

        // Changed to look at active
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Task currentActiveTask = TaskManager.Instance.GetActiveTask();

            // Only update UI when changed
            if (currentActiveTask != lastActiveTask)
            {
                UpdateActiveTaskUI();
            }
        }
        lastActiveTask = TaskManager.Instance.GetActiveTask();
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

