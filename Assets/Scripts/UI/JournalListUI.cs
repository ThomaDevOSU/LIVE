using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalListUI : MonoBehaviour
{
    public GameObject taskButtonPrefab;
    public Transform taskListContainer;

    private void OnEnable()
        // Added this bad boy to help fix journal freezing when completeing task
    {
        DisplayActiveTasks();
    }

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
            buttonText.font = LocalizationManager.Instance.MasterFont;  //  The master font will support all our different languages

            // Highlight
            if (task == currentActiveTask)
            {
                button.GetComponent<Image>().color = new Color(170f / 255f, 121f / 255f, 89f / 255f);
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




