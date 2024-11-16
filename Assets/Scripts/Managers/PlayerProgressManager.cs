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
        string taskId = completedTask.ToString();

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
