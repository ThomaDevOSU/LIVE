using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    public Animator[] animators;
    public Animator animator;

    bool transitioning = false; // Stops excessive transition requests

    public void Awake() // Singleton Setup
    {
        if (Instance == null)
        {
            Instance = this;
            waitForManagers(); // Wait for other managers to load


            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    IEnumerator waitForManagers() // This function will allow us to reliably wait for other magement system to initialize before we take actions requiring them
    {
        while (!(GameManager.Instance && SpriteManager.Instance && LocalizationManager.Instance)) yield return new WaitForSeconds(0.1f);
    }

    public void StartTransition(string sceneName, TransitionType transition)
    {
        if (transitioning == false) 
        {
            if (NPCManager.Instance != null)
            {
                NPCManager.Instance.StoreConversationHistory();
            }
            else
            {
                Debug.LogWarning("NPCManager is not present in the current scene.");
            }
            transitioning = true;
            StartCoroutine(TransitionRoutine(sceneName, transition));
        }
    }

    private IEnumerator TransitionRoutine(string sceneName, TransitionType transition)
    {
        animator.gameObject.SetActive(false); // What animator we have to false
        animator = animators[(int)transition]; // Set what animator we are using
        animator.gameObject.SetActive(true); // Set new animator to true

        animator.SetBool("start", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        

        // Load the new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        yield return new WaitForEndOfFrame(); // Wait for end of frame 
        animator.SetBool("start", false);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        transitioning = false;
    }

}

public enum TransitionType { FADE, WIPE };