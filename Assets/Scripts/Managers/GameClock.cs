using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameClock manages the in-game time system, providing a dynamic clock that progresses in real time,
/// triggers day/night state transitions, and allows for manual time skipping. This is a singleton class
/// that ensures only one instance exists in the game.
/// </summary>
public class GameClock : MonoBehaviour
{
    // Singleton instance of GameClock
    public static GameClock Instance;

    // Configuration: Adjust the real-time to game-time multiplier and the duration of a game day (in seconds)
    public float realTimeToGameTimeMultiplier = 1f; // How many seconds in real time equals one second in game time
    private float originalTimeMultiplier = 1f; // Store the original speed when paused
    public float gameDayDuration = 14f * 60f; // Total duration of a game day in real seconds (default: 14 minutes)
    private const float gameHoursPerDay = 24f; // Total in-game hours in a day

    // Current game time state
    public float elapsedGameTime = 0f; // Total elapsed time within the current game day
    public int currentDay { get; private set; } = 1; // Tracks the current day number
    public float currentHour { get; private set; } = 8f; // Current in-game hour (starts at 8 AM)

    private bool manualSleepMode = true; // Default to manual mode

    // Game clock states for day/night transitions
    public enum ClockState { Day, Evening, Night, SleepPending }
    public ClockState CurrentState { get; private set; } = ClockState.Day;

    // Events for day/night state transitions and day progression
    public event Action OnDayStart; // Triggered when the day starts
    public event Action OnEveningStart; // Triggered when the evening starts
    public event Action OnNightStart; // Triggered when the night starts
    public event Action OnDayEnd; // Triggered when the day ends
    public event Action OnSleepPending; // Triggered when game day time stops

    public bool _cansleep; // This determines whether the player can sleep

    /// <summary>
    /// Ensures the GameClock is a singleton and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _cansleep = false;
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private IEnumerator Start()
    {
        // Wait until GameManager is fully initialized
        while (GameManager.Instance == null || GameManager.Instance.CurrentPlayerData == null)
        {
            Debug.LogWarning("Waiting for GameManager and PlayerData to initialize...");
            yield return new WaitForSeconds(0.1f);
        }

        LoadGameClockFromSave(GameManager.Instance.CurrentPlayerData);
        Debug.Log($"Loaded Clock Data from Save: Day {currentDay}");

        AssignTintOverlay(); // Reassign TintOverlay dynamically

        // Initialize the tint color based on the current state
        UpdateTintOverlay(true); // Initialize with instant transition
    }

    /// <summary>
    /// Updates the game time every frame. Time progresses based on the real-time to game-time multiplier.
    /// </summary>
    private void Update()
    {
        UpdateGameTime(Time.deltaTime); // Progress the game clock by the time elapsed since the last frame
        UpdateTintOverlay(); // Smoothly update the tint overlay
    }

    /// <summary>
    /// Updates the elapsed game time and triggers state transitions and day-end logic.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last frame in real seconds</param>
    private void UpdateGameTime(float deltaTime)
    {
        if (CurrentState == ClockState.SleepPending) return;

        elapsedGameTime += deltaTime * realTimeToGameTimeMultiplier; // Adjust elapsed game time
        currentHour = Mathf.Min(8f + (elapsedGameTime / gameDayDuration) * gameHoursPerDay, 24f); // Ensures hour never exceeds 24

        UpdateClockState(); // Check and trigger state transitions

        if (currentHour > 18f) _cansleep = true;


        if (currentHour >= 24f) // End the day if the hour exceeds 24
        {
            if (manualSleepMode)
            {
                CurrentState = ClockState.SleepPending;
                UpdateTintOverlay();
                OnSleepPending?.Invoke();
                Debug.Log("Time is now frozen in SleepPending state. Player must trigger sleep.");
                return;
            }
            else
            {
                EndDay();
            }
        }
    }

    /// <summary>
    /// Updates the current clock state (Day, Evening, Night) based on the current hour
    /// and triggers associated events.
    /// </summary>
    private void UpdateClockState()
    {
        if (currentHour >= 18f && CurrentState != ClockState.Evening)
        {
            CurrentState = ClockState.Evening;
            OnEveningStart?.Invoke(); // Trigger evening event
            UpdateTintOverlay();
        }
        else if (currentHour >= 22f && CurrentState != ClockState.Night)
        {
            CurrentState = ClockState.Night;
            OnNightStart?.Invoke(); // Trigger night event
            UpdateTintOverlay();
        }
        else if (currentHour < 18f && CurrentState != ClockState.Day)
        {
            CurrentState = ClockState.Day;
            OnDayStart?.Invoke(); // Trigger day event
            _cansleep = false;
            UpdateTintOverlay();
        }
    }

    /// <summary>
    /// The player will attempt to sleep here
    /// </summary>
    public void TrySleep() 
    {
        if (!_cansleep
            || MenuManager.Instance.isPaused
            || DialogueManager.Instance.isTalking)
        {
            Debug.Log("The Player cannot Sleep!");
            return;
        }
        // Capture today's tasks for use in End of Day Summary
        CaptureTodaysCompletedTasks();

        Debug.Log("Player is sleeping�");
        _cansleep = false;

        CurrentState = ClockState.SleepPending;

        // Get EndDayMenu everytime
        var mgrGO = MenuManager.Instance.gameObject;
        var endMenuTrans = mgrGO.transform.Find("EndDayMenu");
        if (endMenuTrans != null)
        {
            endMenuTrans.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Bed: Could not find EndDayMenu!");
        }
    }

    /// <summary>
    /// Manual Player Sleep Function, forces sleep end-of day from any state. 
    /// </summary>
    public void ForceSleep()
    {
        EndDay();
    }

    /// <summary>
    /// Normal Player Sleep Function, available in SleepPending State at the end of day.
    /// </summary>
    public void GoToSleep()
    {
        //EndDay(); // Over-riding to change end of day functionality. Player may sleep at any time. 
        if (CurrentState == ClockState.SleepPending)
        {
            EndDay();
        }
    }

    // Temp List for todays Completed Tasks. Used by UI before EndDay() can be called
    public List<Task> todaysCompletedTasks = new List<Task>();

    // And Store Rewards Temp
    public int dailyCoinsEarned = 0;
    public int dailyScoreEarned = 0;


    /// <summary>
    /// Prepares the daily summary by pulling all tasks completed today from TaskManager.
    /// This should be called BEFORE EndDay() is triggered.
    /// </summary>
    public void CaptureTodaysCompletedTasks()
    {
        todaysCompletedTasks.Clear();
        dailyCoinsEarned = 0;
        dailyScoreEarned = 0;

        if (TaskManager.Instance == null) return;

        while (TaskManager.Instance.PeekCompletedTask() != null)
        {
            var task = TaskManager.Instance.GetCompletedTask();
            todaysCompletedTasks.Add(task);

            // Reward logic
            dailyCoinsEarned += 10 * task.TaskDifficulty;
            dailyScoreEarned += 1 * task.TaskDifficulty;
        }

        // Bonus for completing all tasks
        if (todaysCompletedTasks.Count >= 5)
        {
            dailyCoinsEarned += 25; // Completion bonus
            dailyScoreEarned += 3;  // Score bonus
        }

        Debug.Log($"Captured {todaysCompletedTasks.Count} tasks for daily summary.");
        Debug.Log($"Earned {dailyCoinsEarned} coins and {dailyScoreEarned} score.");
    }

    /// <summary>
    /// Returns the tasks captured for the current day's summary.
    /// </summary>
    public List<Task> GetTodaysCompletedTasks()
    {
        return todaysCompletedTasks;
    }

    /// <summary>
    /// Ends the current day, resets the game time, and triggers the start of a new day.
    /// </summary>
    private void EndDay()
    {
        OnDayEnd?.Invoke(); // Trigger day-end event

        // Start a transition effect before resetting the day
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.StartTransition(SceneManager.GetActiveScene().name, TransitionType.WIPE);
        }
        else
        {
            Debug.LogWarning("TransitionManager is null! Skipping transition.");
        }

        currentDay++; // Increment the day counter
        elapsedGameTime = 0f; // Reset elapsed game time
        currentHour = 8f; // Reset to the start of the next day (8 AM)
        CurrentState = ClockState.Day; // Reset the clock state to Day

        Debug.Log($"Starting new day: {currentDay}");



        // Retrieve and store completed tasks
        if (PlayerProgressManager.Instance != null && todaysCompletedTasks != null)
        {
            foreach (Task completedTask in todaysCompletedTasks)
            {
                PlayerProgressManager.Instance.SaveCompletedTask(completedTask);
                Debug.Log($"Stored completed task: {completedTask.TaskDescription}");
            }
            todaysCompletedTasks.Clear(); // Clear after saving to avoid reuse
        }
        else
        {
            Debug.LogWarning("PlayerProgressManager or todaysCompletedTasks is null. No tasks were saved.");
        }

        // Apply Daily Rewards
        if (PlayerProgressManager.Instance != null)
        {
            PlayerProgressManager.Instance.AddCurrency(dailyCoinsEarned);
            if (GameManager.Instance.CurrentPlayerData != null)
            {
                GameManager.Instance.CurrentPlayerData.score += dailyScoreEarned;

                //Calculating player level based on score
                GameManager.Instance.CurrentPlayerData.preferredDifficulty = Math.Clamp(GameManager.Instance.CurrentPlayerData.score/(25* GameManager.Instance.CurrentPlayerData.preferredDifficulty), GameManager.Instance.CurrentPlayerData.preferredDifficulty, 3);
            }
        }



        // Ensure GameManager & PlayerData exist before saving
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayerData != null)
        {
            SaveGameClockData(GameManager.Instance.CurrentPlayerData);

            if (PlayerProgressManager.Instance != null)
            {
                PlayerProgressManager.Instance.SaveProgressToPlayerData(GameManager.Instance.CurrentPlayerData);
            }
            else
            {
                Debug.LogError("PlayerProgressManager.Instance is null! Progress not saved.");
            }

            // Save to slot
            SaveSystem.SaveGame(GameManager.Instance.CurrentPlayerData, GameManager.Instance.CurrentSaveSlot);
        }
        else
        {
            Debug.LogError("GameManager or CurrentPlayerData is null! Unable to save game clock data.");
        }

        // Notify other systems that the day has ended
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.ClearUncompletedTasks();
            int playerDifficulty = GameManager.Instance.CurrentPlayerData?.preferredDifficulty ?? 1;
            TaskManager.Instance.GenerateTasks(5, playerDifficulty);
        }
        else
        {
            Debug.LogError("TaskManager.Instance is null! Could not generate new tasks.");
        }

        OnDayStart?.Invoke(); // Trigger the start of the new day
    }

    public void SetManualSleepMode(bool enable)
    {
        manualSleepMode = enable;
        Debug.Log($"Manual Sleep Mode: {(manualSleepMode ? "Enabled" : "Disabled")}");
    }

    /// <summary>
    /// Skips a specified number of in-game hours. If skipping exceeds the current day, the day ends.
    /// </summary>
    /// <param name="hoursToSkip">Number of in-game hours to skip</param>
    public void SkipTime(float hoursToSkip)
    {
        float skipSeconds = hoursToSkip * (gameDayDuration / gameHoursPerDay); // Convert hours to seconds
        elapsedGameTime += skipSeconds; // Add the skipped time to elapsed time

        if (elapsedGameTime >= gameDayDuration) // If skipping exceeds the current day duration
        {
            EndDay(); // End the day and reset the clock
        }
    }
    /// <summary>
    /// Set day number manually. 
    /// </summary>
    public void SetDay(int dayNumber)
    {
        currentDay = dayNumber;
        Debug.Log($"Manually set day to: {currentDay}");
    }

    /// <summary>
    /// Pauses the in-game clock by setting the time multiplier to 0.
    /// </summary>
    public void PauseClock()
    {
        if (realTimeToGameTimeMultiplier != 0f) // Prevent overwriting pause state
        {
            originalTimeMultiplier = realTimeToGameTimeMultiplier; // Store current speed
            realTimeToGameTimeMultiplier = 0f; // Stop time progression
            Debug.Log("GameClock paused.");
        }
    }

    /// <summary>
    /// Resumes the in-game clock by restoring the previous time multiplier.
    /// </summary>
    public void ResumeClock()
    {
        if (realTimeToGameTimeMultiplier == 0f) // Only resume if actually paused
        {
            realTimeToGameTimeMultiplier = originalTimeMultiplier; // Restore time speed
            Debug.Log("GameClock resumed.");
        }
    }

    /// <summary>
    /// Returns a formatted string of the current in-game time (HH:mm).
    /// </summary>
    /// <returns>Formatted string of the current time</returns>
    public string GetFormattedTime()
    {
        int hour = Mathf.FloorToInt(currentHour); // Extract the hour
        int minute = Mathf.FloorToInt((currentHour - hour) * 60); // Extract the minute
        return $"{hour:D2}:{minute:D2}"; // Format as HH:mm
    }

    /// <summary>
    /// Loads the current day and time progress from PlayerData.
    /// </summary>
    public void LoadGameClockFromSave(PlayerData data)
    {
        currentDay = data.day;

        Debug.Log($"GameClock Loaded: Day {currentDay}, Time {currentHour:F2}");
    }

    /// <summary>
    /// Saves the current day and time progress into PlayerData for persistent saving.
    /// </summary>
    public void SaveGameClockData(PlayerData data)
    {
        //GameManager.Instance.CurrentPlayerData.day = currentDay;
        data.day = currentDay;
    }

    /// <summary>
    /// Smoothly updates the TintOverlay color for day/night transitions.
    /// </summary>
    /// <param name="instant">If true, instantly sets the color without transition.</param>
    //[SerializeField] private Image tintOverlayPanel;
    [SerializeField] private Image tintOverlayImage;
    private Color targetTint;
    private float transitionSpeed = 0.5f;
    private void UpdateTintOverlay(bool instant = false)
    {
        if (tintOverlayImage == null) return; // Safeguard if the Image component is missing

        // Set the target color based on current state
        if (CurrentState == ClockState.Day)
        {
            targetTint = new Color(0, 0, 0, 0); // Day - Fully transparent
        }
        else if (CurrentState == ClockState.Evening)
        {
            targetTint = new Color(0, 0, 0, 0.6f); // Evening - Slightly dark
        }
        else if (CurrentState == ClockState.Night)
        {
            targetTint = new Color(0, 0, 0, 0.9f); // Night - Darker
        }
        else if (CurrentState == ClockState.SleepPending)
        {
            targetTint = new Color(0, 0, 0, 0.92f); // Full Night - Darkest
        }
        else
        {
            targetTint = new Color(0, 0, 0, 0); // Default to transparent
        }

        // Instantly apply the color if requested
        if (instant)
        {
            tintOverlayImage.color = targetTint;
        }
        else
        {
            // Smooth transition using Color.Lerp()
            tintOverlayImage.color = Color.Lerp(tintOverlayImage.color, targetTint, transitionSpeed * Time.deltaTime);
        }
    }

    public void AssignTintOverlay()
    {
        // Find the TintOverlay object dynamically in the new scene
        GameObject tintObj = GameObject.Find("TintOverlay");

        if (tintObj != null)
        {
            tintOverlayImage = tintObj.GetComponent<Image>();

            if (tintOverlayImage == null)
            {
                Debug.LogError("No Image component found on TintOverlay!");
            }
        }
        else
        {
            Debug.LogWarning("TintOverlay not found in the current scene!");
        }
    }

    /// <summary>
    /// How to Access and Use GameClock:
    /// - To access the GameClock instance:
    ///     GameClock gameClock = GameClock.Instance;
    /// 
    /// - To subscribe to state transition events (e.g., start of day or night):
    ///     GameClock.Instance.OnDayStart += YourDayStartHandler;
    ///     GameClock.Instance.OnNightStart += YourNightStartHandler;
    ///
    /// - To get the current day, hour, or formatted time:
    ///     int currentDay = GameClock.Instance.currentDay;
    ///     float currentHour = GameClock.Instance.currentHour;
    ///     string formattedTime = GameClock.Instance.GetFormattedTime();
    /// 
    /// - To skip time manually (e.g., for testing or gameplay logic):
    ///     GameClock.Instance.SkipTime(3f); // Skip 3 in-game hours
    ///
    /// Hooks:
    /// - Use the provided events to trigger game logic at specific times:
    ///     * OnDayStart: Triggered at the start of the day (8 AM).
    ///     * OnEveningStart: Triggered at the start of the evening (6 PM).
    ///     * OnNightStart: Triggered at the start of the night (10 PM).
    ///     * OnDayEnd: Triggered at the end of the day (midnight).
    /// </summary>
}
