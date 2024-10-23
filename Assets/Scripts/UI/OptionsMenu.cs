using TMPro;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown language;

    public void Awake()
    {
        language.value = language.options.FindIndex(option => option.text == GameManager.Instance.Options.language);
    }

    public void changeLanguage() // God I should've used an enum...
    {
        switch (language.value)
        {
            case 0:
                GameManager.Instance.Options.language = "Chinese";
                break;
            case 1:
                GameManager.Instance.Options.language = "Spanish";
                break;
            case 2:
                GameManager.Instance.Options.language = "English";
                break;
            case 3:
                GameManager.Instance.Options.language = "French";
                break;
            case 4:
                GameManager.Instance.Options.language = "Japanese";
                break;
            case 5:
                GameManager.Instance.Options.language = "German";
                break;
            case 6:
                GameManager.Instance.Options.language = "Italian";
                break;
            case 7:
                GameManager.Instance.Options.language = "Arabic";
                break;
        }

        GameManager.Instance.updateOptions(); // Update Language
    }

}
