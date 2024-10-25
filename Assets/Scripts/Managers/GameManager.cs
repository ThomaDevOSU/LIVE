using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerData CurrentPlayerData;
    public OptionData Options;

    private string locationKey = "";

    private GameObject playerGameObject; // This will be the players gameobject

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

    IEnumerator waitForManagers() // This function will allow us to reliably wait for other magement system to initialize before we take actions requiring them
    {
        while (!(TransitionManager.Instance && SpriteManager.Instance && LocalizationManager.Instance)) yield return new WaitForSeconds(0.1f);

        updateOptions(); // Update all menus and save whatever returned
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) // This will be used to ensure player transitions to the right location
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

    public void setLocation(string loc) // Sets location key
    {
        locationKey = loc;
    }

    public void updateOptions() // When options are updated from a menu all systems update
    {
        SaveSystem.SaveOptions(Options);
        LocalizationManager.Instance.updateLanguageText();
    }

    public bool isOnline() 
    {
        if (Options.online_mode) // If set to online mode, test for connection
        {
            // Fancy code here tests for connection, return true if good connection
        }

        return false;
    }

}

