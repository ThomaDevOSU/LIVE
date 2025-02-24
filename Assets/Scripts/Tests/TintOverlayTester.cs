using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TintOverlayTester allows testing of the Tint Overlay by adjusting its alpha value using keys 1, 2, and 3.
/// 1 = Fully Transparent (Alpha 0)
/// 2 = Half Transparent (Alpha 0.5)
/// 3 = Fully Opaque (Alpha 1)
/// </summary>
public class TintOverlayTester : MonoBehaviour
{
    [SerializeField] private GameObject tintOverlayPanel;
    private Image tintOverlayImage;

    /// <summary>
    /// Finds the Image component from the TintOverlay Panel at startup.
    /// </summary>
    private void Start()
    {
        if (tintOverlayPanel != null)
        {
            tintOverlayImage = tintOverlayPanel.GetComponent<Image>();
            if (tintOverlayImage == null)
            {
                Debug.LogError("No Image component found on the TintOverlay Panel!");
            }
        }
        else
        {
            Debug.LogError("TintOverlay Panel is not assigned!");
        }
    }

    /// <summary>
    /// Checks for key presses to change the alpha value of the Tint Overlay.
    /// </summary>
    private void Update()
    {
        if (tintOverlayImage == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetOverlayAlpha(0f); // Fully Transparent
            Debug.Log("Tint Overlay Alpha set to 0 (Fully Transparent)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetOverlayAlpha(0.5f); // Half Transparent
            Debug.Log("Tint Overlay Alpha set to 0.5 (Half Transparent)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetOverlayAlpha(1f); // Fully Opaque
            Debug.Log("Tint Overlay Alpha set to 1 (Fully Opaque)");
        }
    }

    /// <summary>
    /// Sets the alpha value of the Tint Overlay's color.
    /// </summary>
    /// <param name="alpha">Alpha value (0 to 1)</param>
    private void SetOverlayAlpha(float alpha)
    {
        Color currentColor = tintOverlayImage.color;
        currentColor.a = alpha;
        tintOverlayImage.color = currentColor;
    }
}
