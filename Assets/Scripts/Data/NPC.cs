using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    public string Greeting;
    public bool inDialogue = false;
    public int ID;
    public string Name;
    public string Job;
    public string Description;
    public List<string> Personality;
    public ScheduleEntry[] Schedule;
    public List<Message> messages;
    public string CurrentLocation { get; set; }
    public Vector3 CurrentCoordinates { get; set; }
    // It may or may not be necessary to add translated fields here

    public void printData()
    {
        Debug.Log($"Printing data about this NPC...\n" +
                  $"Greeting: {Greeting}\n" +
                  $"Name: {Name}\n" +
                  $"Job: {Job}\n" +
                  $"Description: {Description}\n" +
                  $"Personality: {string.Join(", ", Personality)}\n" +
                  $"Current Location: {CurrentLocation}\n" +
                  $"Current Coordinates: {CurrentCoordinates}\n" +
                  $"Is in Dialogue: {inDialogue}");
        Debug.Log("Printing NPC's messages...");
        foreach (Message message in messages)
        {
            Debug.Log($"Role: {message.role}\n" + $"Content: {message.content}");
        }
    }

}

public class ScheduleEntry
{
    public Vector2 Coordinates { get; set; }
    public string Location { get; set; }
    // public string time { get; set; }
}

/// <summary>
/// Represents a message in the GPT API conversation, including the role and content.
/// </summary>
[System.Serializable]
public class Message
{
    /// <summary>
    /// The role of the message (e.g., "user" for player input or "system" for API responses).
    /// </summary>
    public string role;

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string content;
}