using System.Collections;
using UnityEngine;

public class GameClockVerboseTester : MonoBehaviour
{
    private GameClock gameClock;

    private void Start()
    {
        // Locate the GameClock in the scene
        gameClock = FindFirstObjectByType<GameClock>();

        if (gameClock == null)
        {
            Debug.LogError("GameClock instance not found in the scene. Please add it to the scene.");
            return;
        }

        Debug.Log("Starting GameClock Verbose Tester...");
        StartCoroutine(TestGameClock());
    }

    private void Update()
    {
        // Check for manual time skip input
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Manual time skipping triggered: Skipping 3 hours...");
            gameClock.SkipTime(3f); // Skip 3 game hours
        }
    }

    private IEnumerator TestGameClock()
    {
        while (true)
        {
            // Print game clock details every 5 seconds
            Debug.Log($"[Game Clock Update] Day: {gameClock.currentDay}, Hour: {gameClock.currentHour:F2}");
            Debug.Log($"Elapsed Game Time: {gameClock.elapsedGameTime:F2}s / {gameClock.gameDayDuration}s");
            Debug.Log($"Current State: {gameClock.CurrentState}");

            // Test day/night transitions
            if (gameClock.CurrentState == GameClock.ClockState.Night)
            {
                Debug.Log("It's nighttime! Players should head to bed.");
            }
            else if (gameClock.CurrentState == GameClock.ClockState.Day)
            {
                Debug.Log("It's daytime! Time to explore and complete tasks.");
            }

            // Add a delay of 15 real seconds before next update
            yield return new WaitForSeconds(5f);
        }
    }
}
