using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all NPCs in the game, providing methods to add, remove, and retrieve NPCs.
/// </summary>
public class NPCManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the NPCManager.
    /// </summary>
    public static NPCManager Instance;

    /// <summary>
    /// List of all NPCs in the game.
    /// </summary>
    private static List<NPC> NPCs;

    /// <summary>
    /// Dictionary to store NPC conversation history.
    /// </summary>
    private Dictionary<string, List<Message>> ConversationHistory;

    /// <summary>
    /// Current Scene's name
    /// </summary>
    string currentSceneName;

    /// <summary>
    ///  Gameclock instance.
    /// </summary>
    GameClock gameClock;

    /// <summary>
    ///  Waypoint Manager instance.
    /// </summary>
    WaypointManager waypointManager;

    /// <summary>
    ///  GPT service instance.
    /// </summary>
    GPTService gptService;

    /// <summary>
    /// waypoint to move to.
    /// </summary>
    Transform waypoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            NPCs = new List<NPC>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameClock = GameClock.Instance;
        waypointManager = WaypointManager.Instance;
        gptService = GPTService.Instance;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        NPCs.Clear();
        currentSceneName = SceneManager.GetActiveScene().name;

        // Start a coroutine to delay PlaceNPCs()
        StartCoroutine(SetupNPCState()); // This also loads conversations
    }

    /// <summary>
    /// This delays calling PlaceNPCs until after the NPCs are instantiated but before Update() is called. Bit of a hack fix.
    /// </summary>
    private IEnumerator SetupNPCState()
    {
        // Wait until the next frame to ensure all Start() methods have run
        yield return null;

        if (NPCs.Count == 0)
        {
            //Debug.LogWarning("NPCs list is still empty after delaying PlaceNPCs().");
        }
        else
        {
            PlaceNPCs();
            LoadConversationHistory();
        }
    }

    /// <summary>
    /// Update location of all NPCs.
    /// </summary>
    void Update()
    {
        MoveNPCs();
    }

    /// <summary>
    /// Place all NPCs where they should be.
    /// </summary>
    private void PlaceNPCs()
    {
        foreach (NPC npc in NPCs)
        {
            foreach (ScheduleEntry entry in npc.Schedule)
            {
                if (entry != null && entry.time == Mathf.FloorToInt(gameClock.currentHour) && entry.location == currentSceneName)
                {
                    var success = npc.agent.Warp(waypointManager.GetWaypoint(entry.waypoint).position);

                    if (success == false)
                    {
                        Debug.LogWarning($"{npc.Name} failed to warp to correct position in {currentSceneName}.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Trigger Movement of all NPCs.
    /// </summary>
    private void MoveNPCs()
    {
        foreach (NPC npc in NPCs)
        {
            npc.UpdateSpeedandDirection();
            foreach (ScheduleEntry entry in npc.Schedule)
            {
                if (entry != null && entry.time == Mathf.FloorToInt(gameClock.currentHour) && currentSceneName == entry.location)
                {
                    // Get waypoint
                    waypoint = waypointManager.GetWaypoint(entry.waypoint);
                    // Logic for moving
                    npc.agent.SetDestination(waypoint.position);
                }
            }
        }
    }

    /// <summary>
    /// Adds an NPC to the list.
    /// </summary>
    /// <param name="npc">The NPC to add.</param>
    public void AddNPC(NPC npc)
    {
        NPCs.Add(npc);
        npc.messages = null;
        if (ConversationHistory == null)
        {
            ConversationHistory = new Dictionary<string, List<Message>>();
        }

        if (!ConversationHistory.ContainsKey(npc.Name))
        {
            ConversationHistory[npc.Name] = new List<Message>();
        }
        
        // NPCs add there own messages from conversation history. conversation history is set on game load (should at least)
        npc.messages = ConversationHistory[npc.Name];
    }

    /// <summary>
    /// Removes an NPC from the list by name.
    /// </summary>
    /// <param name="name">The name of the NPC to remove.</param>
    public void RemoveNPC(string name)
    {
        foreach (NPC npc in NPCs)
        {
            if (npc.Name == name)
            {
                NPCs.Remove(npc);
                return;
            }
        }
    }

    /// <summary>
    /// Retrieves an NPC by name or ID. Both name and ID are optional, but at least one must be provided.
    /// </summary>
    /// <param name="name">The name of the NPC to retrieve. Optional.</param>
    /// <param name="id">The ID of the NPC to retrieve. Optional.</param>
    /// <returns>The NPC with the specified name or ID, or null if not found.</returns>
    public NPC GetNPC(string name = null, int? id = null)
    {
        if (name == null && id == null)
        {
            Debug.LogError("At least one of name or id must be provided.");
        }

        if (NPCs == null || NPCs.Count == 0)
        {
            Debug.LogError("No NPCs found.");
            return null;
        }

        // Search for the NPC by name or ID. Ignores case when comparing names. Returns the first match.
        foreach (NPC npc in NPCs)
        {
            if ((name != null && string.Equals(npc.Name, name, System.StringComparison.OrdinalIgnoreCase) || (id != null && npc.ID == id)))
            {
                return npc;
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves the list of all NPCs.
    /// </summary>
    /// <returns>A list of all NPCs.</returns>
    public List<NPC> GetNPCs()
    {
        return NPCs;
    }

    /// <summary>
    /// Store the conversation history of all NPCs.
    /// </summary>
    public void StoreConversationHistory()
    {
        if (ConversationHistory == null)
        {
            ConversationHistory = new Dictionary<string, List<Message>>();
        }

        foreach (NPC npc in NPCs)
        {
            if (string.IsNullOrEmpty(npc.Name))
            {
                Debug.LogWarning("NPC name is empty. Skipping conversation history storage.");
                continue;
            }

            if (npc.messages == null)
            {
                Debug.LogWarning($"NPC {npc.Name} has no messages. Skipping.");
                continue;
            }
            // Uncomment this for debugging save, load, summary of dialogue.
            // PrintConversationHistory();
            // Update or add the conversation history
            ConversationHistory[npc.Name] = npc.messages;
        }
    }

    /// <summary>
    /// Load conversation history from ConversationHistory Dictionary. This is to be used between scenes as NPCs are loaded and unloaded.
    /// </summary>
    public void LoadConversationHistory()
    {
        LoadConversationHistoryFromPlayerData(GameManager.Instance.CurrentPlayerData);
        foreach (NPC npc in NPCs)
        {
            if (!ConversationHistory.ContainsKey(npc.Name))
            {
                continue;
            }
            npc.messages = ConversationHistory[npc.Name];
        }
    }

    /// <summary>
    /// Retrieve a dictionary of all NPC conversation history. This is only called before a Save.
    /// </summary>
    /// <returns>
    /// Dictionary with NPC names as keys and lists of conversation history as values.
    /// </returns>
    public Dictionary<string, List<Message>> GetConversationHistory()
    {
        // SummarizeConversationHistory();
        return ConversationHistory ??= new Dictionary<string, List<Message>>();
    }

    /// <summary>
    /// Load Conversation History from PlayerData.
    /// </summary>
    public void LoadConversationHistoryFromPlayerData(PlayerData data)
    {
        ConversationHistory.Clear();
        ConversationHistory = data.conversationHistory;
    }

    /// <summary>
    /// Retrieves the NPC currently in dialogue.
    /// </summary>
    /// <returns>The NPC currently in dialogue, or null if none are in dialogue.</returns>
    public NPC GetCurrentDialogueNPC()
    {
        foreach (NPC npc in NPCs)
        {
            if (npc.inDialogue)
            {
                return npc;
            }
        }
        return null;
    }

    /// <summary>
    /// Prints all keys and their corresponding messages in the ConversationHistory dictionary.
    /// </summary>
    public void PrintConversationHistory()
    {
        if (ConversationHistory == null || ConversationHistory.Count == 0)
        {
            Debug.LogWarning("ConversationHistory is empty or null.");
            return;
        }

        foreach (var entry in ConversationHistory)
        {
            Debug.Log($"NPC Name: {entry.Key} has {entry.Value.Count()} messages");
            foreach (var message in entry.Value)
            {
                Debug.Log($"Message Role: {message.role}, Content: {message.content}");
            }
        }
    }

    ///<summary>
    /// Summarize contents of conversation history using GPTService. 
    /// Almost works. No longer worth time investment.
    /// </summary>
//    public void SummarizeConversationHistory()
//    {
//        // Need to summarize with conversationhistory not NPCs
//        // Send each value (List<Message>) and return and replace that list with the new single entry
//        foreach (var item in ConversationHistory)
//        {
//            //Debug.Log($"Summarizing {item.Key} convo history from {string.Join(", ", item.Value.Select(msg => $"[{msg.role}:\n {msg.content}]\n"))}");
//            if (gptService == null)
//            {
//                Debug.LogError("gptService is null. Ensure GPTService.Instance is properly initialized.");
//            }
//            if (item.Value == null)
//            {
//                Debug.LogError($"item.Value is null for key: {item.Key}. Check ConversationHistory for inconsistencies.");
//            }
//            if ( item.Value == null || item.Value.Count < 2) { continue; }
//            StartCoroutine(gptService.SummarizeMessagesApiCall(item.Value));
//            //Debug.Log($"Finished summary for {item.Key}");
//            //Debug.Log($"To: {string.Join(", ", item.Value.Select(msg => $"[{msg.role}: {msg.content}]"))}");
//        }
//    }
}
