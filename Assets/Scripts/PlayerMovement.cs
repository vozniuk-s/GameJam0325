using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private AudioSource movementSound;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    private void Update()
    {
        movement = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            animator.SetBool("inRun", true);
            spriteRenderer.flipX = movement.x < 0.0f;

            if (!movementSound.isPlaying)
                movementSound.Play ();
        }
        else
            animator.SetBool("inRun", false);

        rb.linearVelocity = movement * speed;
    }
}
