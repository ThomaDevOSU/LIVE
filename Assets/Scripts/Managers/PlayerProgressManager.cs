using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager Instance;

    // List to store completed task data
    private List<Task> completedTasks;

    // Currency tracking
    private int playerCurrency;

    // Mission statistics
    private Dictionary<string, MissionStats> missionStats;

    // Rewards tracking
    private List<string> playerRewards;
    

    private void Awake() // Singleton pattern
    {
        if (Instance == null)
        {
            Instance = this;
            completedTasks = new List<Task>();
            playerCurrency = 0;
            missionStats = new Dictionary<string, MissionStats>();
            playerRewards = new List<string>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load player data from GameManager into PlayerProgressManager
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayerData != null)
        {
            PlayerProgressManager.Instance.LoadProgressFromPlayerData(GameManager.Instance.CurrentPlayerData);
            Debug.Log("Loaded player progress data from GameManager.");
        }
        else
        {
            Debug.LogWarning("GameManager or CurrentPlayerData is null. Unable to load player progress.");
        }
    }

    /// <summary>
    /// Saves the completed task information to track player progress
    /// </summary>
    /// <param name="completedTask">The task that has been completed</param>
    public void SaveCompletedTask(Task completedTask)
    {
        // Add completed task to the list for tracking
        completedTasks.Add(completedTask);

        Debug.Log("Task completed and saved: " + completedTask.ToString());

        // Record mission completion statistics
        RecordMissionCompletion(completedTask, true);
    }

    // Currency Management Functions
    public void AddCurrency(int amount)
    {
        playerCurrency += amount;
        Debug.Log($"Currency Added: {amount}. Total: {playerCurrency}");
    }

    public bool SpendCurrency(int amount)
    {
        if (amount > playerCurrency)
        {
            Debug.Log("Not enough currency to complete this action.");
            return false;
        }

        playerCurrency -= amount;
        Debug.Log($"Currency Spent: {amount}. Total: {playerCurrency}");
        return true;
    }

    public int GetCurrency()
    {
        return playerCurrency;
    }

    // Mission Completion Tracking Functions
    public void RecordMissionCompletion(Task completedTask, bool success)
    {
        string taskId = completedTask.TaskDescription;

        if (!missionStats.ContainsKey(taskId))
        {
            missionStats[taskId] = new MissionStats(taskId);
        }

        missionStats[taskId].UpdateStats(success);
        Debug.Log($"Task '{taskId}' completion recorded. Success: {success}");
    }

    public MissionStats GetMissionStats(string taskId)
    {
        if (missionStats.ContainsKey(taskId))
        {
            return missionStats[taskId];
        }

        Debug.LogWarning($"No stats found for task '{taskId}'");
        return null;
    }
    public Dictionary<string, MissionStats> GetAllMissionStats()
    {
        return new Dictionary<string, MissionStats>(missionStats);
    }

    // Reward System Functions
    public void AddReward(string reward)
    {
        if (!playerRewards.Contains(reward))
        {
            playerRewards.Add(reward);
            Debug.Log($"Reward '{reward}' added.");
        }
    }

    public bool HasReward(string reward)
    {
        return playerRewards.Contains(reward);
    }

    public List<string> GetAllRewards()
    {
        return new List<string>(playerRewards);
    }

    // Method to get the list of completed tasks
    public List<Task> GetCompletedTasks()
    {
        return new List<Task>(completedTasks);
    }

    /// <summary>
    /// Prints all completed tasks to the console, each on one line separated by '|'.
    /// If no tasks are completed, it prints a message indicating no completed tasks.
    /// </summary>
    public void PrintCompletedTasks()
    {
        if (completedTasks.Count > 0)
        {
            Debug.Log("----- Completed Tasks -----");
            Debug.Log("Index | Description | Subject | NPC | Location | Difficulty | Custom Task");
            Debug.Log("------------------------------------------------------------");

            for (int i = 0; i < completedTasks.Count; i++)
            {
                Task task = completedTasks[i];
                Debug.Log($"{i + 1} | {task.TaskDescription} | {task.TaskSubject} | {task.TaskNPC ?? "N/A"} | {task.TaskLocation ?? "N/A"} | {task.TaskDifficulty} | {task.IsCustom}");
            }

            Debug.Log("------------------------------------------------------------");
        }
        else
        {
            Debug.Log("No completed tasks to display.");
        }
    }

    // Save Progress to PlayerData
    public void SaveProgressToPlayerData(PlayerData data)
    {
        data.currency = playerCurrency;
        data.completedTasks = GetCompletedTasks();
        data.missionStats = GetAllMissionStats();
        data.playerRewards = GetAllRewards();
        data.conversationHistory = NPCManager.Instance.GetConversationHistory();
    }

    // Load Progress from PlayerData
    public void LoadProgressFromPlayerData(PlayerData data)
    {
        playerCurrency = data.currency;
        completedTasks = new List<Task>(data.completedTasks);
        missionStats = new Dictionary<string, MissionStats>(data.missionStats);
        playerRewards = new List<string>(data.playerRewards);
        
    }

}

// Class to store mission statistics
[System.Serializable]
public class MissionStats
{
    public string missionName;
    public int attempts;
    public int successes;

    public MissionStats(string name)
    {
        missionName = name;
        attempts = 0;
        successes = 0;
    }

    public void UpdateStats(bool success)
    {
        attempts++;
        if (success)
        {
            successes++;
        }
    }

    public float GetSuccessRate()
    {
        if (attempts == 0) return 0f;
        return (float)successes / attempts;
    }
}
