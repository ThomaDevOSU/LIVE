using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject PlayerMenu;

    public bool isPaused = false;

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
        if (Input.GetButtonDown("Cancel") && !DialogueManager.Instance.isTalking) // Opens or closes the menu if not talking
        {
            isPaused = !isPaused;
            PlayerMenu.SetActive(!(PlayerMenu.activeSelf));
        }
    }

    public void unPause() // This is for the button
    {
        PlayerMenu.SetActive(!(PlayerMenu.activeSelf));
        isPaused = !isPaused;
    }

}
