using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the in-game menu and pause functionality.
/// Allows toggling the menu via the "Cancel" button (typically ESC).
/// Handles pausing and resuming the game clock when the menu is opened or closed.
/// Implements a cooldown to prevent rapid toggling.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // Singleton instance

    [Tooltip("Reference to the in-game player menu UI.")]
    public GameObject PlayerMenu;

    [Tooltip("Indicates whether the game is currently paused.")]
    public bool isPaused = false;

    private bool canToggle = true; // Prevents rapid toggling of the menu

    private void Awake()
    {
        // Ensure only one instance of MenuManager exists (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Update()
    {
        // Check for "Cancel" button (Escape key by default) and ensure dialogue is not active
        if (Input.GetButtonDown("Cancel") && !DialogueManager.Instance.isTalking)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Cooldown coroutine to prevent rapid toggling of the menu.
    /// Ensures the toggle function isn't called multiple times in quick succession.
    /// </summary>
    public IEnumerator TogglePauseCooldown()
    {
        canToggle = false; // Disable further toggles
        yield return new WaitForSeconds(0.2f); // Small delay before allowing toggling again
        canToggle = true; // Enable toggling
    }

    /// <summary>
    /// Toggles the pause state, showing or hiding the menu and pausing/resuming game time.
    /// </summary>
    public void TogglePause()
    {
        if (!canToggle) return; // Prevent spam-clicking

        bool isMenuOpen = PlayerMenu.activeSelf; // Check if the menu is currently open
        isPaused = !isMenuOpen; // Update pause state
        PlayerMenu.SetActive(isPaused); // Show/hide the menu UI

        // Pause or resume the game clock if it exists
        if (GameClock.Instance != null)
        {
            if (isPaused)
                GameClock.Instance.PauseClock();
            else
                GameClock.Instance.ResumeClock();
        }

        StartCoroutine(TogglePauseCooldown()); // Start cooldown to prevent spam
        Debug.Log($"Menu Toggled - isPaused: {isPaused}"); // Log state change for debugging
    }

    /// <summary>
    /// Closes the menu and resumes the game.
    /// This is called by the "Close" button inside the UI menu.
    /// </summary>
    public void unPause()
    {
        TogglePause();
    }
}
