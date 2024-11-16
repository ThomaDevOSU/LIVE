using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerProgressTester : MonoBehaviour
{
    void Start()
    {
        // Ensure PlayerProgressManager exists in the scene
        if (PlayerProgressManager.Instance == null)
        {
            GameObject playerProgressManagerObj = new GameObject("PlayerProgressManager");
            playerProgressManagerObj.AddComponent<PlayerProgressManager>();
        }

        TestCurrencyManagement();
        TestRewardSystem();
        TestMissionCompletion();
    }

    void TestCurrencyManagement()
    {
        Debug.Log("Testing Currency Management...");

        PlayerProgressManager.Instance.AddCurrency(100);
        PlayerProgressManager.Instance.SpendCurrency(40);
        PlayerProgressManager.Instance.SpendCurrency(70);
        PlayerProgressManager.Instance.AddCurrency(50);
        Debug.Log("Final Currency: " + PlayerProgressManager.Instance.GetCurrency());
    }

    void TestMissionCompletion()
    {
        Debug.Log("Testing Mission Completion...");

        // Create sample tasks using TaskManager (assuming tasks are available)
        //TaskManager.Instance.CreateCustomTask("Baker", "Bakery", "Find the Key", "Buy", 2);
        //TaskManager.Instance.CreateCustomTask("Barista", "Cafe", "Serve Coffee", "Serve", 3);
        TaskManager.Instance.GenerateTasks(1);

        // Get and print all tasks
        Debug.Log("Testing Mission Completion...");
        Debug.Log(TaskManager.Instance.GetTaskList());
        List<Task> allTasks = TaskManager.Instance.GetTaskList();
        //Debug.Log(allTasks);
        if (allTasks.Count > 0)
        {
            Debug.Log("All Tasks:");
            foreach (Task task in allTasks)
            {
                Debug.Log("Task - NPC: " + task.TaskNPC + ", Location: " + task.TaskLocation + ", Subject: " + task.TaskSubject + ", Difficulty: " + task.TaskDifficulty);
            }
        }
        else
        {
            Debug.LogWarning("No tasks available.");
        }

        // Get and print the active task
        Task activeTask = TaskManager.Instance.GetActiveTask();
        if (activeTask != null)
        {
            Debug.Log("Active Task - NPC: " + activeTask.TaskNPC + ", Location: " + activeTask.TaskLocation + ", Subject: " + activeTask.TaskSubject + ", Difficulty: " + activeTask.TaskDifficulty);

            // Save the completed task
            PlayerProgressManager.Instance.SaveCompletedTask(activeTask);
        }
        else
        {
            Debug.LogWarning("No active task available to complete.");
        }
    }

    void TestRewardSystem()
    {
        Debug.Log("Testing Reward System...");

        PlayerProgressManager.Instance.AddReward("Golden Key");
        PlayerProgressManager.Instance.AddReward("Silver Shield");
        PlayerProgressManager.Instance.AddReward("Golden Key");  // Testing duplicate reward addition

        List<string> rewards = PlayerProgressManager.Instance.GetAllRewards();
        foreach (string reward in rewards)
        {
            Debug.Log("Reward: " + reward);
        }
    }
}
