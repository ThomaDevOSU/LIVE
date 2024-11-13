using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// NPCS that can be chosen for generated tasks
/// </summary>
enum TaskNPCs { Baker, Barista }

/// <summary>
/// Locations that can be chosen for generated tasks
/// </summary>
enum TaskLocations { Bakery, Cafe, Restaurant }

/// <summary>
/// Class <c>TaskManager</c> is in charge of generating tasks for the user to accomplish. 
/// It can be interfaced with the following functions:
/// <list type="table">
///     <listheader>
///         <term>SubjectTask(string name, string location)</term>
///         <description>Checks the list of tasks to see if a task corresponds to an NPCs name or location. Returning the FIRST Task that matches or null</description>
///     </listheader>
///     <item>
///         <term>GetActiveTask() </term>
///         <description>Returns the current active task, this does not effect task completion. But should be useful for task guidance</description>
///     </item>
///     <item>
///         <term>SetActiveTask(Task task)</term>
///         <description>Sets the active task to a new task, given the task is within the task list</description>
///     </item>
///     <item>
///         <term>GetTaskList()</term>
///         <description>Returns the list of Tasks the player currently has</description>
///     </item>
///     <item>
///         <term>CreateCustomTask(string npc_name, string location, string subject, string type, int difficulty) </term>
///         <description>Creates a custom task and adds it to the TaskList</description>
///     </item>
///     <item>
///         <term>CompleteTask(Task task)</term>
///         <description>Sets a tasks completion status given it exists within the task list, this removes it from the List of tasks, placing it in the Queue of CompletedTasks</description>
///     </item>
///     <item>
///         <term>GetCompletedTask()</term>
///         <description>Gets the most recently Completed Task, removing it from the CompletedTask Queue</description>
///     </item>
///     <item>
///         <term>PeekCompletedTask()</term>
///         <description>Peeks at the most recently Enqueued completed task, not removing it from the Completed task list</description>
///     </item>
///     <item>
///         <term>GenerateTasks(int count)</term>
///         <description>Generates {count} number of random tasks</description>
///     </item>
///     <item>
///         <term>ClearUncompletedTasks()</term>
///         <description>Clears the Task list of NON-CUSTOM uncompleted tasks, returns number of tasks cleared</description>
///     </item>
/// </list>
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance; //  Singleton variable

    private List<Task> TaskList;   //  Our list of tasks that will be generated
    private Queue<Task> CompletedTasks; //  Our Queue of completed tasks. When a task is completed it is placed within this Queue

    [SerializeField]
    private Task ActiveTask;    //  Our currently tracked task

    Dictionary<string, List<string>> Subjects;
    Dictionary<string, List<string>> Types;


    private void Awake() // Singleton
    {
        if (Instance == null)
        {
            Instance = this;

            TaskList = new List<Task>();    //  Instantiate our task list
            CompletedTasks = new Queue<Task>(); //  Instantiate our task Queue

            CreateTaskDictionaries();   //  Instansiate our task Dictionaries

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
        GenerateTasks(5);  
    }

    /// <summary>
    /// Used for Debugging, prints the TaskList contents out
    /// </summary>
    public void PrintTaskList() 
    {
        Debug.Log("Printing TaskList Contents");
        foreach (Task t in TaskList)
        {
            t.printData();
        }
    }

    /// <summary>
    /// Initiates the dictionaries that will be used for generating Tasks.
    /// Important Dictionaries: Subjects and Types can be created and filled here
    /// and as long as an existing enum exists.
    /// <example>
    /// Adding new Elements should go as follows:
    /// <list type="bullet">
    /// <item>
    /// <term>Subject Key</term>
    /// <description>
    /// Adding a new subject key can be likened to adding a new location or NPC, it requires 4 actions to be taken.
    /// <para>First, adding the key to the subject dictionary.</para>
    /// <para>Second, adding the key to the corresponding enum above the TaskManager class.</para>
    /// <para>Third, adding the key to the Localization Managers language JSON file. Example Spanish translation format:
    /// <code>
    /// { "key": "Baker", "value": "Panadero" }
    /// </code>
    /// </para>
    /// <para>Fourth, adding a corresponding Task Type with the same key name.</para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>Subject Value</term>
    /// <description>
    /// Adding a subject value adds contextually accurate subjects that an NPC / Location might be assigned. Doing so requires 2 actions.
    /// <para>First, given a key, add a string to the corresponding list, this is the true Subject Value</para>
    /// <para>Second, add the string value to the Localization Managers language JSON file. Example Spanish translation format:
    /// <code>
    /// { "key": "Bread", "value": "Pan" }
    /// </code>
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>Task Type</term>
    /// <description>
    /// Task types should be added based on Subject keys, they represent what type of task an individual or location might be assigned to. To add a Task Type without error:
    /// <para>First, given a key {IE. Baker}, add a Type string to the corresponding list, this is a Type of the Task that can be assigned to this NPC / Location</para>
    /// <para>Second, add the string Type value to the Localization Managers {language}Tasks JSON file. Example SpanishTasks translation format:
    /// <code>
    ///     "key": "Buy", 
    ///     "value": 
    ///     {
    ///         "description":"Compre {subject} de {npc}",
    ///         "answer":"Quisiera comprar {subject}."
    ///     }
    /// </code>
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </example>
    /// </summary>
    private void CreateTaskDictionaries() 
    {
        // Add new subjects to this section
        Subjects = new Dictionary<string, List<string>>();

        //  NPC SUBJECTS
        Subjects["Baker"] = new List<string> { "Bread", "Pastry" };
        Subjects["Barista"] = new List<string> { "Coffee", "Tea" };

        //  LOCATION SUBJECTS
        Subjects["Bakery"] = new List<string> { "Bread", "Pastry" };
        Subjects["Cafe"] = new List<string> { "Coffee", "Tea" };
        Subjects["Restaurant"] = new List<string> { "Reservation", "Menu" };

        // Add new types to this section
        Types = new Dictionary<string, List<string>>();

        //  NPC TASK TYPES
        Types["Baker"] = new List<string> { "Buy" };
        Types["Barista"] = new List<string> { "Buy" };

        //  LOCATION TASK TYPES
        Types["Bakery"] = new List<string> { "AskLoc" };
        Types["Cafe"] = new List<string> { "AskLoc" };
        Types["Restaurant"] = new List<string> { "AskLoc" };
    }

    /// <summary>
    /// A getter function for the Active Task
    /// </summary>
    /// <returns></returns>
    public Task GetActiveTask() 
    {
        return ActiveTask;
    }

    /// <summary>
    /// Sets the current Active Task to the given task
    /// </summary>
    /// <param name="task">The task that will be set as active, needs to be within the TaskList</param>
    /// <returns>True if succesful, false if not</returns>
    public bool SetActiveTask(Task task) 
    {
        foreach (Task t in TaskList) 
        {
            if (task == t) 
            {
                ActiveTask = task;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Getter function for the TaskList
    /// </summary>
    /// <returns>Returns the Task List</returns>
    public List<Task> GetTaskList() 
    {
        return TaskList;
    }

    /// <summary>
    /// CompleteTask takes a task input and searches for it within the list of our tasks. If the task is found,
    /// the IsCompleted element is set to true and we return true as the function succeeded.
    /// <para>
    /// Completing a task means removing it from the task list and placing in the Queue of completed tasks
    /// </para>
    /// </summary>
    /// <param name="task">The task which must be within the task list</param>
    /// <returns>True if task was found and set to complete, false if it wasn't</returns>
    public bool CompleteTask(Task task) 
    {
        foreach (Task t in TaskList) 
        {
            if(task == t) 
            {
                task.IsCompleted = true;
                CompletedTasks.Enqueue(task);
                TaskList.Remove(task);

                //  Set Active task to next avaiable task
                if(TaskList[0] != null && ActiveTask == task) ActiveTask = TaskList[0];

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// GetCompletedTask dequeues a task from the Completed task list.
    /// </summary>
    /// <returns>A completed task</returns>
    public Task GetCompletedTask() 
    {
        return CompletedTasks.Dequeue();
    }
    /// <summary>
    /// PeekCompletedTask peeks at the most recently completed task
    /// </summary>
    /// <returns>A completed task</returns>
    public Task PeekCompletedTask()
    {
        return CompletedTasks.Peek();
    }

    /// <summary>
    /// Creates a Custom Task and adds it to the Task List, in order for this to work, the npc_name OR location,
    /// AND subject need to be within the {language} JSON File. Additionally, the task type needs to be within the
    /// {language}Tasks JSON file. Otherwise this will fail, and no task will be added.
    /// <para>
    /// If npc_name is not set to null, it will be considered the most relevant element of the task. If you want
    /// the task to be location based, set npc_name to null.
    /// </para>
    /// </summary>
    /// <param name="npc_name">The name of the Corresponding NPC, set to null if not relevant. Must be within the {language} JSON File</param>
    /// <param name="location">The name of the Corresponding NPC, set to null if not relevant. Must be within the {language} JSON File</param>
    /// <param name="subject">The Subject of the task. Must be within the {language} JSON File</param>
    /// <param name="type">The Type of the task. Must be within the {language}Tasks JSON File</param>
    /// <param name="difficulty">The Difficulty of the task</param>
    /// <returns>True if succesful, false if not</returns>
    public bool CreateCustomTask(string npc_name, string location, string subject, string type, int difficulty) 
    {
        string description = null, answer = null;
        string t_npc_name = null, t_location = null, t_subject = null, t_description = null, t_answer = null;

        var taskDetails = GetTaskTemplates(type, "English");
        var t_taskDetails = GetTaskTemplates(type, LocalizationManager.Instance.learningLanguage);

        //  If either of these were null, we exit
        if (taskDetails == null || t_taskDetails == null) { Debug.Log("taskDetails OR t_taskDetails returned NULL"); return false; }

        //  Translate the subject
        t_subject = LocalizationManager.Instance.localizedLearningText.ContainsKey(subject) ? LocalizationManager.Instance.localizedLearningText[subject] : null;

        if (t_subject == null) { Debug.Log($"KEY: {subject} could not be translated"); return false; }

        if (npc_name == null) //   taskLocation is relevant?
        {
            description = taskDetails.description.Replace("{subject}", subject).Replace("{location}", location);
            answer = taskDetails.answer.Replace("{subject}", subject).Replace("{location}", location);

            t_location = LocalizationManager.Instance.localizedLearningText.ContainsKey(location) ? LocalizationManager.Instance.localizedLearningText[location] : null;
            if (t_location == null) { Debug.Log($"KEY: {location} could not be translated"); return false; }

            t_description = t_taskDetails.description.Replace("{subject}", t_subject).Replace("{location}", t_location);
            t_answer = t_taskDetails.answer.Replace("{subject}", t_subject).Replace("{location}", t_location);
        }
        else    //  taskNPC is relevant?
        {
            description = taskDetails.description.Replace("{npc}", npc_name).Replace("{subject}", subject);
            answer = taskDetails.answer.Replace("{npc}", npc_name).Replace("{subject}", subject);

            t_npc_name = LocalizationManager.Instance.localizedLearningText.ContainsKey(npc_name) ? LocalizationManager.Instance.localizedLearningText[npc_name] : null;
            if (t_npc_name == null) { Debug.Log($"KEY: {npc_name} could not be translated"); return false; }

            t_description = t_taskDetails.description.Replace("{subject}", t_subject).Replace("{name}", t_npc_name);
            t_answer = t_taskDetails.answer.Replace("{subject}", t_subject).Replace("{name}", t_npc_name);
        }


        TaskList.Add(new Task
        {
            TaskDifficulty = difficulty,
            TaskDescription = description,
            TaskSubject = subject,
            TaskNPC = npc_name,
            TaskLocation = location,
            TaskAnswer = answer,

            T_TaskDescription = t_description,
            T_TaskSubject = t_subject,
            T_TaskNPC = t_npc_name,
            T_TaskLocation = t_location,
            T_TaskAnswer = t_answer,
            IsCompleted = false,
            IsCustom = true
        });
        return true;
    }

    /// <summary>
    /// Checks to see if the subject name or location is within the list of managed tasks.
    /// Takes two parameters, name and location, and searches through the TaskList for matching tasks
    /// <para>NOTE: This returns the FIRST task found matching the parameters.</para>
    /// </summary>
    /// <param name="name">Name of the NPC being spoken to</param>
    /// <param name="location">Location of NPC being spoken to</param>
    /// <returns>first task found matching name or location</returns>
    public Task SubjectTask(string npc_name, string location) 
    {
        foreach (Task task in TaskList) 
        {
            if (task?.TaskNPC == (npc_name ?? "") || task?.TaskLocation == (location ?? "")) 
            {
                return task;
            }
        }
        return null;
    }

    /// <summary>
    /// GenerateTasks will generate new tasks based on the count paramater. Each task will be marked as NOT custom
    /// </summary>
    /// <param name="count">The number of tasks to be generated</param>
    public void GenerateTasks(int count) 
    {
        if (count <= 0) return;

        for (int i = 0; i < count; i++) 
        {
            Task task = GenerateTask(1);
            if (task == null) { Debug.Log("Failed to generate Task"); continue; }

            TaskList.Add(task);
            //TaskList[i].printData();  //  Debug for the generated Task
        }
        ActiveTask = TaskList[0];
    }

    /// <summary>
    /// Removes all uncompleted NON-CUSTOM tasks from the task list
    /// </summary>
    /// <returns>Returns the number of items removed</returns>
    public int ClearUncompletedTasks() 
    {
        if (!(ActiveTask.IsCustom)) ActiveTask = null;
        return TaskList.RemoveAll(t => !(t.IsCustom));
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

        //Flip coin, if it lands on 0, generate Task
        if (Random.Range(0,2) == 0)
        {
            taskNPC = GetRandomNPC().ToString();

            //  Generate relevant subject and type
            taskSubject = GetSubject(taskNPC);
            taskType = GetTaskType(taskNPC);
        }
        else 
        {
            taskLocation = GetRandomLocation().ToString();

            //  Generate relevant subject and type
            taskSubject = GetSubject(taskLocation);
            taskType = GetTaskType(taskLocation);
        }

        //Generate LCLZ class with relevant descriptions and answers
        var taskDetails = GetTaskTemplates(taskType, "English");
        if (taskDetails == null) { Debug.Log($"KEY: {taskType} could not be translated from English"); return null; }

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

        t_taskSubject = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskSubject) ? LocalizationManager.Instance.localizedLearningText[taskSubject] : null;
        if (t_taskSubject == null) { Debug.Log($"KEY: {t_taskSubject} could not be translated from {LocalizationManager.Instance.learningLanguage}"); return null; }

        //Debug.Log($"Displaying data {taskNPC},\n{taskLocation},\n{taskSubject},\n{taskType},\n{taskDescription},\n{taskAnswer}");

        var t_taskDetails = GetTaskTemplates(taskType, LocalizationManager.Instance.learningLanguage);
        if (t_taskDetails == null) { Debug.Log($"KEY: {taskType} could not be translated"); return null; }

        if (taskNPC == null)    //   taskLocation is relevant?
        {
            t_taskLocation = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskLocation) ? LocalizationManager.Instance.localizedLearningText[taskLocation] : null;
            if (t_taskLocation == null) { Debug.Log($"KEY: {t_taskLocation} could not be translated"); return null; }

            t_taskDescription = t_taskDetails.description.Replace("{subject}", t_taskSubject).Replace("{location}", t_taskLocation);
            t_taskAnswer = t_taskDetails.answer.Replace("{subject}", t_taskSubject).Replace("{location}", t_taskLocation);
        }
        else    //  taskNPC is relevant?
        {
            t_taskNPC = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskNPC) ? LocalizationManager.Instance.localizedLearningText[taskNPC] : null;
            if (t_taskNPC == null) { Debug.Log($"KEY: {t_taskNPC} could not be translated"); return null; }

            t_taskDescription = t_taskDetails.description.Replace("{npc}", t_taskNPC).Replace("{subject}", t_taskSubject);
            t_taskAnswer = t_taskDetails.answer.Replace("{npc}", t_taskNPC).Replace("{subject}", t_taskSubject);
        }

        return new Task
        {
            TaskDifficulty = skill,
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
            IsCompleted = false,
            IsCustom = false
        };
    }

    /// <summary>
    /// Generates random npc based on available npcs defined in TaskNPCs
    /// </summary>
    /// <returns>Enum of NPC</returns>
    private TaskNPCs GetRandomNPC() 
    {
        return (TaskNPCs)Random.Range(0, System.Enum.GetNames(typeof(TaskNPCs)).Length);
    }

    /// <summary>
    /// Generates random location based on available locations defined in TaskLocations
    /// </summary>
    /// <returns>Enum of Location</returns>
    private TaskLocations GetRandomLocation()
    {
        return (TaskLocations)Random.Range(0, System.Enum.GetNames(typeof(TaskLocations)).Length);
    }

    /// <summary>
    /// GetSubject attempts to generate relevant subjects given the name of an NPC or location
    /// If no relevant subject can be found, subject generated will be more vague and applicable
    /// 
    /// </summary>
    /// <param name="context">The case-sensitive name of either an NPC or location</param>
    /// <returns>Returns random string of relevant subject</returns>
    private string GetSubject(string context)
    {
        //Creates a default subject list that will be returned if the key cannot be found
        List<string> defaultSubjects = new List<string> { "General" };

        return Subjects.ContainsKey(context) ? Subjects[context][Random.Range(0, Subjects[context].Count)] : defaultSubjects[Random.Range(0, defaultSubjects.Count)];
    }

    /// <summary>
    /// GetTaskTypeForLocation attempts to generate relevant task type given the name of a Location or NPC
    /// If no relevant subject can be found, task type generated will be more vague
    /// 
    /// </summary>
    /// <param name="context">The case-sensitive name of either an NPC or location</param>
    /// <returns>Returns random string of relevant task type</returns>
    private string GetTaskType(string context)
    {
        //Creates a default task list that will be returned if the key cannot be found
        List<string> defaultTasks = new List<string> { "General" };

        return Types.ContainsKey(context) ? Types[context][Random.Range(0, Types[context].Count)] : defaultTasks[Random.Range(0, defaultTasks.Count)];
    }

    /// <summary>
    /// Attempts to return the value at the location
    /// </summary>
    /// <param name="taskType">The Task type of the generated task</param>
    /// <param name="language">The desired translation language</param>
    /// <returns>
    /// Returns a LCLZ_Value class whos attributes contain the description and answer format for our task if the task exists
    /// within the JSON {language}Tasks file. If not, returns NULL
    /// </returns>
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
