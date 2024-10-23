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
        GetComponent<Animator>().SetFloat("Horizontal", movement.x);
        GetComponent<Animator>().SetFloat("Vertical", movement.y);
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

}
