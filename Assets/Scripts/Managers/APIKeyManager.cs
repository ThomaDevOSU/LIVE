using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;

public class APIKeyManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField apiKeyInput;

    [Header("Save Popup")]
    public GameObject      popupPanel;
    public TextMeshProUGUI popupText;
    public float           popupDuration = 2f;
    private string envPath;

    void Awake()
    {
        envPath = Path.Combine(Application.persistentDataPath, ".env");

        // Load the env if exists
        if (File.Exists(envPath))
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                if (line.StartsWith("OPENAI_API_KEY="))
                {
                    var key = line.Substring("OPENAI_API_KEY=".Length).Trim();
                    apiKeyInput.text = key;
                    APIKeyStore.OpenAIKey = key;  // <-- Store globally
                    break;
                }
            }
        }

        apiKeyInput.onEndEdit.AddListener(OnApiKeyChanged);

        // Setup popup hidden
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    private void OnApiKeyChanged(string newKey)
    {
        var trimmed = newKey.Trim();
        APIKeyStore.OpenAIKey = trimmed;

        try
        {
            File.WriteAllText(envPath, $"OPENAI_API_KEY={trimmed}");
            ShowPopup("API Key Saved Successfully!");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to write API key: {e}");
            ShowPopup("Failed To Save API Key!");
        }
    }

    private void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null) return;

        popupText.text     = message;
        popupPanel.SetActive(true);

        // Cancel anything else to not bug, kept getting issues
        StopAllCoroutines();
        StartCoroutine(HidePopupAfterDelay());
    }

    // expose the key globally
    public static class APIKeyStore
    {
        public static string OpenAIKey { get; set; }
    }


    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        popupPanel.SetActive(false);
    }
}
