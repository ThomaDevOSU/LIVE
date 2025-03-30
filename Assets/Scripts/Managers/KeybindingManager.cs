using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// For handling rebinding keys in settings
/// </summary>
public class KeybindingManager : MonoBehaviour
{
    public InputActionAsset inputActions; // Reference to InputSystem_Actions
    public GameObject keybindPrefab;
    public Transform keybindContainer;

    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    void Start()
    {
        InitializeKeybinds();
    }

    /// <summary>
    /// Get keybinds
    /// </summary>
    void InitializeKeybinds()
    {
        if (inputActions == null)
        {
            Debug.LogError("inputActions not assigned!");
            return;
        }

        InputActionMap playerMap = inputActions.FindActionMap("Player", true);

        if (playerMap == null)
        {
            Debug.LogError("Player Action not found!");
            return;
        }

        foreach (InputAction action in playerMap)
        {
            CreateKeybindUI(action);
        }
    }

    /// <summary>
    /// For setting up the list of keybinds
    /// </summary>
    /// <param name="action"></param>
    void CreateKeybindUI(InputAction action)
    {
        if (keybindPrefab == null || keybindContainer == null)
        {
            Debug.LogError("keybindPrefab or keybindContainer is missing!");
            return;
        }

        HashSet<string> processedBindings = new HashSet<string>();

        foreach (var binding in action.bindings)
        {
            if (!binding.path.Contains("Keyboard") && !binding.path.Contains("Mouse"))
                continue;

            if (binding.isComposite)
                continue; // Ignore composite groups, this is for move keybinds or others

            string displayName = GetReadableBindingName(binding, action);

            if (processedBindings.Contains(displayName))
                continue;
            processedBindings.Add(displayName);

            CreateBindingUI(action, binding, displayName);
        }
    }

    /// <summary>
    /// Handing rebinding
    /// </summary>
    /// <param name="action"></param>
    /// <param name="binding"></param>
    /// <param name="displayName"></param>
    void CreateBindingUI(InputAction action, InputBinding binding, string displayName)
    {
        GameObject obj = Instantiate(keybindPrefab, keybindContainer);
        TextMeshProUGUI actionNameText = obj.transform.Find("ActionName")?.GetComponent<TextMeshProUGUI>();
        Button rebindButton = obj.transform.Find("RebindButton")?.GetComponent<Button>();
        TextMeshProUGUI keyText = rebindButton.transform.Find("KeyText")?.GetComponent<TextMeshProUGUI>();

        if (actionNameText == null || rebindButton == null || keyText == null)
        {
            Debug.LogError("Keybind elements not found.");
            return;
        }

        actionNameText.text = displayName;

        int bindingIndex = GetBindingIndex(action, binding);
        keyText.text = bindingIndex != -1 ? action.GetBindingDisplayString(bindingIndex) : "Unbound";

        rebindButton.onClick.AddListener(() => StartRebinding(action, binding, keyText));
        uiElements[displayName] = obj;
    }

    /// <summary>
    /// Get the name of the binding. I set this up for move binds, but will likely add others if needed
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private string GetReadableBindingName(InputBinding binding, InputAction action)
    {
        if (binding.isPartOfComposite)
        {
            // Set the binding to lower to help with reading issues
            switch (binding.name.ToLower())
            {
                case "up": return "Up";
                case "down": return "Down";
                case "left": return "Left";
                case "right": return "Right";
            }
        }

        return action.name;
    }


    private int GetBindingIndex(InputAction action, InputBinding binding)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].id == binding.id)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Begin the rebinding
    /// </summary>
    /// <param name="action"></param>
    /// <param name="binding"></param>
    /// <param name="keyText"></param>
    void StartRebinding(InputAction action, InputBinding binding, TextMeshProUGUI keyText)
    {
        int bindingIndex = GetBindingIndex(action, binding);

        if (bindingIndex == -1)
        {
            Debug.LogError($"Binding not found for {binding.name}");
            return;
        }

        keyText.text = "Press any key...";
        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse") // Excluding mouse for now as I don't think we'll ever use it
            .OnComplete(operation =>
            {
                keyText.text = action.GetBindingDisplayString(bindingIndex); // Update UI
                action.Enable();
                operation.Dispose();
            })
            .Start();
    }
}



