using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rg;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public float jumpForce = 7f;
    public float moveSpeed = 5f;

    // Ground check parameters
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public Transform groundCheckPoint;

    private bool isGrounded;
    private bool canDoubleJump = true;
    private int jumpCount;

    private GameOverManager gameOverManager;
    private bool isDead = false;
    private float deathAnimationDuration = 0.01f;

    [System.Obsolete]
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameOverManager = FindObjectOfType<GameOverManager>();

        // Ensure the animation duration is properly set
        if (animator != null)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name.Contains("Death-Animation"))
                {
                    deathAnimationDuration = clip.length;
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (!isDead)
        {
            HandleJumpInput();
            UpdateAnimationStates();
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            HandleMovement();
            CheckGround();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded || (canDoubleJump && jumpCount < 2))
            {
                rg.linearVelocity = new Vector2(rg.linearVelocity.x, jumpForce);
                jumpCount++;
                isGrounded = false;
                animator.SetBool("isJumping", true);
            }
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rg.linearVelocity = new Vector2(horizontalInput * moveSpeed, rg.linearVelocity.y);

        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }

    private void UpdateAnimationStates()
    {
        bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0;
        animator.SetBool("isRunning", isMoving && isGrounded);
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckDistance, Color.red);

        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null;

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }
    }

    [System.Obsolete]
    public void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Stop all movement
            if (rg != null)
            {
                rg.velocity = Vector2.zero;
                rg.isKinematic = true;
            }

            // Disable collider
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Play death animation if animator exists
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetBool("isDead", true);
            }

            // Start coroutine to show game over after animation
            StartCoroutine(ShowGameOverAfterAnimation());
        }
    }

    [System.Obsolete]
    private IEnumerator ShowGameOverAfterAnimation()
    {
        // Wait for the death animation to complete
        yield return new WaitForSeconds(deathAnimationDuration);

        // Show Game Over screen
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
    }

    [System.Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Needle")
        {
            Die();
        }
    }
}
