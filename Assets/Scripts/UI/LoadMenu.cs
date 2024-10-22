using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    public Button[] saveButtons; // Assign buttons in the Inspector
    public Button startButton; // Start game button

    public Button[] trashButtons; // Buttons to prompt deletion
    public Button deleteButton; // Button to delete

    public GameObject deleteChecker; // Checks for deletion confirmation

    public GameObject loadButtons; // Gameobject for load buttons
    public GameObject createCharacter; // GameObject For Character Creator

    private void Awake()
    {
        // Loop through each button and set its behavior based on save files
        for (int i = 0; i < saveButtons.Length; i++)
        {
            int slot = i + 1; // Save slots are 1-indexed
            Button button = saveButtons[i];
            Button trash = trashButtons[i];

            if (File.Exists(SaveSystem.GetSaveFilePath(slot)))
            {
                PlayerData data = SaveSystem.LoadGame(slot); // Load player data to get the name
                button.onClick.AddListener(() => LoadGame(slot));
                button.GetComponentInChildren<TMP_Text>().text = LocalizationManager.Instance.GetLocalizedValue("Load", isUI.UI) + $" {data.name}";

                trash.onClick.AddListener(() => setDelete(slot));
            }
            else
            {
                button.onClick.AddListener(() => CreateChar(slot));
                button.GetComponentInChildren<TMP_Text>().text = LocalizationManager.Instance.GetLocalizedValue("NewGame", isUI.UI);

                trash.gameObject.SetActive(false);
            }
        }
    }

    public void setDelete(int slot) // Brings up the delete checker and sets the event for deletion
    {
        deleteChecker.SetActive(true);
        deleteButton.onClick.AddListener(() => deleteSave(slot));
    }

    public void deleteSave(int slot) // Deletes the save file and resets the button associated with it
    {
        SaveSystem.deleteSave(slot);

        saveButtons[slot].GetComponentInChildren<TMP_Text>().text = LocalizationManager.Instance.GetLocalizedValue("Load", isUI.UI);
        saveButtons[slot].onClick.RemoveAllListeners();
        saveButtons[slot].onClick.AddListener(() => CreateChar(slot));
        trashButtons[slot].gameObject.SetActive(false);
    }

    public void LoadGame(int slot) // Loads the playerdata from the provided save slot and sets our language
    {
        PlayerData data = SaveSystem.LoadGame(slot);
        if (data != null)
        {
            GameManager.instance.CurrentPlayerData = data; // Store loaded data in GameManager
            LocalizationManager.Instance.LoadLocalizedLearningText(GameManager.instance.CurrentPlayerData.language); // Load our language
            // Move player to saved position and load the saved scene
            SceneManager.LoadScene("PlayerHome");
        }
    }

    public void CreateChar(int slot) // I know it's dirty passing the int slot over like this
    {
        startCharacterCreator();
        loadButtons.SetActive(false);
        createCharacter.SetActive(true);
        startButton.onClick.AddListener(() => NewGame(slot));
    }

    public void startCharacterCreator() // Sets default values for playerdaya
    {
        GameManager.instance.CurrentPlayerData.language = "Chinese";
        GameManager.instance.CurrentPlayerData.name = "Bob";
        GameManager.instance.CurrentPlayerData.gender = Gender.male;
        GameManager.instance.CurrentPlayerData.day = 1;
        GameManager.instance.CurrentPlayerData.currency = 0;
        GameManager.instance.CurrentPlayerData.score = 0;
        GameManager.instance.CurrentPlayerData.playerSprite = 0;
    }

    public Image displayedSprite; // Sprite on display
    public Sprite[] sprites; // Array of player sprites
    public TMP_InputField inputName; // Name entered
    public TMP_Dropdown language; // Language chosen

    // This section of the loadmenu will be dedicated to managing the character creator
    public void adjustSprite(int x)  //Adjust Chosen Sprite
    {
        GameManager.instance.CurrentPlayerData.playerSprite += x;
        if (GameManager.instance.CurrentPlayerData.playerSprite < 0)
        {
            GameManager.instance.CurrentPlayerData.playerSprite = sprites.Length - 1;
        }
        else if (GameManager.instance.CurrentPlayerData.playerSprite >= sprites.Length) 
        {
            GameManager.instance.CurrentPlayerData.playerSprite = 0;
        }
        displayedSprite.sprite = sprites[GameManager.instance.CurrentPlayerData.playerSprite];
    }

    public void setGender(int choice) // Gender set via male/female buttons.
    {
        GameManager.instance.CurrentPlayerData.gender = (Gender)choice;
    }

    public void changeName() // Changes the name based on input
    {
        GameManager.instance.CurrentPlayerData.name = inputName.text;
    }

    public void changeLanguage()
    {
        switch (language.value) 
        {
            case 0:
                GameManager.instance.CurrentPlayerData.language = "Chinese";
                break;
            case 1:
                GameManager.instance.CurrentPlayerData.language = "Spanish";
                break;
            case 2:
                GameManager.instance.CurrentPlayerData.language = "English";
                break;
            case 3:
                GameManager.instance.CurrentPlayerData.language = "French";
                break;
            case 4:
                GameManager.instance.CurrentPlayerData.language = "Japanese";
                break;
            case 5:
                GameManager.instance.CurrentPlayerData.language = "German";
                break;
            case 6:
                GameManager.instance.CurrentPlayerData.language = "Italian";
                break;
            case 7:
                GameManager.instance.CurrentPlayerData.language = "Arabic";
                break;
        }
    }


    public void NewGame(int slot) 
    {
        SaveSystem.SaveGame(GameManager.instance.CurrentPlayerData, slot);
        LocalizationManager.Instance.LoadLocalizedLearningText(GameManager.instance.CurrentPlayerData.language); // Load our language

        SceneManager.LoadSceneAsync("PlayerHome");
    }

}
