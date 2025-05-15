using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider ambientSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private AudioManager audioManager;

    void Awake()
    {
        // Find audio manager
        audioManager = AudioManager.Instance;
        if (audioManager == null)
            Debug.LogError("SoundSettingsUI: No AudioManager.Instance found!");
    }


    void Start()
    {
        // Init sliders to default
        InitializeSlider(masterSlider, audioManager.masterVolumeParam);
        InitializeSlider(musicSlider,  audioManager.musicVolumeParam);
        InitializeSlider(ambientSlider, audioManager.ambientVolumeParam);
        InitializeSlider(sfxSlider,     audioManager.sfxVolumeParam);
        InitializeSlider(voiceSlider,   audioManager.voiceVolumeParam);

        // Callbacks if needed
        masterSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        ambientSlider.onValueChanged.AddListener(audioManager.SetAmbientVolume);
        sfxSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        voiceSlider.onValueChanged.AddListener(audioManager.SetVoiceVolume);
    }

    /// <summary>
    /// Gets current value in db of audio
    /// </summary>
    void InitializeSlider(Slider slider, string mixerParam)
    {
        if (audioManager.audioMixer.GetFloat(mixerParam, out float db))
        {
            // Conversion
            float lin = Mathf.Pow(10f, db / 20f);
            slider.value = lin;
        }
        else
        {
            // If gone, default
            slider.value = 1f;
        }
    }
}
