/*
AudioManager.cs
Author: Doug Hillyer, Spring 2025
 
This implementation follows Unity Technologies' best practices for audio management 
including the use of Singleton patterns, persistent audio objects, and AudioMixer 
integration for separate music, ambient, and SFX channels.
 
Inspired by official Unity Documentation and Unity Learn Tutorials (2025).

    The AudioManager class skeleton was developed independently based on Unity Technologies' 
    documentation and commonly accepted industry practices regarding Singleton managers, 
    AudioSource usage, and AudioMixer volume control.

    It was informed by the following Unity documentation resources:

        Unity Technologies. DontDestroyOnLoad. 
        Retrieved from https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html

        Unity Technologies. AudioSource Component Manual. 
        Retrieved from https://docs.unity3d.com/Manual/class-AudioSource.html

        Unity Technologies. Audio Mixer Overview. 
        Retrieved from https://docs.unity3d.com/Manual/AudioMixer.html

        Unity Learn. Controlling Audio Mixer Volume with UI Sliders. 
        Retrieved from https://learn.unity.com/tutorial/audio-mixer#5c8920f6edbc2a002053b4cc

------------------------------------------------------------
Integration Notes for UI Developers:
------------------------------------------------------------
    This AudioManager script is fully set up to support in-game audio sliders.
    UI sliders can be linked to the following public methods:

        AudioManager.Instance.SetMasterVolume(float value);
        AudioManager.Instance.SetMusicVolume(float value);
        AudioManager.Instance.SetAmbientVolume(float value);
        AudioManager.Instance.SetSFXVolume(float value);
        AudioManager.Instance.SetVoiceVolume(float value);

    Each value should range between 0.0001f and 1.0f.
    These are converted to decibel space using a logarithmic curve.
    The mixer parameters must be exposed in the Unity Audio Mixer:
        "MasterVolume", "MusicVolume", "AmbientVolume", "SFXVolume", "VoiceVolume"

    The AudioSource objects should be created as child GameObjects of AudioManager,
    assigned in the Inspector, and routed through the AudioMixer Groups for clarity.
*/

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("AudioSource for background music (looping)")]
    public AudioSource musicSource;

    [Tooltip("AudioSource for ambient environment sounds (looping)")]
    public AudioSource ambientSource;

    [Tooltip("AudioSource for one-shot sound effects")]
    public AudioSource sfxSource;

    [Tooltip("AudioSource for NPC or player voices")]
    public AudioSource voiceSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string ambientVolumeParam = "AmbientVolume";
    public string sfxVolumeParam = "SFXVolume";
    public string voiceVolumeParam = "VoiceVolume";

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

    private float VolumeToDecibels(float volume)
    {
        return Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
    }

    // Music Controls
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource != null && (musicSource.clip != clip || !musicSource.isPlaying))
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // Ambient Controls
    public void PlayAmbient(AudioClip clip, bool loop = true)
    {
        if (ambientSource != null && (ambientSource.clip != clip || !ambientSource.isPlaying))
        {
            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.Play();
        }
    }

    public void StopAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }
    }

    // SFX Controls
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Voice Controls
    public void PlayVoice(AudioClip clip)
    {
        if (voiceSource != null && clip != null)
        {
            voiceSource.PlayOneShot(clip);
        }
    }

    // Volume Controls
    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(masterVolumeParam, VolumeToDecibels(volume));
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParam, VolumeToDecibels(volume));
        }
        else if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetAmbientVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(ambientVolumeParam, VolumeToDecibels(volume));
        }
        else if (ambientSource != null)
        {
            ambientSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParam, VolumeToDecibels(volume));
        }
        else if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    public void SetVoiceVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(voiceVolumeParam, VolumeToDecibels(volume));
        }
        else if (voiceSource != null)
        {
            voiceSource.volume = volume;
        }
    }
}
