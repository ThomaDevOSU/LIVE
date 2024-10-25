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
        
        // Get input from the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (MenuManager.Instance.isPaused || DialogueManager.Instance.isTalking) // Dont allow input while menu is open or talking
        {
            movement.x = 0;
            movement.y = 0;
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
