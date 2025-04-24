using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTracker : MonoBehaviour
{
    public RectTransform mapPanel;
    public RectTransform playerIcon;  
    public RectTransform taskIcon;

    private Transform player;
    private Transform taskTarget;
    private bool isOverworld;

    public Rect mapBounds;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryFindPlayer();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Are we in main world
        isOverworld = scene.name == "Overworld";

        // Find dat playa
        TryFindPlayer();
    }

    private void TryFindPlayer()
    {
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
            player = pc.transform;
    }

    private void Update()
    {
        // Update if in main world
        if (isOverworld && player != null)
        {
            // Playa icon
            playerIcon.anchoredPosition = WorldToMapPosition(player.position);

            // Task icon
            if (taskTarget != null)
            {
                taskIcon.gameObject.SetActive(true);
                taskIcon.anchoredPosition = WorldToMapPosition(taskTarget.position);
            }
            else taskIcon.gameObject.SetActive(false);
        }
    }

    // For calculating map position
    private Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        float normalizedX = (worldPosition.x - mapBounds.x) / mapBounds.width;
        float normalizedY = (worldPosition.y - mapBounds.y) / mapBounds.height;

        float mapWidth = mapPanel.rect.width;
        float mapHeight = mapPanel.rect.height;

        return new Vector2(normalizedX * mapWidth, normalizedY * mapHeight);
    }

    // (For the future) Dynamic targets
    public void SetTaskTarget(Transform newTaskTarget)
    {
        taskTarget = newTaskTarget;
    }

    // (For the future) Clear target
    public void ClearTaskTarget()
    {
        taskTarget = null;
    }
}

