using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton for the GameManager Instance
    /// </summary>
    public static GameManager Instance;

    /// <summary>
    /// CurrentSaveSlot will keep track of the current active save slot being used in the game state.
    /// This defaults to 0, but is reassigned in the LoadGame function.
    /// </summary>
    public int CurrentSaveSlot = 0;

    /// <summary>
    /// The CurrentPlayerData object represents the player that has been loaded by the LoadGame function
    /// </summary>
    public PlayerData CurrentPlayerData;

    /// <summary>
    /// The Options object represents our active option settings.
    /// </summary>
    public OptionData Options;

    /// <summary>
    /// The locationKey can be edited using the SetLocation function, it is used when transporting the player from level to level.
    /// </summary>
    private string locationKey = "";

    /// <summary>
    /// The players Gameobject
    /// </summary>
    private GameObject playerGameObject;

    /// <summary>
    /// 
    /// </summary>
    public bool DeveloperModeEnabled { get; private set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public event Action OnDeveloperModeToggled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Options = SaveSystem.LoadOptions(); // Attempt to get our option preferences         
            StartCoroutine(waitForManagers()); // Wait for other managers to load

            DontDestroyOnLoad(gameObject);
        }
        else 
        {
        Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleDeveloperModeToggle();
        if (DeveloperModeEnabled)
        {
            HandleDeveloperTools();
        }
    }
    

    IEnumerator waitForManagers() // This function will allow us to reliably wait for other magement system to initialize before we take actions requiring them
    {
        while (!(TransitionManager.Instance && SpriteManager.Instance && LocalizationManager.Instance)) yield return new WaitForSeconds(0.1f);

        updateOptions(); // Update all menus and save whatever returned
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// When a new scene is loaded, The OnSceneLoaded function will check the new scene for different "SpawnPoints" that it can send the player to.
    /// Whichever SpawnPoints' string matches the current locationKey will be the spawnpoint the player is assigned.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return; // No main menu

        Debug.Log("Loaded into scene");

        playerGameObject = FindFirstObjectByType<PlayerController>().gameObject; // Find play on map


        foreach (var spawn in Resources.FindObjectsOfTypeAll(typeof(SpawnPoint)) as SpawnPoint[]) 
        {
            if (locationKey == spawn.locationValue) // If we have a match
            {
                Debug.Log($"Found location! {locationKey}");
                playerGameObject.transform.position = spawn.gameObject.transform.position; // Set our location accordingly
            }
        }

    }

    /// <summary>
    /// Simple setter function for the locationKey
    /// </summary>
    /// <param name="loc"></param>
    public void setLocation(string loc)
    {
        locationKey = loc;
    }

    /// <summary>
    /// When options are updated, the new options are saved to file and then the update function for
    /// the options is called.
    /// </summary>
    public void updateOptions()
    {
        SaveSystem.SaveOptions(Options);
        LocalizationManager.Instance.updateLanguageText();
    }

    /// <summary>
    /// Checker function for online accessibilty, NOT YET IMPLEMENTED
    /// </summary>
    /// <returns></returns>
    public bool isOnline() 
    {
        if (Options.online_mode) // If set to online mode, test for connection
        {
            // Fancy code here tests for connection, return true if good connection
        }

        return false;
    }

    private void HandleDeveloperModeToggle()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DeveloperModeEnabled = !DeveloperModeEnabled;
            OnDeveloperModeToggled?.Invoke();
            Debug.Log($"Developer Mode {(DeveloperModeEnabled ? "Enabled" : "Disabled")}");
            if (DeveloperModeEnabled)
            {
                PrintDeveloperHotkeys();
            }
        }
    }
    private void HandleDeveloperTools()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TaskManager.Instance.PrintTaskList();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TaskManager.Instance.PrintAllTasksToConsole();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameClock.Instance.GoToSleep();
        }        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameClock.Instance.SkipTime(1);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameClock.Instance.SkipTime(4);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            GameClock.Instance.SetDay(GameClock.Instance.currentDay + 1);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            GameClock.Instance.ForceSleep();
        }
    }

    private void PrintDeveloperHotkeys()
    {
        Debug.Log("Developer Mode Hotkeys:\n" +
                  "F2 - Print Task List\n" +
                  "F3 - Print All Tasks\n" +
                  "F4 - Go To Sleep (End Day)\n" +
                  "F5 - Skip 1 Hour\n" +
                  "F6 - Skip 4 Hours\n" +
                  "F7 - Set Next Day\n" +
                  "F9 - Toggle Verbose Clock Logging\n" +
                  "F12 - Force Sleep & End Day\n");
    }
}


