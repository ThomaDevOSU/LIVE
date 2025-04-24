using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// For the Display settings
/// </summary>
public class DisplayManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public TMP_Dropdown fpsLimitDropdown;

    private Resolution[] resolutions;
    private int currentResolutionIndex;

    void Start()
    {
        LoadResolutions();
        LoadFPSOptions();
        LoadSettings();
    }

    /// <summary>
    /// Get resolutions from screen
    /// </summary>
    void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = resolutions[i].width + " x " + resolutions[i].height;
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionOption));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    /// <summary>
    /// Fix the FPS issue
    /// </summary>
    void LoadFPSOptions()
    {
        fpsLimitDropdown.ClearOptions();

        // Set 1:1
        var labels = new List<string>
        {
            "30 FPS",
            "60 FPS",
            "120 FPS",
            "144 FPS",
            "240 FPS",
            "Unlimited"
        };

        fpsLimitDropdown.AddOptions(labels);
    }

    /// <summary>
    /// Method to set resolution
    /// </summary>
    /// <param name="index"></param>
    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Method to set fullscreen
    /// </summary>
    /// <param name="isFullscreen"></param>
    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Method to set vsync
    /// </summary>
    /// <param name="isVSyncOn"></param>
    public void ToggleVSync(bool isVSyncOn)
    {
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isVSyncOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Method to set fps limit, -1 is infinite
    /// </summary>
    /// <param name="index"></param>
    public void SetFPSLimit(int index)
    {
        int[] fpsLimits = { 30, 60, 120, 144, 240, -1 };
        Application.targetFrameRate = fpsLimits[index];
        PlayerPrefs.SetInt("FPSLimit", index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads settings
    /// </summary>
    void LoadSettings()
    {
        // Load fullscreen
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);

        // Load V-Sync
        vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1;
        vSyncToggle.onValueChanged.AddListener(ToggleVSync);

        // Load FPS limit
        int savedFPSIndex = PlayerPrefs.GetInt("FPSLimit", 1);
        fpsLimitDropdown.value = Mathf.Clamp(savedFPSIndex, 0, fpsLimitDropdown.options.Count - 1);
        fpsLimitDropdown.RefreshShownValue();
        fpsLimitDropdown.onValueChanged.AddListener(SetFPSLimit);
    }
}

