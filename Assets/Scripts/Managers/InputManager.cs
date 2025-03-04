using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }   // Singleton variable
    private PlayerInput playerInput;                            // Player input

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerInput = GetComponent<PlayerInput>();  // Make sure to have player input
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public InputAction GetAction(string actionName)
    {
        return playerInput.actions[actionName]; // Get the action
    }
}

