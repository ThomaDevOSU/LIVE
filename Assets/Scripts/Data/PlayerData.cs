using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string language; // What language are we trying to learn?
    public string name; // What is the character's name?
    public Gender gender; // Gender choice, 0 == female, 1 == male... Defaults to male
    public int day; // What day are we on?
    public int currency; // How much money do we have?
    public int score; // How many tasks have we completed?
    public int playerSprite; // What character model is being used?

    // Fields for Player Progress
    public List<Task> completedTasks; // Store completed tasks directly
    public Dictionary<string, MissionStats> missionStats; // Store mission statistics directly
    public List<string> playerRewards; // Store player rewards directly

    // Conversation History
    public Dictionary<string, List<Message>> conversationHistory;

    public PlayerData()
    {
        language = "";
        name = "";
        gender = Gender.male;
        day = 1;
        currency = 0;
        score = 0;
        playerSprite = 0;
        completedTasks = new List<Task>();
        missionStats = new Dictionary<string, MissionStats>();
        playerRewards = new List<string>();
        conversationHistory = new Dictionary<string, List<Message>>();
    }
}

public enum Gender { female, male };
