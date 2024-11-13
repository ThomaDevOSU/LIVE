using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class <c>TaskManager</c> is in charge of generating tasks for the user to
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance; //  Singleton variable

    private List<Task> TaskList;   //  Our list of tasks that will be generated

    [SerializeField]
    public Task ActiveTask;    //  Our currently tracked task

    private void Awake() // Singleton
    {
        if (Instance == null)
        {
            Instance = this;

            TaskList = new List<Task>();   

            StartCoroutine(waitForManagers()); // Wait for other managers to load

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator waitForManagers() // This function will allow us to reliably wait for other magement system to initialize before we take actions requiring them
    {
        while (!(GameManager.Instance && LocalizationManager.Instance)) yield return new WaitForSeconds(0.1f);
        GenerateTasks(1);
    }

    /// <summary>
    /// Takes two parameters, name and location, and searches through the TaskList for matching tasks
    /// </summary>
    /// <param name="name">Name of the NPC being spoken to</param>
    /// <param name="location">Location of NPC being spoken to</param>
    /// <returns>first task found matching name or location</returns>
    public Task SubjectTask(string name, string location) 
    {
        foreach (Task task in TaskList) 
        {
            if (task?.TaskNPC == name || task?.TaskLocation == location) 
            {
                return task;
            }
        }
        return null;
    }

    /// <summary>
    /// GenerateTasks will remove any remaining tasks before generating three new tasks based on calculated skill level
    /// </summary>
    /// <param skill="skill">The calculate skill level</param>>
    public void GenerateTasks(int skill) 
    {
        TaskList.Clear();
        for (int i = 0; i < 3; i++) 
        {
            TaskList.Add(GenerateTask(skill));
            TaskList[i].printData();
        }
        ActiveTask = TaskList[0];
    }

    /// <summary>
    /// The GenerateTask function first rolls a two sided dice to decide whether the task will be based
    /// on a Location on the map, or the name of an NPC. Then it will generate a random conversational task
    /// relevant to the NPC/Location {IE. Buy bread from Baker, ask about weather at Park }
    /// String data must be in a format that is easy to translate
    /// </summary>
    /// <param name="skill">The calculate skill level</param>
    /// <returns>Generates a task, filled with</returns>
    private Task GenerateTask(int skill) 
    {
        //  Relevant task strings that will be assigned
        string taskDescription = null, taskNPC = null, taskLocation = null, taskSubject = null, taskAnswer = null;

        //  Relevant task type, that does not get saved to the task itself
        string taskType = null;

        System.Random random = new System.Random();

        //Flip coin, if it lands on 0, generate Task
        if (random.Next(2) == 0)
        {
            taskNPC = GetRandomNPC().ToString();
        }
        else 
        {
            taskLocation = GetRandomLocation().ToString();
        }

        //  Generate relevant subject and type
        taskSubject = GetSubject(taskNPC, taskLocation);
        taskType = GetTaskType(taskNPC, taskLocation);

        //Generate LCLZ class with relevant descriptions and answers
        var taskDetails = GetTaskTemplates(taskType, "English");

        //Debug.Log($"Printing variables, {taskNPC}\n{taskLocation}\n{taskSubject}\n{taskType}\n{taskDetails}");

        //Assign values, replace strings with proper context
        if (taskNPC==null) //   taskLocation is relevant?
        {
            taskDescription = taskDetails.description.Replace("{subject}", taskSubject).Replace("{location}", taskLocation);
            taskAnswer = taskDetails.answer.Replace("{subject}", taskSubject).Replace("{location}", taskLocation);
        } 
        else    //  taskNPC is relevant?
        {
            taskDescription = taskDetails.description.Replace("{npc}", taskNPC).Replace("{subject}", taskSubject);
            taskAnswer = taskDetails.answer.Replace("{npc}", taskNPC).Replace("{subject}", taskSubject);
        }

        //  This section is dedicated to the translatable section, this requires that the localizer be loaded
        string t_taskDescription = null, t_taskNPC = null, t_taskLocation = null, t_taskSubject = null, t_taskAnswer = null;

        t_taskSubject = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskSubject) ? LocalizationManager.Instance.localizedLearningText[taskSubject] : taskSubject;

        //Debug.Log($"Displaying data {taskNPC},\n{taskLocation},\n{taskSubject},\n{taskType},\n{taskDescription},\n{taskAnswer}");

        var t_taskDetails = GetTaskTemplates(taskType, LocalizationManager.Instance.learningLanguage);
        if (taskNPC == null)    //   taskLocation is relevant?
        {
            t_taskLocation = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskLocation) ? LocalizationManager.Instance.localizedLearningText[taskLocation] : taskLocation;

            t_taskDescription = t_taskDetails.description.Replace("{subject}", t_taskSubject).Replace("{location}", t_taskLocation);
            t_taskAnswer = t_taskDetails.answer.Replace("{subject}", t_taskSubject).Replace("{location}", t_taskLocation);
        }
        else    //  taskNPC is relevant?
        {
            t_taskNPC = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskNPC) ? LocalizationManager.Instance.localizedLearningText[taskNPC] : taskNPC;       
           
            taskDescription = taskDetails.description.Replace("{npc}", t_taskNPC).Replace("{subject}", t_taskSubject);
            taskAnswer = taskDetails.answer.Replace("{npc}", t_taskNPC).Replace("{subject}", t_taskSubject);
        }

        return new Task
        {
            TaskDescription = taskDescription,
            TaskSubject = taskSubject,
            TaskNPC = taskNPC,
            TaskLocation = taskLocation,
            TaskAnswer = taskAnswer,

            T_TaskDescription = t_taskDescription,
            T_TaskSubject = t_taskSubject,
            T_TaskNPC = t_taskNPC,
            T_TaskLocation = t_taskLocation,
            T_TaskAnswer = t_taskAnswer,
            IsCompleted = false
        };
    }

    /// <summary>
    /// Generates random npc based on available npcs defined in TaskNPCs
    /// </summary>
    /// <returns>Enum of NPC</returns>
    private TaskNPCs GetRandomNPC() 
    {
        System.Random random = new System.Random();
        return (TaskNPCs)random.Next(System.Enum.GetNames(typeof(TaskNPCs)).Length);
    }

    /// <summary>
    /// Generates random location based on available locations defined in TaskLocations
    /// </summary>
    /// <returns>Enum of Location</returns>
    private TaskLocations GetRandomLocation()
    {
        System.Random random = new System.Random();
        return (TaskLocations)random.Next(System.Enum.GetNames(typeof(TaskLocations)).Length);
    }

    /// <summary>
    /// GetSubject attempts to generate relevant subjects given the name of an NPC or location
    /// If no relevant subject can be found, subject generated will be more vague and applicable
    /// 
    /// </summary>
    /// <param name="npc">The name of the NPC, is case-sensitive</param>
    /// <param name="location">The name of the Location, is case-sensitive</param>
    /// <returns>Returns random string of relevant subject</returns>
    private string GetSubject(string npc, string location)
    {
        //Creates baseline dictionary that we will add to
        Dictionary<string, List<string>> Subjects = new Dictionary<string, List<string>>();

        //Creates a default subject list that will be returned if the key cannot be found
        List<string> defaultSubjects = new List<string> { "General" };

        //Is the location non-existent/non-relevant?
        if (location == null)
        {
            //We begin adding a list of relevant subjects to this dictionary
            Subjects["Baker"] = new List<string> { "Bread", "Pastry" };
            Subjects["Barista"] = new List<string> { "Coffee", "Tea" };

            //We return a random element from the list given a correct key
            return Subjects.ContainsKey(npc) ? Subjects[npc][Random.Range(0, Subjects[npc].Count)] : defaultSubjects[Random.Range(0, defaultSubjects.Count)];
        }
        else // The location exists and is the relevant task data
        {
            //We begin adding a list of relevant subjects to this dictionary
            Subjects["Bakery"] = new List<string> { "Bread", "Pastry" };
            Subjects["Cafe"] = new List<string> { "Coffee", "Tea" };
            Subjects["Restaurant"] = new List<string> { "Reservation", "Menu" };
            //We return a random element from the list given a correct key
            return Subjects.ContainsKey(location) ? Subjects[location][Random.Range(0, Subjects[location].Count)] : defaultSubjects[Random.Range(0, defaultSubjects.Count)];
        }
    }

    /// <summary>
    /// GetTaskTypeForLocation attempts to generate relevant task type given the name of a Location or NPC
    /// If no relevant subject can be found, task type generated will be more vague
    /// 
    /// </summary>
    /// <param name="location">The name of the Location, is case-sensitive</param>
    /// <param name="npc">The name of the NPC, is case-sensitive</param>
    /// <returns>Returns random string of relevant task type</returns>
    private string GetTaskType(string npc, string location)
    {
        //Creates baseline dictionary that we will add to
        Dictionary<string, List<string>> Tasks = new Dictionary<string, List<string>>();

        //Creates a default task list that will be returned if the key cannot be found
        List<string> defaultTasks = new List<string> { "General" };

        //Is the location non-existent/non-relevant?
        if (location == null)
        {
            //We begin adding a list of relevant tasks to this dictionary
            Tasks["Baker"] = new List<string> { "Buy" };
            Tasks["Barista"] = new List<string> { "Buy" };

            //We return a random element from the list given a correct key
            return Tasks.ContainsKey(npc) ? Tasks[npc][Random.Range(0, Tasks[npc].Count)] : defaultTasks[Random.Range(0, defaultTasks.Count)];
        }
        else // The location exists and is the relevant task data
        {
            //We begin adding a list of relevant task to this dictionary
            Tasks["Bakery"] = new List<string> { "AskLoc" };
            Tasks["Cafe"] = new List<string> { "AskLoc" };
            Tasks["Restaurant"] = new List<string> { "AskLoc" };
            //We return a random element from the list given a correct key
            return Tasks.ContainsKey(location) ? Tasks[location][Random.Range(0, Tasks[location].Count)] : defaultTasks[Random.Range(0, defaultTasks.Count)];
        }
    }

    /// <summary>
    /// Attempts to return the value at the location
    /// </summary>
    /// <param name="taskType">The Task type of the generated task</param>
    /// <param name="language">The desired translation language</param>
    /// <returns>Returns a LCLZ_Value class whos attributes contain the description and answer format for our</returns>
    private LCLZ_Value GetTaskTemplates(string taskType, string language)
    {
        // This method would retrieve templates for the task type from a dictionary or JSON file
        var taskTemplates = new Dictionary<string, LCLZ_Value>();
        string path = Path.Combine(Application.streamingAssetsPath, $"Localization/{language}Tasks.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            taskTemplates = JsonUtility.FromJson<LCLZ_TaskList>(json).ToDictionary(); // Converts json to localizationdata which converts to dictionary
        }
        else
        {
            Debug.LogError($"Localization file not found: {path}");
        }

        return taskTemplates.ContainsKey(taskType) ? taskTemplates[taskType] : null;
    }

}

/// <summary>
/// LCLZ_TaskList is a class whos only responsibility is to be a translation point from JSON
/// </summary>
[System.Serializable]
public class LCLZ_TaskList
{
    public List<LCLZ_Entry> entries;

    public Dictionary<string, LCLZ_Value> ToDictionary()
    {
        Dictionary<string, LCLZ_Value> dict = new Dictionary<string, LCLZ_Value>();
        foreach (var entry in entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}

/// <summary>
/// LCLZ_Entry will be our entry point into loading our JSON file into a dictionary
/// </summary>
[System.Serializable]
public class LCLZ_Entry
{
    public string key;
    public LCLZ_Value value;
}

/// <summary>
/// LCLZ_Value will contain the descriptions and answers
/// </summary>
[System.Serializable]
public class LCLZ_Value 
{
    public string description;
    public string answer;
}


/// <summary>
/// NPCS that can be chosen for generated tasks
/// </summary>
enum TaskNPCs { Baker, Barista }

/// <summary>
/// Locations that can be chosen for generated tasks
/// </summary>
enum TaskLocations { Bakery, Cafe, Restaurant}