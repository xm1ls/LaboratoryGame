using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalMove;
    private float verticalMove;
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private bool facingRight = true;
    private bool isJumping = false;
    private bool isCrouching = false;
    public float playerSpeed = 10f;
    public float playerJumpForce = 10f;
    public LayerMask GroundLayer;

    [Range(0, .3f)]
    [SerializeField]
    private float m_MovementSmoothing = .05f;

    private Vector3 m_Velocity = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Start() { }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (!isJumping)
        {
            if (horizontalMove < 0 && facingRight)
            {
                animator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMove));
            }
            else if (horizontalMove > 0 && !facingRight)
            {
                animator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMove));
            }
            else
            {
                animator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMove));
            }
        }

        if (horizontalMove < 0 && facingRight)
        {
            Flip();
        }
        else if (horizontalMove > 0 && !facingRight)
        {
            Flip();
        }

        if (isJumping)
        {
            animator.SetFloat("verticalMovement", 1);

            if (rb.velocity.y < 0)
            {
                animator.SetFloat("verticalMovement", -1);
            }

            if (bc.IsTouchingLayers(GroundLayer.value))
            {
                isJumping = false;
                animator.SetFloat("verticalMovement", 0);
            }
        }

        if(isCrouching)
        {
            animator.SetFloat("Run_Animation_speed", 0.1f);
        }
        if(!isCrouching)
        {
            animator.SetFloat("Run_Animation_speed", 1);
        }
    }

    void FixedUpdate()
    {
        if (verticalMove > 0 && !isJumping)
        {
            rb.velocity = Vector3.SmoothDamp(
                rb.velocity,
                new Vector2(rb.velocity.x, playerJumpForce),
                ref m_Velocity,
                m_MovementSmoothing
            );

            isJumping = true;
            animator.SetFloat("horizontalMovement", 0);
        }

        if (verticalMove < 0)
        {
            isCrouching = true;
        }

        if (verticalMove == 0 || verticalMove > 0)
        {
            isCrouching = false;
        }

        if (horizontalMove != 0)
        {
            if (!isCrouching)
            {
                rb.velocity = Vector3.SmoothDamp(
                    rb.velocity,
                    new Vector2(horizontalMove * playerSpeed, rb.velocity.y),
                    ref m_Velocity,
                    m_MovementSmoothing
                );
            }
            if(isCrouching)
            {
                rb.velocity = Vector3.SmoothDamp(
                    rb.velocity,
                    new Vector2(horizontalMove * playerSpeed / 2, rb.velocity.y),
                    ref m_Velocity,
                    m_MovementSmoothing
                );
            }
        }
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
