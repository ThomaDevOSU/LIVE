using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskListUI : MonoBehaviour
{
    private TMP_Text taskListText;      // Reference TMP as I almost forgot and was getting errors

    void Start()
    {
        taskListText = GetComponent<TMP_Text>();

        // As TaskManager is a singleton, pass if not relevant
        if (TaskManager.Instance == null)
        {
            Debug.LogError("TaskManager instance not found.");
            return;
        }

        StartCoroutine(DisplayTasksAfterLoad());
    }

    IEnumerator DisplayTasksAfterLoad()
    {
        yield return new WaitUntil(() => TaskManager.Instance.GetTaskList().Count > 0);
        DisplayTasks();
    }

    // Wait until load, give header, and preposition '-' with each task, then newline
    void DisplayTasks()
    {
        string taskDisplay = "Today's Tasks:\n";

        foreach (Task task in TaskManager.Instance.GetTaskList())
        {
            taskDisplay += "- " + task.TaskDescription + "\n";
        }

        taskListText.text = taskDisplay;
    }
}



