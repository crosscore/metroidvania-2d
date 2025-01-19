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

    [System.Obsolete]
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameOverManager = FindObjectOfType<GameOverManager>();
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

            // アニメーターがある場合は死亡アニメーションを再生
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            // アニメーションの長さに基づいてGameOver表示を遅延実行
            StartCoroutine(ShowGameOverAfterAnimation());

            // GameOverManagerを通じてGame Over表示
            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOver();
            }

            // 必要に応じてプレイヤーの動きを停止
            if (rg != null)
            {
                rg.linearVelocity = Vector2.zero;
                rg.isKinematic = true;
            }

            // コライダーを無効化（オプション）
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }

    [System.Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a Needle object
        if (collision.gameObject.name == "Needle")
        {
            Die();
        }
    }

    [System.Obsolete]
    private IEnumerator ShowGameOverAfterAnimation()
    {
        // アニメーションの再生時間待機（例：0.85秒）
        yield return new WaitForSeconds(0.85f);

        // GameOverManagerを通じてGame Over表示
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }

        // プレイヤーの動きを停止
        if (rg != null)
        {
            rg.linearVelocity = Vector2.zero;
            rg.isKinematic = true;
        }

        // コライダーを無効化
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}
