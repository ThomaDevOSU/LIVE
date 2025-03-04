using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages Developer Menu functionality.
/// </summary>
public class DevMenu : MonoBehaviour
{
    public static DevMenu Instance;

    public GameObject DeveloperMenu;

    public bool DeveloperMenuOpen { get; private set; } = false;

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

    /// <summary>
    /// Handles dev menu input
    /// </summary>
    public void HandleDevMenuInput()
    {
        if (InputManager.Instance.GetAction("DevMenu").WasPressedThisFrame())
        {
            ToggleDeveloperMenu();
        }
    }

    /// <summary>
    /// For toggling the menu
    /// </summary>
    public void ToggleDeveloperMenu()
    {
        if (!GameManager.Instance.DeveloperModeEnabled) return;

        DeveloperMenuOpen = !DeveloperMenuOpen;

        if (DeveloperMenu != null)
        {
            DeveloperMenu.SetActive(DeveloperMenuOpen);
        }
        else
        {
            Debug.LogWarning("Developer Menu is not assigned!");
        }
    }

    public void PrintCompletedTasks() => PlayerProgressManager.Instance.PrintCompletedTasks();
    public void PrintAllTasks() { TaskManager.Instance.PrintAllTasksToConsole(); TaskManager.Instance.PrintActiveTask(); }
    public void Skip1Hour() => GameClock.Instance.SkipTime(1);
    public void Skip4Hours() => GameClock.Instance.SkipTime(4);
    public void SkipDay() => GameClock.Instance.SetDay(GameClock.Instance.currentDay + 1);
    public void ForceCompleteAllTasks() => GameManager.Instance.ForceCompleteTasks(true);
    public void ForceCompleteCurrentTask() => GameManager.Instance.ForceCompleteTasks(false);
    public void GoToSleep() => GameClock.Instance.GoToSleep();
    public void ForceSleep() => GameClock.Instance.ForceSleep();
}
