using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;       // скорость движения
    [SerializeField] private float jumpForce = 10f;  // сила прыжка

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;   // точка под ногами
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;   // Ground + SpearPlatform

    [Header("Anims")]
    [SerializeField] private Animator animator;
    private int moveHash = Animator.StringToHash("IsRunning");
    private int midairHash = Animator.StringToHash("IsMidair");

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // блокировка Z
    }

    private void Update()
    {
        if (!IsOwner) return;
        // Горизонтальное движение
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        animator.SetBool(moveHash, horizontalInput != 0); 

        // Проверка на землю/копья
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool(midairHash, !isGrounded);

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Flip персонажа
        if (horizontalInput > 0.01f) transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
