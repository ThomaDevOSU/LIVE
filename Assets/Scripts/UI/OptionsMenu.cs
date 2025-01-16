using System;
using TMPro;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown language;

    /// <summary>
    /// When the OptionsMenu first begins, the value assigned is the 
    /// </summary>
    private void OnEnable()
    {
        language.value = (int)Enum.Parse(typeof(LANGUAGES), GameManager.Instance.Options.language);
    }

    /// <summary>
    /// changeLanguage checks the option menus TMP_Dropdown to see what the player has selected, and correspondingly
    /// assigns the language string of the Options object the value based on an enum
    /// </summary>
    public void changeLanguage()
    {
        GameManager.Instance.Options.language = ((LANGUAGES)language.value).ToString();

        GameManager.Instance.updateOptions(); // Update Language
    }

}
