using UnityEngine;

[SerializeField]
public class Task
{
    /// <summary>
    /// General description of task {IE. Go buy some bread from the bakery, go ask Jake about the weather, go buy coffee, etc...}
    /// </summary>
    public string TaskDescription { get; set; }
    /// <summary>
    /// The Subject of the task {IE. Bread, weather, coffee}
    /// </summary>
    public string TaskSubject { get; set; }
    /// <summary>
    /// The Name of the NPC {IE. Baker, Barista, Jake}
    /// </summary>
    public string TaskNPC { get; set; }
    /// <summary>
    /// The Location of the task {IE. Park, Library, Cafe}
    /// </summary>
    public string TaskLocation { get; set; }
    /// <summary>
    /// The answer to the task {IE. "Can I buy a loaf of bread?" or "Hello Jake, how is the Weather?"}
    /// </summary>
    public string TaskAnswer { get; set; }

    /// <summary>
    /// The TaskType is used to define what type {IE. Buy, Sell, Ask, etc} of task
    /// the user has been assigned. The Dictionary loaded from JSON uses this to assign relevant
    /// Tasks. {IE. Buying only from vendors, and Asking random subject question to other NPCS}
    /// </summary>
    public string TaskType { get; set; }

    /// <summary>
    /// TRANSLATED: General name for task {IE. Get Bread! Talk about weather! Get Coffee}
    /// </summary>

    public string T_TaskDescription { get; set; }
    /// <summary>
    /// TRANSLATED: The Subject of the task {IE. Bread, weather, coffee}
    /// </summary>
    public string T_TaskSubject { get; set; }
    /// <summary>
    /// TRANSLATED: The Name of the NPC {IE. Baker, Barista, Jake}
    /// </summary>
    public string T_TaskNPC { get; set; }
    /// <summary>
    /// TRANSLATED: The Location of the task {IE. Park, Library, Cafe}
    /// </summary>
    public string T_TaskLocation { get; set; }
    /// <summary>
    /// TRANSLATED: The answer to the task {IE. "Can I buy a loaf of bread?" or "Hello Jake, how is the Weather?"}
    /// </summary>
    public string T_TaskAnswer { get; set; }


    /// <summary>
    /// Is the Task is completed?
    /// </summary>
    /// <returns>True if task is completed, false if not</returns>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// printData prints every value of a generated task
    /// </summary>
    public void printData() 
    {
        Debug.Log($"Printing data about this Task...\n" + $"Task Subject: {TaskSubject}\n" + $"TaskNPC: {TaskNPC}\n" + $"TaskLocation: {TaskLocation}\n" + $"TaskDescription: {TaskDescription}\n" + $"TaskAnswer: {TaskAnswer}");
        Debug.Log($"Printing data about this Task but translated...\n" + $"Task Subject: {T_TaskSubject}\n" + $"TaskNPC: {T_TaskNPC}\n" + $"TaskLocation: {T_TaskLocation}\n" + $"TaskDescription: {T_TaskDescription}\n" + $"TaskAnswer: {T_TaskAnswer}");
    }
}
