using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class LocalizedText : MonoBehaviour
{
    public string localizationKey; // Key to look up in the JSON file
    public isUI ui; // Whether this is UI or not

    private void Start()
    {
        //UpdateText();
    }

    public void UpdateText() // We use TMP in this household
    {
        TMP_Text textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.font = LocalizationManager.Instance.MasterFont;

        if (textComponent != null)  //  This component is not null
        {
            LANGUAGES lan = (LANGUAGES)Enum.Parse(typeof(LANGUAGES), GameManager.Instance.Options.language);

            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(localizationKey, ui);

            if (lan == LANGUAGES.ar)   //  If the language requires right to left reading
            {
                //textComponent.isRightToLeftText = true;
                textComponent.text = ArabicFixer.Fix(textComponent.text);
            }
            else // Otherwise
            {
                textComponent.isRightToLeftText = false;
            }

            //SECTION FOR DETERMINING BOLDNESS, ANY NON LATIN LANGUAGE SHOULD BE BOLD
            if (lan == LANGUAGES.zh || lan == LANGUAGES.ja || lan == LANGUAGES.ar) // Make font bold if not english
            {
                textComponent.fontStyle = FontStyles.Bold;
            }
            else 
            {
                textComponent.fontStyle = FontStyles.Normal;
            }
        }
    }
}
