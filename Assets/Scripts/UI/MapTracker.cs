using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class MapTracker : MonoBehaviour
{
    public RectTransform mapPanel;
    public RectTransform playerIcon;  
    public RectTransform taskIcon;
    public Rect mapBounds;
    private Transform player;
    private Transform taskTarget;
    private bool      isOverworld;
    private string currentSceneName;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryFindPlayer();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        isOverworld = scene.name == "Overworld";
        TryFindPlayer();
        // Clear
        taskTarget = null;
        // Wait for NPCs to spawn
        StartCoroutine(WaitForNPCsThenRefresh());
    }

    IEnumerator WaitForNPCsThenRefresh()
    {
        // Wait for it to populate
        yield return new WaitUntil(() => NPCManager.Instance.GetNPCs().Count > 0);
        RefreshTaskTarget();
    }

    void TryFindPlayer()
    {
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null) player = pc.transform;
    }

    void Update()
    {
        if (!isOverworld || player == null) return;

        // Update player icon
        playerIcon.anchoredPosition = WorldToMapPosition(player.position);

        // Refresh for task changes
        RefreshTaskTarget();

        // Show/hide and position task icon
        if (taskTarget != null)
        {
            taskIcon.gameObject.SetActive(true);
            taskIcon.anchoredPosition = WorldToMapPosition(taskTarget.position);
        }
        else
        {
            taskIcon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Grabs the current active Task, gets transform, sends it
    /// </summary>
    private void RefreshTaskTarget()
    {
        var active = TaskManager.Instance.GetActiveTask();
        if (active == null || string.IsNullOrEmpty(active.TaskNPC))
        {
            taskTarget = null;
            return;
        }

        var npc = NPCManager.Instance.GetNPCs()
                      .FirstOrDefault(n => n.Job == active.TaskNPC);
        if (npc == null)
        {
            taskTarget = null;
            return;
        }

        int hour = Mathf.FloorToInt(GameClock.Instance.currentHour);
        var entry = npc.Schedule.FirstOrDefault(e => e.time == hour);
        if (entry == null)
        {
            taskTarget = null;
            return;
        }

        // If NPC is in Overworld, find em
        if (entry.location == "Overworld")
        {
            taskTarget = npc.agent.transform;
            return;
        }

        // Otherwise, whats behind that door!
        var door = FindObjectsOfType<Door>()
                   .FirstOrDefault(d => d.scene == entry.location);
        if (door != null)
        {
            taskTarget = door.transform;
        }
        else
        {
            taskTarget = null;
        }
    }

    /// <summary>
    /// Converts a world location to map
    /// </summary>
    private Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        float nx = (worldPosition.x - mapBounds.x) / mapBounds.width;
        float ny = (worldPosition.y - mapBounds.y) / mapBounds.height;

        return new Vector2(nx * mapPanel.rect.width,
                           ny * mapPanel.rect.height);
    }
}
