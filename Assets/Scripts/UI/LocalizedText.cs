using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string localizationKey; // Key to look up in the JSON file
    public isUI ui; // Whether this is UI or not

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText() // We use TMP in this household
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(localizationKey, ui);
        }
    }
}
