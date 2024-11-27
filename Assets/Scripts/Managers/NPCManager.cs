using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    // list of NPCs
    public static List<NPC> NPCs;

    void Start()
    {
        // Maybe create a list of NPCs here?
    }

    // Update is called once per frame
    void Update()
    {
        
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
