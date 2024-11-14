using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskListUI : MonoBehaviour
{
    private TMP_Text taskListText;   // Reference to the TMP_Text component

    void Start()
    {
        taskListText = GetComponent<TMP_Text>();

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



