using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject PlayerMenu;
    public GameObject DialogueMenu;

    public bool isOpen = false;

    private void Awake() // Singleton
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) // Opens or closes the menu
        {
            isOpen = !isOpen;
            PlayerMenu.SetActive(!(PlayerMenu.activeSelf));
        }
    }

}
