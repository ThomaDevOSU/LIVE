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
        TMP_Text textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            if(GameManager.instance.Options.language == "Arabic")
            {
                textComponent.font = LocalizationManager.Instance.fonts[3];
                textComponent.isRightToLeftText = true;
            }
            else if(GameManager.instance.Options.language == "Chinese")
            {
                textComponent.font = LocalizationManager.Instance.fonts[2];
                textComponent.isRightToLeftText = false;
            }
            else if(GameManager.instance.Options.language == "Japanese")
            {
                textComponent.font = LocalizationManager.Instance.fonts[1];
                textComponent.isRightToLeftText = false;
            }
            else
            {
                textComponent.font = LocalizationManager.Instance.fonts[0];
                textComponent.isRightToLeftText = false;
            }

            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(localizationKey, ui);
        }
    }
}
