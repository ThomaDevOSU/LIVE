using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically adds UISoundTrigger to all buttons in this GameObject and assigns hover/click sounds.
/// Attach this to the root of any prefab or UI canvas that contains buttons.
/// </summary>
public class UISoundAutoBinder : MonoBehaviour
{
    [Header("Optional: Override Sounds (else uses AudioLibrary defaults)")]
    public AudioClip hoverSoundOverride;
    public AudioClip clickSoundOverride;

    private void Awake()
    {
        if (AudioLibrary.Instance == null)
        {
            Debug.LogWarning("UISoundAutoBinder: AudioLibrary not found. Skipping sound binding.");
            return;
        }

        Button[] buttons = GetComponentsInChildren<Button>(true); // true includes inactive children

        foreach (var button in buttons)
        {
            // Check if UISoundTrigger already exists
            UISoundTrigger soundTrigger = button.GetComponent<UISoundTrigger>();
            if (soundTrigger == null)
            {
                soundTrigger = button.gameObject.AddComponent<UISoundTrigger>();
            }

            // Assign sounds if not already set
            if (soundTrigger.hoverSound == null)
            {
                soundTrigger.hoverSound = hoverSoundOverride ?? AudioLibrary.Instance.uiHover;
            }

            if (soundTrigger.clickSound == null)
            {
                soundTrigger.clickSound = clickSoundOverride ?? AudioLibrary.Instance.uiClick;
            }
        }
    }
}
