using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


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

    private Task ActiveTask;    //  Our currently tracked task

    private Dictionary<string, List<string>> Types;
    private Dictionary<string, Dictionary<string, List<string>>> NPCSubjects;  //  Subject for NPC will be dependant on types
    private Dictionary<string, Dictionary<string, List<string>>> LocationSubjects;  //  Subject for NPC will be dependant on types



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
        float wait_time = 0f;
        while (!(GameManager.Instance && LocalizationManager.Instance && LocalizationManager.Instance.localizedLearningText != null)) 
        {
            yield return new WaitForSeconds(0.1f);
            wait_time += 0.1f;
            if (wait_time > 3.0f) 
            {
                Debug.LogError("ERR: Task Manager Initialization exceeded 3 seconds. Logging Info Below");
                DumpInfo();
                yield break;
            }
        }

        
    }

    
    /// <summary>
    /// Dumps Asserts on why loading failed
    /// </summary>
    private void DumpInfo() 
    {
        Debug.Assert(GameManager.Instance, "GameManager failed to set Instance");
        Debug.Assert(LocalizationManager.Instance, "Localization Manager failed to set Instance");
        Debug.Assert(LocalizationManager.Instance.localizedLearningText != null, "Localization Manager failed to initialize the localizedLearningText Dictionary");
    }


    /// <summary>
    /// Initiates the dictionaries that will be used for generating Tasks.
    /// Important Dictionaries: Subjects and Types can be created and filled here
    /// <para>Task Keys should be added to their respective JSON file for accurate translation</para>
    /// <para>Task Values should be added to a JSON file with their description and answers</para>
    /// <para>Subject Values should also be added to their JSON file</para>
    /// </summary>
    private void CreateTaskDictionaries() 
    {
        // Add new types to this section
        Types = new Dictionary<string, List<string>>();

        // NPC TASK TYPES
        Types["Baker"] = new List<string> { "Buy" };
        Types["Barista"] = new List<string> { "Buy" };

        // LOCATION TASK TYPES
        Types["Bakery"] = new List<string> { "AskLoc" };
        Types["Cafe"] = new List<string> { "AskLoc" };
        Types["Restaurant"] = new List<string> { "AskLoc" };

        // Add new NPC subjects to this section
        NPCSubjects = new Dictionary<string, Dictionary<string, List<string>>>();

        // Initialize NPC subjects with their nested dictionaries
        NPCSubjects["Baker"] = new Dictionary<string, List<string>>
        {
            { "Buy", new List<string> { "Bread", "Pastry" } },
            { "AskNPC", new List<string> { "Bread", "Pastry" } }
        };
        NPCSubjects["Barista"] = new Dictionary<string, List<string>>
        {
            { "Buy", new List<string> { "Coffee", "Tea" } }
        };

        // Add new Location subjects to this section
        LocationSubjects = new Dictionary<string, Dictionary<string, List<string>>>();

        // Initialize Location subjects with their nested dictionaries
        LocationSubjects["Bakery"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Bread", "Pastry" } }
        };
        LocationSubjects["Cafe"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Coffee", "Tea" } }
        };
        LocationSubjects["Restaurant"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Reservation", "Menu" } }
        };
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
            taskType = GetTaskType(taskNPC);
            taskSubject = GetSubject(NPCSubjects[taskNPC], taskType);
        }
        else 
        {
            taskLocation = GetRandomLocation().ToString();

            //  Generate relevant subject and type
            taskType = GetTaskType(taskLocation);
            taskSubject = GetSubject(LocationSubjects[taskLocation], taskType);
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
    private string GetRandomNPC() 
    {
        List<string> list = new List<string>(NPCSubjects.Keys);
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Generates random location based on available locations defined in TaskLocations
    /// </summary>
    /// <returns>Enum of Location</returns>
    private string GetRandomLocation()
    {
        List<string> list = new List<string>(LocationSubjects.Keys);
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// GetSubject attempts to generate relevant subjects given the name of an NPC or location
    /// If no relevant subject can be found, subject generated will be more vague and applicable
    /// 
    /// </summary>
    /// <param name="SubjectDict">The dictionary either pertaining to a NPC or Location</param>
    /// <param name="type">The case-sensitive type of a task</param>
    /// <returns>Returns random string of relevant subject</returns>
    private string GetSubject(Dictionary<string, List<string>> SubjectDict, string type)
    {
        return SubjectDict.ContainsKey(type) ? SubjectDict[type][Random.Range(0, SubjectDict[type].Count)] : null;
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
        return Types.ContainsKey(context) ? Types[context][Random.Range(0, Types[context].Count)] : null;
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
            if (task == t)
            {
                task.IsCompleted = true;
                CompletedTasks.Enqueue(task);
                TaskList.Remove(task);

                //  Set Active task to next avaiable task
                if (TaskList.Count > 0 && ActiveTask == task) ActiveTask = TaskList[0];
                else ActiveTask = null;

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
        if (ActiveTask != null && !(ActiveTask.IsCustom)) ActiveTask = null;
        return TaskList.RemoveAll(t => !(t.IsCustom));
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
    /// Tests each of the functions in the task manager out
    /// </summary>
    public void TestTaskManager() 
    {
        Debug.Log("Starting massive test");
        // Test 1: Add a Custom Task
        bool createResult = CreateCustomTask("Baker", null, "Bread", "AskNPC", 2);
        Debug.Log("Adding Custom Task: " + $"{createResult}");
        Debug.Assert(createResult == true, "CreateCustomTask failed.");
        Debug.Assert(GetTaskList().Count == 1, "Task list should have 1 task after creation.");

        // Test 2: Generate a Random Task
        Task randomTask = GenerateTask(1);
        Debug.Log("Generating random task");
        randomTask.printData();
        Debug.Assert(randomTask != null, "GenerateTask should return a valid task.");

        // Test 3: Complete a Task
        Task taskToComplete = GetTaskList()[0];
        bool completeResult = CompleteTask(taskToComplete);
        Debug.Log("Completing task: " + $"{completeResult}");
        Debug.Log("Completed task");
        taskToComplete.printData();
        Debug.Assert(completeResult == true, "CompleteTask failed.");
        Debug.Assert(GetTaskList().Count == 0, "Task list should be empty after completion.");
        Debug.Assert(CompletedTasks.Count == 1, "CompletedTasks should contain the completed task.");

        // Test 4: Clear Non-Custom Uncompleted Tasks
        Debug.Log("Adding Non-Custom and Custom Task");
        GetTaskList().Add(new Task { IsCustom = false });
        GetTaskList().Add(new Task { IsCustom = true });
        
        int clearedCount = ClearUncompletedTasks();
        Debug.Log("Clearing all Non-Custom Tasks: " + $"{clearedCount}");
        Debug.Assert(clearedCount == 1, "ClearUncompletedTasks should remove non-custom tasks.");
        Debug.Assert(GetTaskList().Count == 1, "Task list should have 1 task remaining.");
        Debug.Assert(GetTaskList()[0].IsCustom == true, "Custom task was incorrectly removed.");

        // Clearing out TaskList and completed tasks
        TaskList.Clear();
        CompletedTasks.Clear();

        // Test 5: Get Active Task
        Debug.Log("Creating new Task");
        Task newTask = new Task { TaskNPC = "Barista", TaskSubject = "Coffee" };
        newTask.printData();
        GetTaskList().Add(newTask);
        SetActiveTask(newTask);
        Task retrievedActiveTask = GetActiveTask();
        Debug.Log("Setting newTask as Active task, printing active task");
        retrievedActiveTask.printData();
        Debug.Assert(retrievedActiveTask == newTask, "GetActiveTask returned the wrong task.");

        // Test 6: Active Task auto fills to next available task
        Debug.Log("Testing Active Task Autofill");
        Task newerTask = GenerateTask(1);
        GetTaskList().Add(newerTask);
        CompleteTask(GetTaskList()[0]);
        Debug.Log("Creating new task, Completing old task");
        GetActiveTask().printData();
        Debug.Assert(newerTask == GetActiveTask(), "GetTaskList()[0] returned the wrong task.");

        // Test 7: Active Task sets itself to null when no available task
        Debug.Log("Testing Active Task autofill to null");
        CompleteTask(GetTaskList()[0]);
        Debug.Assert(GetActiveTask() == null, "GetActiveTask() returned something when it should have been null.");
        Debug.Log("Massive Test Passed!");
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
