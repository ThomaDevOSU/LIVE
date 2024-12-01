using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCanvas;

    public void clickPlay() // Reasonably we want to load specific data so this is temporary 
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void clickQuit() // Quits game
    {
        Application.Quit();
    }

    public void QuitToMainMenu() // Quits to main menu
    {

        if (menuCanvas != null) {menuCanvas.SetActive(false);}
        TransitionManager.Instance.StartTransition("MainMenu", TransitionType.FADE);
    }
}
