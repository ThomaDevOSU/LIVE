using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CompletionPopupUI : MonoBehaviour
{
    public GameObject popupPanel; // PopUpUI 
    public TextMeshProUGUI popupText; // PopUpText

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        popupPanel.SetActive(false);

        if (TaskManager.Instance != null)
        {
            // Use the event Devon added
            TaskManager.Instance.AddTaskCompletionListener(ShowTaskCompletedPopup);
        }
        else
        {
            Debug.LogError("CompletionPopupUI: TaskManager instance is null!");
        }
    }

    public void ShowTaskCompletedPopup(Task completedTask)
    {
        if (completedTask == null) return;

        popupText.text = $"Congratulations! You've completed: {completedTask.T_TaskDescription}";
        popupPanel.SetActive(true);
        Invoke(nameof(HidePopup), 3f); // Setup auto-hide if they don't click X
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

}
