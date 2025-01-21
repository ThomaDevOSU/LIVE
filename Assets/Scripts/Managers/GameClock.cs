using System;
using UnityEngine;

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
    public float gameDayDuration = 14f * 60f; // Total duration of a game day in real seconds (default: 14 minutes)
    private const float gameHoursPerDay = 24f; // Total in-game hours in a day

    // Current game time state
    public float elapsedGameTime = 0f; // Total elapsed time within the current game day
    public int currentDay { get; private set; } = 1; // Tracks the current day number
    public float currentHour { get; private set; } = 8f; // Current in-game hour (starts at 8 AM)

    // Game clock states for day/night transitions
    public enum ClockState { Day, Evening, Night }
    public ClockState CurrentState { get; private set; } = ClockState.Day;

    // Events for day/night state transitions and day progression
    public event Action OnDayStart; // Triggered when the day starts
    public event Action OnEveningStart; // Triggered when the evening starts
    public event Action OnNightStart; // Triggered when the night starts
    public event Action OnDayEnd; // Triggered when the day ends

    /// <summary>
    /// Ensures the GameClock is a singleton and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Updates the game time every frame. Time progresses based on the real-time to game-time multiplier.
    /// </summary>
    private void Update()
    {
        UpdateGameTime(Time.deltaTime); // Progress the game clock by the time elapsed since the last frame
    }

    /// <summary>
    /// Updates the elapsed game time and triggers state transitions and day-end logic.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last frame in real seconds</param>
    private void UpdateGameTime(float deltaTime)
    {
        elapsedGameTime += deltaTime * realTimeToGameTimeMultiplier; // Adjust elapsed game time
        currentHour = 8f + (elapsedGameTime / gameDayDuration) * gameHoursPerDay; // Calculate current hour

        UpdateClockState(); // Check and trigger state transitions

        if (currentHour >= 24f) // End the day if the hour exceeds 24
        {
            EndDay();
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
        }
        else if (currentHour >= 22f && CurrentState != ClockState.Night)
        {
            CurrentState = ClockState.Night;
            OnNightStart?.Invoke(); // Trigger night event
        }
        else if (currentHour < 18f && CurrentState != ClockState.Day)
        {
            CurrentState = ClockState.Day;
            OnDayStart?.Invoke(); // Trigger day event
        }
    }

    /// <summary>
    /// Ends the current day, resets the game time, and triggers the start of a new day.
    /// </summary>
    private void EndDay()
    {
        OnDayEnd?.Invoke(); // Trigger day-end event
        currentDay++; // Increment the day counter
        elapsedGameTime = 0f; // Reset elapsed game time
        currentHour = 8f; // Reset to the start of the next day (8 AM)
        CurrentState = ClockState.Day; // Reset the clock state to Day
        OnDayStart?.Invoke(); // Trigger the start of the new day
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
