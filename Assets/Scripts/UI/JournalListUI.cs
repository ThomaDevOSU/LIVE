﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalListUI : MonoBehaviour
{
    public GameObject taskButtonPrefab;
    public Transform taskListContainer;

    void Start()
    {
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
        DisplayActiveTasks();
    }

    // Wait until load, get list of tasks and include as buttons, organize active to top with pink highlight
    void DisplayActiveTasks()
    {
        // Clear
        foreach (Transform child in taskListContainer)
        {
            Destroy(child.gameObject);
        }

        // Get list
        List<Task> activeTasks = TaskManager.Instance.GetTaskList();

        // Move active
        Task currentActiveTask = TaskManager.Instance.GetActiveTask();
        if (currentActiveTask != null && activeTasks.Contains(currentActiveTask))
        {
            activeTasks.Remove(currentActiveTask);
            activeTasks.Insert(0, currentActiveTask);
        }

        // Create button
        foreach (Task task in activeTasks)
        {
            GameObject button = Instantiate(taskButtonPrefab, taskListContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = task.T_TaskDescription;

            // Highlight
            if (task == currentActiveTask)
            {
                button.GetComponent<Image>().color = new Color(1f, 0.75f, 0.8f);
            }

            // Listener and refresh
            Button taskButton = button.GetComponent<Button>();
            taskButton.onClick.AddListener(() =>
            {
                TaskManager.Instance.SetActiveTask(task);
                DisplayActiveTasks();
                FindObjectOfType<TaskListUI>()?.UpdateActiveTaskUI(); // This line fixed the error Devon lol
            });
        }
    }
}




