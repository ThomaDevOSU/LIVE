using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = SpriteManager.Instance.playerAnimator();
    }

    private void Update()
    {
        movement = InputManager.Instance.GetAction("Move").ReadValue<Vector2>(); // New movement from input manager

        if (MenuManager.Instance.isPaused || DialogueManager.Instance.isTalking) // Dont allow input while menu is open or talking
        {
            movement = Vector2.zero; // vector2 handles the movement/dampaner
        }

        GetComponent<Animator>().SetFloat("Horizontal", movement.x); // Animate player
        GetComponent<Animator>().SetFloat("Vertical", movement.y);
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
