using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;

    private AudioSource footstepSource;

    private void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = SpriteManager.Instance.playerAnimator();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = AudioLibrary.Instance.footsteps; 
        footstepSource.loop = true;
        footstepSource.outputAudioMixerGroup = AudioManager.Instance.sfxSource.outputAudioMixerGroup;
        footstepSource.playOnAwake = false;
    }

    private void Update()
    {
        movement = InputManager.Instance.GetAction("Move").ReadValue<Vector2>();

        if (MenuManager.Instance.isPaused || DialogueManager.Instance.isTalking)
        {
            movement = Vector2.zero;
        }

        GetComponent<Animator>().SetFloat("Horizontal", movement.x);
        GetComponent<Animator>().SetFloat("Vertical", movement.y);

        HandleFootstepAudio();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void HandleFootstepAudio()
    {
        if (movement.magnitude > 0.1f)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }
}
