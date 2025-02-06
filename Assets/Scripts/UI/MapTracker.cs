using UnityEngine;

public class MapTracker : MonoBehaviour
{
    public RectTransform mapPanel;
    public RectTransform playerIcon;  
    public RectTransform taskIcon;

    public Transform player;
    public Transform taskTarget;

    public Rect mapBounds;

    void Update()
    {
        // For player icon on the map
        Vector2 playerMapPosition = WorldToMapPosition(player.position);
        playerIcon.anchoredPosition = playerMapPosition;

        // For task on the map if exists
        if (taskTarget != null)
        {
            taskIcon.gameObject.SetActive(true);
            Vector2 taskMapPosition = WorldToMapPosition(taskTarget.position);
            taskIcon.anchoredPosition = taskMapPosition;
        }
        else
        {
            taskIcon.gameObject.SetActive(false);
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

