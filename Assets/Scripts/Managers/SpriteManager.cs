using System.Collections;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    public RuntimeAnimatorController[] playerModels;

    private void Awake() // Singleton
    {
        if (Instance == null)
        {
            Instance = this;
            waitForManagers(); // Wait for other managers to load

            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(this);
        }
    }

    IEnumerator waitForManagers() // This function will allow us to reliably wait for other magement system to initialize before we take actions requiring them
    {
        while (!(TransitionManager.Instance && GameManager.Instance && LocalizationManager.Instance)) yield return new WaitForSeconds(0.1f);
    }

    public RuntimeAnimatorController playerAnimator() // Will return animator from list based on chosen sprite
    {
        return playerModels[GameManager.Instance.CurrentPlayerData.playerSprite];
    }

}
