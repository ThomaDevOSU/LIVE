using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// Update location of all NPCs.
    /// </summary>
    void Update()
    {
        // Update location of all NPCs. Not sure how to do this yet
    }

    /// <summary>
    /// Trigger Movement of all NPCs.
    /// </summary>
    private void MoveNPCs()
    {
        foreach (NPC npc in NPCs)
        {
           
        }
    }

    /// <summary>
    /// Adds an NPC to the list.
    /// </summary>
    /// <param name="npc">The NPC to add.</param>
    public void AddNPC(NPC npc)
    {
        NPCs.Add(npc);
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
    /// <example>
    /// <code>
    /// NPC npcByName = NPCManager.Instance.GetNPC(name: "John");
    /// NPC npcById = NPCManager.Instance.GetNPC(id: 123);
    /// NPC npcByNameOrId = NPCManager.Instance.GetNPC(name: "John", id: 123);
    /// </code>
    /// </example>
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
    /// Retrieve a dictionary of all NPC conversation history.
    /// This data should eventually be summarized before being stored in PlayerData.
    /// </summary>
    /// <returns>
    /// Dictionary with NPC names as keys and lists of conversation history as values.
    /// </returns>
    public Dictionary<string, List<Message>> GetConversationHistory()
    {
        foreach (NPC npc in NPCs)
        {
            if (!ConversationHistory.ContainsKey(npc.Name))
            {
                ConversationHistory.Add(npc.Name, npc.messages);
            }
        }
        return ConversationHistory;
    }

    /// <summary>
    /// Load Conversation History from PlayerData.
    /// </summary>
    public void LoadConversationHistoryFromPlayerData(PlayerData data)
    {
        foreach (NPC npc in NPCs)
        {
                npc.messages = data.conversationHistory[npc.Name];
        }
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
}
