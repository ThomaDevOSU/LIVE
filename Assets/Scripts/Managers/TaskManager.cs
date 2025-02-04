using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using System;
using Random = UnityEngine.Random;


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

    /// <summary>
    /// The Types dictionary is a dictionary of possible task types that a given npc/location can be assigned
    /// </summary>
    private Dictionary<int, Dictionary<string, List<string>>> Types;

    private Dictionary<string, Dictionary<string, List<string>>> NPCSubjects;  //  Subject for NPC will be dependant on types
    private Dictionary<string, Dictionary<string, List<string>>> LocationSubjects;  //  Subject for NPC will be dependant on types

    /// <summary>
    /// The following dictionary is loaded from our templatization file into memory. Given a difficulty level {1: Easy, 2: Medium, 3: Hard}
    /// It links to a new Dictionary whose string corresponds to a task type, and value leads to a Class of description/answer pairs.
    /// </summary>
    private Dictionary<int, Dictionary<string, TaskTemplate>> TaskTemplateDictLearning;

    /// <summary>
    /// This dictionary behaves the same as the top, but is in english for GPT to process.
    /// </summary>
    private Dictionary<int, Dictionary<string, TaskTemplate>> TaskTemplateDictEnglish;

    /// <summary>
    /// The completeTask unityEvent is invoked whenever a task is completed
    /// </summary>
    private UnityEvent<Task> completeTask = new UnityEvent<Task>();

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

        //TestTaskManager();
        // GENERATES 5 TASKS from the Level 3 Task pool, DISABLE THIS IF YOU WANT AN EMPTY TASK LIST
        GenerateTasks(5 , GameManager.Instance.CurrentPlayerData.preferredDifficulty);
        //PrintTaskList();
        
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
        Types = new Dictionary<int, Dictionary<string, List<string>>>();
        // Difficulty 1 Task Types. A subject is given a list of easy tasks types.
        Types[1] = new Dictionary<string, List<string>>();

        // NPC TASK TYPES
        Types[1]["Baker"] = new List<string> { "Buy" };
        Types[1]["Barista"] = new List<string> { "Buy" };
        Types[1]["Chef"] = new List<string> { "OrderMeal" };
        Types[1]["Doctor"] = new List<string> { "SeekAdvice" };
        Types[1]["Waitress"] = new List<string> { "OrderMeal" };
        Types[1]["Mayor"] = new List<string> { "DiscussPolicy" };
        Types[1]["Pharmacist"] = new List<string> { "Buy" };
        Types[1]["Sheriff"] = new List<string> { "ReportIssue" };

        // LOCATION TASK TYPES
        Types[1]["Bakery"] = new List<string> { "AskLoc" };
        Types[1]["Cafe"] = new List<string> { "AskLoc" };
        Types[1]["Restaurant"] = new List<string> { "AskLoc" };

        // Difficulty 2 Task Types. Same subjects as before, with identical task type lists but added medium difficulty task types.
        Types[2] = new Dictionary<string, List<string>>();

        // NPC TASK TYPES
        Types[2]["Baker"] = new List<string> { "Buy", "DiscussRecipes" };
        Types[2]["Barista"] = new List<string> { "Buy", "RecommendDrink" };
        Types[2]["Chef"] = new List<string> { "OrderMeal", "DiscussRecipes" };
        Types[2]["Doctor"] = new List<string> { "SeekAdvice", "RequestMedicine" };
        Types[2]["Waitress"] = new List<string> { "OrderMeal", "RequestRefill" };
        Types[2]["Mayor"] = new List<string> { "DiscussPolicy", "SeekApproval" };
        Types[2]["Pharmacist"] = new List<string> { "Buy", "SeekAdvice" };
        Types[2]["Sheriff"] = new List<string> { "ReportIssue", "AskForHelp" };

        // LOCATION TASK TYPES
        Types[2]["Bakery"] = new List<string> { "AskLoc", "DiscussMenu" };
        Types[2]["Cafe"] = new List<string> { "AskLoc", "DiscussMenu" };
        Types[2]["Restaurant"] = new List<string> { "AskLoc", "DiscussMenu" };

        // Difficulty Type 3, Hard, should have same as previous with some hard additions
        Types[3] = new Dictionary<string, List<string>>();

        // NPC TASK TYPES
        Types[3]["Baker"] = new List<string> { "Buy", "DiscussRecipes", "NegotiatePrices" };
        Types[3]["Barista"] = new List<string> { "Buy", "RecommendDrink", "ExplainIngredients" };
        Types[3]["Chef"] = new List<string> { "OrderMeal", "DiscussRecipes", "RateDishes" };
        Types[3]["Doctor"] = new List<string> { "SeekAdvice", "RequestMedicine", "DiscussHealth" };
        Types[3]["Waitress"] = new List<string> { "OrderMeal", "RequestRefill", "AskSpecials" };
        Types[3]["Mayor"] = new List<string> { "DiscussPolicy", "SeekApproval", "DebateIssues" };
        Types[3]["Pharmacist"] = new List<string> { "Buy", "SeekAdvice", "AskAvailability" };
        Types[3]["Sheriff"] = new List<string> { "ReportIssue", "AskForHelp", "SeekAssistance" };

        // LOCATION TASK TYPES
        Types[3]["Bakery"] = new List<string> { "AskLoc", "DiscussMenu", "PlanEvent" };
        Types[3]["Cafe"] = new List<string> { "AskLoc", "DiscussMenu", "PlanEvent" };
        Types[3]["Restaurant"] = new List<string> { "AskLoc", "DiscussMenu", "PlanEvent" };

        // Add new NPC subjects to this section
        NPCSubjects = new Dictionary<string, Dictionary<string, List<string>>>();

        // Initialize NPC subjects with their nested dictionaries
        NPCSubjects["Baker"] = new Dictionary<string, List<string>>
        {
            { "Buy", new List<string> { "Bread", "Pastry" } },
            { "DiscussRecipes", new List<string> { "Sourdough", "Croissant" } },
            { "NegotiatePrices", new List<string> { "Discounts", "Bulk Orders" } }
        };

        NPCSubjects["Barista"] = new Dictionary<string, List<string>>
        {
            { "Buy", new List<string> { "Coffee", "Tea" } },
            { "RecommendDrink", new List<string> { "Latte", "Espresso" } },
            { "ExplainIngredients", new List<string> { "Espresso Beans", "Milk Alternatives" } }
        };

        NPCSubjects["Chef"] = new Dictionary<string, List<string>>
        {
            { "OrderMeal", new List<string> { "Pasta", "Steak" } },
            { "DiscussRecipes", new List<string> { "Soup", "Sauce" } },
            { "RateDishes", new List<string> { "Flavor", "Presentation" } }
        };

        NPCSubjects["Doctor"] = new Dictionary<string, List<string>>
        {
            { "SeekAdvice", new List<string> { "Headache", "Flu" } },
            { "RequestMedicine", new List<string> { "Painkillers", "Antibiotics" } },
            { "DiscussHealth", new List<string> { "Diet", "Exercise" } }
        };

        NPCSubjects["Waitress"] = new Dictionary<string, List<string>>
        {
            { "OrderMeal", new List<string> { "Soup", "Burger" } },
            { "RequestRefill", new List<string> { "Water", "Soda" } },
            { "AskSpecials", new List<string> { "Dessert", "Seasonal Menu" } }
        };

        NPCSubjects["Mayor"] = new Dictionary<string, List<string>>
        {
            { "DiscussPolicy", new List<string> { "Taxes", "Education" } },
            { "SeekApproval", new List<string> { "Event", "New Business" } },
            { "DebateIssues", new List<string> { "Crime", "Infrastructure" } }
        };

        NPCSubjects["Pharmacist"] = new Dictionary<string, List<string>>
        {
            { "Buy", new List<string> { "Painkillers", "Vitamins" } },
            { "SeekAdvice", new List<string> { "Side Effects", "Dosages" } },
            { "AskAvailability", new List<string> { "Prescriptions", "Supplements" } }
        };

        NPCSubjects["Sheriff"] = new Dictionary<string, List<string>>
        {
            { "ReportIssue", new List<string> { "Lost Item", "Neighborhood Problem" } },
            { "AskForHelp", new List<string> { "Directions", "Community Event" } },
            { "SeekAssistance", new List<string> { "Safety Tips", "Local Laws" } }
        };

        // Add new Location subjects to this section
        LocationSubjects = new Dictionary<string, Dictionary<string, List<string>>>();

        // Initialize Location subjects with their nested dictionaries
        LocationSubjects["Bakery"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Bread", "Pastry" } },
            { "DiscussMenu", new List<string> { "Seasonal Specials", "Custom Orders" } },
            { "PlanEvent", new List<string> { "Tasting Session", "Cooking Class" } }
        };
        LocationSubjects["Cafe"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Coffee", "Tea" } },
            { "DiscussMenu", new List<string> { "Daily Brews", "Signature Drinks" } },
            { "PlanEvent", new List<string> { "Live Music Night", "Coffee Tasting" } }
        };
        LocationSubjects["Restaurant"] = new Dictionary<string, List<string>>
        {
            { "AskLoc", new List<string> { "Reservation", "Menu" } },
            { "DiscussMenu", new List<string> { "Chef's Specials", "Wine Pairings" } },
            { "PlanEvent", new List<string> { "Private Dining", "Corporate Lunch" } }
        };

        LoadTemplates();

    }

    /// <summary>
    /// Load Templates will load all known task templates into both the
    /// TaskTemplateDictLearning and TaskTemplateDictEnglish dictionaries.
    /// These dictionaries can then be used for dynamic task generation
    /// </summary>
    private void LoadTemplates() 
    {
        TaskTemplateDictLearning = new Dictionary<int, Dictionary<string, TaskTemplate>>();
        TaskTemplateDictEnglish = new Dictionary<int, Dictionary<string, TaskTemplate>>();

        JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };

        string learningpath = Path.Combine(Application.streamingAssetsPath, $"Localization/{LocalizationManager.Instance.learningLanguage}/");
        string englishpath = Path.Combine(Application.streamingAssetsPath, $"Localization/en/");

        //  Loads from the three difficulties
        for (int i = 1; i < 4; i++) 
        {
            using (StreamReader file = File.OpenText(learningpath + $"task{i}.json"))
            {
                TaskTemplateDictLearning[i] = (Dictionary<string, TaskTemplate>)serializer.Deserialize(file, typeof(Dictionary<string, TaskTemplate>));
            }

            using (StreamReader file = File.OpenText(englishpath + $"task{i}.json"))
            {
                TaskTemplateDictEnglish[i] = (Dictionary<string, TaskTemplate>)serializer.Deserialize(file, typeof(Dictionary<string, TaskTemplate>));
            }
        }

    }


    /// <summary>
    /// The GenerateTask function first rolls a two sided dice to decide whether the task will be based
    /// on a Location on the map, or the name of an NPC. Then it will generate a random conversational task
    /// relevant to the NPC/Location {IE. Buy bread from Baker, ask about weather at Park }
    /// String data must be in a format that is easy to translate
    /// </summary>
    /// <param name="skill">The calculate skill level</param>
    /// <param name="taskNPC">Typically null, allows for generating specfic content</param>
    /// <param name="taskLocation">Typically null, allows for generating specfic content</param>
    /// <returns>Generates a task, filled with</returns>
    private Task GenerateTask(int skill, string taskNPC = null, string taskLocation = null) 
    {
        //  Relevant task strings that will be assigned
        string taskDescription = null, taskSubject = null, taskAnswer = null;

        //  Relevant task type, that does not get saved to the task itself
        string taskType = null;

        //Flip coin, if it lands on 0, generate Task
        if (taskNPC == null && taskLocation == null) //  Do this only if the taskNPC and taskLoc has not been assigned
        {
            if (Random.Range(0, 2) == 0)
            {
                taskNPC = GetRandomNPC().ToString();
            }
            else
            {
                taskLocation = GetRandomLocation().ToString();
            }
        }

        if (taskNPC != null)    //  taskNPC was assigned
        {
            //  Generate relevant subject and type
            taskType = GetTaskType(taskNPC, skill);
            taskSubject = GetSubject(NPCSubjects[taskNPC], taskType);
        }
        else    //  taskLocation was assigned
        {
            //  Generate relevant subject and type
            taskType = GetTaskType(taskLocation, skill);
            taskSubject = GetSubject(LocationSubjects[taskLocation], taskType);
        }

        //Generate LCLZ class with relevant descriptions and answers
        var taskDetails = GetTaskTemplates(taskType, skill, true);
        if (taskDetails == null) { Debug.Log($"KEY: {taskType} could not be translated from English"); return null; }

        //Debug.Log($"Printing variables, {taskNPC}\n{taskLocation}\n{taskSubject}\n{taskType}\n{taskDetails}");

        //Assign values, replace strings with proper context
        if (taskNPC==null) //   taskLocation is relevant?
        {
            taskDescription = taskDetails.description.Replace("{{subject}}", taskSubject).Replace("{{location}}", taskLocation);
            taskAnswer = taskDetails.answer.Replace("{{subject}}", taskSubject).Replace("{{location}}", taskLocation);
        } 
        else    //  taskNPC is relevant?
        {
            taskDescription = taskDetails.description.Replace("{{npc}}", taskNPC).Replace("{{subject}}", taskSubject);
            taskAnswer = taskDetails.answer.Replace("{{npc}}", taskNPC).Replace("{{subject}}", taskSubject);
        }

        //  This section is dedicated to the translatable section, this requires that the localizer be loaded
        string t_taskDescription = null, t_taskNPC = null, t_taskLocation = null, t_taskSubject = null, t_taskAnswer = null;

        t_taskSubject = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskSubject) ? LocalizationManager.Instance.localizedLearningText[taskSubject] : null;
        if (t_taskSubject == null) { Debug.Log($"KEY: {taskSubject} could not be translated from {LocalizationManager.Instance.learningLanguage}"); return null; }

        //Debug.Log($"Displaying data {taskNPC},\n{taskLocation},\n{taskSubject},\n{taskType},\n{taskDescription},\n{taskAnswer}");

        var t_taskDetails = GetTaskTemplates(taskType, skill);
        if (t_taskDetails == null) { Debug.Log($"KEY: {taskType} could not be translated"); return null; }

        if (taskNPC == null)    //   taskLocation is relevant?
        {
            t_taskLocation = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskLocation) ? LocalizationManager.Instance.localizedLearningText[taskLocation] : null;
            if (t_taskLocation == null) { Debug.Log($"KEY: {t_taskLocation} could not be translated"); return null; }

            t_taskDescription = t_taskDetails.description.Replace("{{subject}}", t_taskSubject).Replace("{{location}}", t_taskLocation);
            t_taskAnswer = t_taskDetails.answer.Replace("{{subject}}", t_taskSubject).Replace("{{location}}", t_taskLocation);
        }
        else    //  taskNPC is relevant?
        {
            t_taskNPC = LocalizationManager.Instance.localizedLearningText.ContainsKey(taskNPC) ? LocalizationManager.Instance.localizedLearningText[taskNPC] : null;
            if (t_taskNPC == null) { Debug.Log($"KEY: {t_taskNPC} could not be translated"); return null; }

            t_taskDescription = t_taskDetails.description.Replace("{{npc}}", t_taskNPC).Replace("{{subject}}", t_taskSubject);
            t_taskAnswer = t_taskDetails.answer.Replace("{{npc}}", t_taskNPC).Replace("{{subject}}", t_taskSubject);
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
    private string GetTaskType(string context, int difficulty)
    {
        return Types[difficulty].ContainsKey(context) ? Types[difficulty][context][Random.Range(0, Types[difficulty][context].Count)] : null;
    }

    /// <summary>
    /// Attempts to return the value at the location
    /// </summary>
    /// <param name="taskType">The Task type of the generated task</param>
    /// <param name="difficulty">The desired difficulty of the template</param>
    /// <param name="gptver">Whether this is the version for gpt or not</param>
    /// <returns>
    /// Returns a LCLZ_Value class whos attributes contain the description and answer format for our task if the task exists
    /// within the JSON {language}Tasks file. If not, returns NULL
    /// </returns>
    private TaskTemplate GetTaskTemplates(string taskType, int difficulty, bool gptver = false)
    {
        //  if gptver, we search the english dictionary
        if (gptver)
        {
            return TaskTemplateDictEnglish[difficulty].ContainsKey(taskType) ? TaskTemplateDictEnglish[difficulty][taskType] : null;
        }
        else // Otherwise we search the learning dictionary
        {
            return TaskTemplateDictLearning[difficulty].ContainsKey(taskType) ? TaskTemplateDictLearning[difficulty][taskType] : null;
        }
    }


    /// <summary>
    /// Original Task searches through the list of tasks and attempts to generate one
    /// if one cannot be generated, null is returned
    /// </summary>
    /// <param name="difficulty">The desired difficulty of a task</param>
    /// <returns></returns>
    private Task OriginalTask(int difficulty)
    {
        //  First we search through the list of all NPC possibilities. If one cannot be found for a specific subject, we return a task generated for that
        foreach (var npc in NPCSubjects) 
        {
            if (SubjectTask(npc.Key, null) == null) return GenerateTask(difficulty, taskNPC:npc.Key);
        }

        //  First we search through the list of all NPC possibilities. If one cannot be found for a specific subject, we return a task generated for that
        foreach (var loc in LocationSubjects)
        {
            if (SubjectTask(null, loc.Key) == null) return GenerateTask(difficulty, taskLocation:loc.Key);
        }

        return null;    //  We could not find an original task to make
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

                completeTask?.Invoke(task);
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

        var taskDetails = GetTaskTemplates(type, difficulty, true);
        var t_taskDetails = GetTaskTemplates(type, difficulty);

        //  If either of these were null, we exit
        if (taskDetails == null || t_taskDetails == null) { Debug.Log("taskDetails OR t_taskDetails returned NULL"); return false; }

        //  Translate the subject
        t_subject = LocalizationManager.Instance.localizedLearningText.ContainsKey(subject) ? LocalizationManager.Instance.localizedLearningText[subject] : null;

        if (t_subject == null) { Debug.Log($"KEY: {subject} could not be translated"); return false; }

        if (npc_name == null) //   taskLocation is relevant?
        {
            description = taskDetails.description.Replace("{{subject}}", subject).Replace("{{location}}", location);
            answer = taskDetails.answer.Replace("{{subject}}", subject).Replace("{{location}}", location);

            t_location = LocalizationManager.Instance.localizedLearningText.ContainsKey(location) ? LocalizationManager.Instance.localizedLearningText[location] : null;
            if (t_location == null) { Debug.Log($"KEY: {location} could not be translated"); return false; }

            t_description = t_taskDetails.description.Replace("{{subject}}", t_subject).Replace("{{location}}", t_location);
            t_answer = t_taskDetails.answer.Replace("{{subject}}", t_subject).Replace("{{location}}", t_location);
        }
        else    //  taskNPC is relevant?
        {
            description = taskDetails.description.Replace("{{npc}}", npc_name).Replace("{{subject}}", subject);
            answer = taskDetails.answer.Replace("{{npc}}", npc_name).Replace("{{subject}}", subject);

            t_npc_name = LocalizationManager.Instance.localizedLearningText.ContainsKey(npc_name) ? LocalizationManager.Instance.localizedLearningText[npc_name] : null;
            if (t_npc_name == null) { Debug.Log($"KEY: {npc_name} could not be translated"); return false; }

            t_description = t_taskDetails.description.Replace("{subject}", t_subject).Replace("{{name}}", t_npc_name);
            t_answer = t_taskDetails.answer.Replace("{{subject}}", t_subject).Replace("{{name}}", t_npc_name);
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
            if (task.TaskNPC == (npc_name ?? "") || task.TaskLocation == (location ?? ""))
            {
                //Debug.Log($"Match found in SubjectTask\nTaskNPC :{task.TaskNPC ?? "NULL"}, NPC: {npc_name ?? "NULL"}, TaskLoc :{task.TaskLocation ?? "NULL"}, Loc: {location ?? "NULL"}");
                return task;
            }
        }
        return null;
    }

    /// <summary>
    /// GenerateTasks will generate new tasks based on the count paramater. Each task will be marked as NOT custom
    /// </summary>
    /// <param name="count">The number of tasks to be generated</param>
    public void GenerateTasks(int count, int difficulty)
    {
        if (count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            Task task = GenerateTask(difficulty);
            //task.printData();
            if (SubjectTask(task.TaskNPC, task.TaskLocation) != null) //  This task subject was already in the TaskList
            {
                //Debug.Log("New Task Generation attempt");
                task = OriginalTask(difficulty);
                //Debug.Log("New Task Generation completed");
                if (task!=null) task.printData();
            }  

            if (task == null) { Debug.LogError($"Failed to generate Task!\nSuccesfully generated {i} tasks!"); return; }
            
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
    /// Prints all current tasks in the TaskList to the console.
    /// Useful for debugging purposes.
    /// </summary>
    public void PrintAllTasksToConsole()
    {
        if (TaskList == null || TaskList.Count == 0)
        {
            Debug.Log("TaskManager: No active tasks in the list.");
            return;
        }

        Debug.Log($"TaskManager: Printing {TaskList.Count} active tasks...");

        foreach (Task task in TaskList)
        {
            Debug.Log($"Task: {task.TaskDescription} | Subject: {task.TaskSubject} | NPC: {task.TaskNPC} | Location: {task.TaskLocation} | Difficulty: {task.TaskDifficulty} | Completed: {task.IsCompleted}");
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
        // Adding listener for task completion
        AddTaskCompletionListener((Task t) => { Debug.Log($"Pinging completeTask event! Task Description: {t.TaskDescription}"); });
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

        // Clearing out TaskList and completed tasks
        TaskList.Clear();      
        CompletedTasks.Clear();

        // Test 8: Difficulty Scaling Generate 5 difficulty 2 tasks
        Debug.Log("TESTING LEVEL 2 GENERATION");
        GenerateTasks(5, 2);
        PrintTaskList();
        TaskList.Clear();

        // Test 9: Difficulty Scaling Generate 5 difficulty 3 tasks
        Debug.Log("TESTING LEVEL 3 GENERATION");
        GenerateTasks(5, 3);
        PrintTaskList();
        TaskList.Clear();

        Debug.Log("FINISHED TEST!");
    }


    /// <summary>
    /// AddTaskCompletionListener adds a listener
    /// </summary>
    /// <param name="action"></param>
    public void AddTaskCompletionListener(UnityAction<Task> action)
    {
        if (action == null) { Debug.LogError("ERROR: ACTION SENT TO COMPLETETASK EVENT NULL"); return; }
        completeTask.AddListener(action);
    }

}

/// <summary>
/// The task template 
/// </summary>
class TaskTemplate 
{
    /// <summary>
    /// The description of a possible task
    /// </summary>
    public string description;

    /// <summary>
    /// The templatized answer to a possible task
    /// </summary>
    public string answer;
}