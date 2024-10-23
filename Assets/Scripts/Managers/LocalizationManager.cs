using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance; // Singleton

    public Dictionary<string, string> localizedMenuText; // We will keep the current menu language in this dictionary

    public string menuLanguage;

    public Dictionary<string, string> localizedLearningText; // We will keep the current learning language in this dictionary

    public string learningLanguage;

    public TMP_FontAsset[] fonts; // 0 is baseline, 1 is eastern supporting, 2 is arabic supporting

    private void Awake() // Singleton
    {
        if (Instance == null)
        {
            Instance = this;
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
        while (!(TransitionManager.Instance && SpriteManager.Instance && GameManager.Instance)) yield return new WaitForSeconds(0.1f);
        updateLanguageText(); // Load game managers options
    }

    public void LoadLocalizedMenuText(string language) // Attempts to load localized text from file into menu dictionary
    {
        menuLanguage = language;
        string path = Path.Combine(Application.streamingAssetsPath, $"Localization/{language}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            localizedMenuText = JsonUtility.FromJson<LocalizationData>(json).ToDictionary(); // Converts json to localizationdata which converts to dictionary
            Debug.Log($"Loaded {language} localization");
        }
        else
        {
            Debug.LogError($"Localization file not found: {path}");
        }
    }

    public void LoadLocalizedLearningText(string language) // Attempts to load localized text from file into menu dictionary
    {
        learningLanguage = language;
        string path = Path.Combine(Application.streamingAssetsPath, $"Localization/{language}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            localizedLearningText = JsonUtility.FromJson<LocalizationData>(json).ToDictionary(); // Converts json to localizationdata which converts to dictionary
            Debug.Log($"Loaded {language} localization");
        }
        else
        {
            Debug.LogError($"Localization file not found: {path}");
        }
    }

    public string GetLocalizedValue(string key, isUI ui) // Get string from Dictionary depending if what is loaded is UI
    {
        if (ui == isUI.UI)
        {
            if (localizedMenuText != null && localizedMenuText.TryGetValue(key, out string value))
            {
                return value;
            }
        }
        else 
        {
            if (localizedLearningText != null && localizedLearningText.TryGetValue(key, out string value))
            {
                return value;
            }
        }

        return "N/A";
    }


    public void updateLanguageText() // Sets the language of every text
    {
        if (this != null && GameManager.Instance != null && menuLanguage != GameManager.Instance.Options.language) // If our assigned menu language does not match the options value
        {
            Debug.Log("Loading Language");
            LoadLocalizedMenuText(GameManager.Instance.Options.language); // Load LocalizationManager to fill dictionary

            foreach (var localizedText in Resources.FindObjectsOfTypeAll(typeof(LocalizedText)) as LocalizedText[]) // Update all TMP assets to reflect
            {
                localizedText.UpdateText();
            }
        }
    }

}

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationEntry> entries;

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string value;
}

public enum isUI { UI, NOTUI };