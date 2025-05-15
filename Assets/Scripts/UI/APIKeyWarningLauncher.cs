using System.IO;
using UnityEngine;

public class APIKeyWarningLauncher : MonoBehaviour
{
    [Tooltip("Warning Panel”")]
    public GameObject warningPanel;

    private string envPath;

    void Awake()
    {
        // Send to our default path with the saves. Can change this later if needed for security/compatibility
        envPath = Path.Combine(Application.persistentDataPath, ".env");
    }

    void Start()
    {
        if (warningPanel == null)
        {
            Debug.LogError("APIKeyWarningLauncher: warningPanel not assigned!");
            return;
        }

        // If no env, warn that baby
        bool hasEnv = File.Exists(envPath);
        warningPanel.SetActive(!hasEnv);
    }
}
