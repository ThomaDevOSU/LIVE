using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the in-game menu and pause functionality.
/// Allows toggling the menu via the "Pause" button (Escape).
/// Handles pausing and resuming the game clock when the menu is opened or closed.
/// Implements a cooldown to prevent rapid toggling.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // Singleton instance

    public GameObject PauseMenu;

    [Tooltip("Indicates whether the game is currently paused.")]
    public bool isPaused = false;

    private bool canToggle = true; // Prevents rapid toggling of the menu

    public GameObject InventoryMenu;
    private bool isInventoryOpen = false;

    public GameObject MapMenu;
    private bool isMapOpen = false;

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
        // Setup the get actions and lock them against is talking/paused
        if (InputManager.Instance.GetAction("Pause").WasPressedThisFrame() && !DialogueManager.Instance.isTalking)
        {
            TogglePause();
        }

        if (InputManager.Instance.GetAction("Journal").WasPressedThisFrame() && !DialogueManager.Instance.isTalking && !isPaused)
        {
            ToggleInventory();
        }

        if (InputManager.Instance.GetAction("Map").WasPressedThisFrame() && !DialogueManager.Instance.isTalking && !isPaused)
        {
            ToggleMap();
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

        // Pause should close all menus
        if (isInventoryOpen && InventoryMenu != null)
        {
            isInventoryOpen = false;
            InventoryMenu.SetActive(false);
        }
        if (isMapOpen && MapMenu != null)
        {
            isMapOpen = false;
            MapMenu.SetActive(false);
        }

        bool isMenuOpen = PauseMenu.activeSelf; // Check if menu is open
        isPaused = !isMenuOpen; // Update pause
        PauseMenu.SetActive(isPaused); // Show/hide based

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
    public void UnPause()
    {
        TogglePause();
    }

    /// <summary>
    /// Toggles the inventory menu without pausing.
    /// </summary>
    private void ToggleInventory()
    {
        if (!canToggle) return;

        // If map is open, close it
        if (isMapOpen && MapMenu != null)
        {
            isMapOpen = false;
            MapMenu.SetActive(false);
        }

        bool currentlyOpen = (InventoryMenu != null && InventoryMenu.activeSelf);
        isInventoryOpen = !currentlyOpen;

        if (InventoryMenu != null)
            InventoryMenu.SetActive(isInventoryOpen);

        StartCoroutine(TogglePauseCooldown());
    }

    /// <summary>
    /// Toggles the map menu without pausing.
    /// </summary>
    private void ToggleMap()
    {
        if (!canToggle) return;

        // If inven is open, close it
        if (isInventoryOpen && InventoryMenu != null)
        {
            isInventoryOpen = false;
            InventoryMenu.SetActive(false);
        }

        bool currentlyOpen = (MapMenu != null && MapMenu.activeSelf);
        isMapOpen = !currentlyOpen;

        if (MapMenu != null)
            MapMenu.SetActive(isMapOpen);

        StartCoroutine(TogglePauseCooldown());
    }

    /// <summary>
    /// This unpauses the menu to prevent game from being locked when closing menu.
    /// </summary>
    public void ClosePauseMenu()
    {
        // Only do something if we're actually paused
        if (isPaused)
        {
            isPaused = false;
            PauseMenu.SetActive(false);

            // Resume the clock
            if (GameClock.Instance != null)
                GameClock.Instance.ResumeClock();

            StartCoroutine(TogglePauseCooldown());
        }
    }
}