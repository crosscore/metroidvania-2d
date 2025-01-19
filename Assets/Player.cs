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

    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleJumpInput();
        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
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
}
