using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;
    public List<Transform> waypoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("\n\nWaypoint Manager OnSceneLoaded: " + scene.name + "\n\n");

        // Get all Active Waypoints
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        // Waypoints need to be of type transform for NavMesh
        waypoints = new List<Transform>();
        foreach (var waypointObject in waypointObjects)
        {
            waypoints.Add(waypointObject.transform);
            Debug.Log("Waypoint added" +  waypointObject.name + "\n\n" + waypointObject.transform);
            Debug.Log(waypointObject);
        }
    }

    public List<Transform> GetAllActiveWaypoints()
    {
        return waypoints;
    }

    /// <summary>
    /// Get waypoint by name.
    /// </summary>
    public Transform GetWaypoint(string name)
    {
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint.name == name)
            {
                return waypoint;
            }
        }
        return null;
    }
}

