using TMPro;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown language;

    public void Awake()
    {
        language.value = language.options.FindIndex(option => option.text == GameManager.instance.Options.language);
    }

    public void changeLanguage()
    {
        switch (language.value)
        {
            case 0:
                GameManager.instance.Options.language = "Chinese";
                break;
            case 1:
                GameManager.instance.Options.language = "Spanish";
                break;
            case 2:
                GameManager.instance.Options.language = "English";
                break;
            case 3:
                GameManager.instance.Options.language = "French";
                break;
            case 4:
                GameManager.instance.Options.language = "Japanese";
                break;
            case 5:
                GameManager.instance.Options.language = "German";
                break;
            case 6:
                GameManager.instance.Options.language = "Italian";
                break;
            case 7:
                GameManager.instance.Options.language = "Arabic";
                break;
        }

        GameManager.instance.updateOptions(); // Update Language
    }

}
