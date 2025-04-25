using UnityEngine;

public class SceneAudioTrigger : MonoBehaviour
{
    [Header("Scene Audio Settings")]
    public bool playMusic;
    public AudioClip musicClip;

    public bool playAmbient;
    public AudioClip ambientClip;

    private void Start()
    {
        if (playMusic && musicClip != null)
        {
            AudioManager.Instance.PlayMusic(musicClip);
        }

        if (playAmbient && ambientClip != null)
        {
            AudioManager.Instance.PlayAmbient(ambientClip);
        }
    }
}
