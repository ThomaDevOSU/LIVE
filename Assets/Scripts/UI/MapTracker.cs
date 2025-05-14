using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTracker : MonoBehaviour
{
    public RectTransform mapPanel;
    public RectTransform playerIcon;  
    public RectTransform taskIcon;
    public Rect mapBounds;
    private Transform player;
    private Transform taskTarget;
    private bool      isOverworld;

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
        if (active != null && !string.IsNullOrEmpty(active.TaskNPC))
        {
            taskTarget = NPCManager.Instance.GetTransformForJob(active.TaskNPC);
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
