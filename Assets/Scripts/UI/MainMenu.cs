using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   

    public void clickPlay() // Reasonably we want to load specific data so this is temporary 
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void clickQuit() // Quits game
    {
        Application.Quit();
    } 
}