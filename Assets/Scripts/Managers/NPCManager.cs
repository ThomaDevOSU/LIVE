using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    // list of NPCs
    // NPCs are added to this list when they are spawned. This might be done in the NPCManager in the future.
    public static List<NPC> NPCs;

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

    public void AddNPC(NPC npc)
    {
        NPCs.Add(npc);
    }

    public void RemoveNPC(NPC npc)
    {
        NPCs.Remove(npc);
    }

    public NPC GetNPC(string name)
    {
        foreach (NPC npc in NPCs)
        {
            if (npc.Name == name)
            {
                return npc;
            }
        }
        return null;
    }

    public List<NPC> GetNPCs()
    {
        return NPCs;
    }

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
