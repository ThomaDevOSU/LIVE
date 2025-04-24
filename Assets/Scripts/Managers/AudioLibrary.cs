using UnityEngine;

/*
AudioLibrary.cs
Author: Doug Hillyer, Spring 2025

This script serves as a centralized container for frequently used AudioClips.
It allows global access to music, ambient, SFX, and voice clips throughout the project
via AudioLibrary.Instance.<clip>. This approach prevents duplication and simplifies management
of audio resources across scenes and systems.

Example Usage:
AudioManager.Instance.PlayMusic(AudioLibrary.Instance.townTheme);
AudioManager.Instance.PlaySFX(AudioLibrary.Instance.doorOpen);
*/

public class AudioLibrary : MonoBehaviour
{
    public static AudioLibrary Instance { get; private set; }

    [Header("Music")]
    public AudioClip mainTheme;
    public AudioClip townTheme;
    public AudioClip eveningTheme;
    public AudioClip daytimeTheme;

    [Header("Ambient Sounds")]
    public AudioClip rain;
    public AudioClip cityNoise;

    [Header("Sound Effects")]
    public AudioClip doorOpen;
    public AudioClip footsteps;
    public AudioClip taskComplete;
    public AudioClip uiClick;

    [Header("Voice Clips")]
    public AudioClip greetingHello;
    public AudioClip farewellBye;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
