using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompassArrow : MonoBehaviour
{
    public RectTransform compassArrow;
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
        // Find player
        TryFindPlayer();

        // Clear
        taskTarget = null;

        // Wait for NPC round up
        StartCoroutine(WaitForNPCsThenRefresh());
    }

    IEnumerator WaitForNPCsThenRefresh()
    {
        yield return new WaitUntil(() => NPCManager.Instance.GetNPCs().Count > 0);
        RefreshTaskTarget();
    }

    void TryFindPlayer()
    {
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
            player = pc.transform;
    }

    void Update()
    {
        if (player == null)
        {
            compassArrow.gameObject.SetActive(false);
            return;
        }

        // Refresh for task changes
        RefreshTaskTarget();

        if (taskTarget != null)
        {
            // Show and point arrow
            compassArrow.gameObject.SetActive(true);
            PointAt(taskTarget.position);
        }
        else
        {
            // No tango, no arrow
            compassArrow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Finds the NPC transform and sends it
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
    /// Point the arrow, rotate accordingly
    /// </summary>
    private void PointAt(Vector3 worldPos)
    {
        Vector3 dir   = worldPos - player.position;
        float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Setting the arrow upwards
        compassArrow.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
