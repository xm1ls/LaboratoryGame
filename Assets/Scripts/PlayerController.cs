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
    public Transform fp;
    private bool facingRight = true;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isReadyToShoot = false;
    private bool isShooting = false;
    public float playerSpeed = 10f;
    public float playerJumpForce = 10f;
    public float fireForce = 10f;
    public LayerMask GroundLayer;
    public GameObject projectilePrefab;

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

        if (Input.GetButtonDown("Fire1"))
        {
            if (isReadyToShoot)
            {
                isShooting = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isReadyToShoot = !isReadyToShoot;
        }

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
                // animator.SetFloat("verticalMovement", -1);
            }

            if (bc.IsTouchingLayers(GroundLayer.value))
            {
                isJumping = false;
                animator.SetFloat("verticalMovement", 0);
            }
        }

        if (verticalMove < 0)
        {
            isCrouching = true;
            animator.speed = 0.5f;
        }
        else if (verticalMove == 0 || verticalMove > 0)
        {
            isCrouching = false;
            animator.speed = 1;
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
            if (isCrouching)
            {
                rb.velocity = Vector3.SmoothDamp(
                    rb.velocity,
                    new Vector2(horizontalMove * playerSpeed / 2, rb.velocity.y),
                    ref m_Velocity,
                    m_MovementSmoothing
                );
            }
        }

        if (isReadyToShoot)
        {
            animator.SetBool("IsReadyToShoot", true);
            if (isShooting)
            {
                FireProjectile();
                isShooting = false;
            }
        }
        if (!isReadyToShoot)
        {
            animator.SetBool("IsReadyToShoot", false);
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, fp.position, fp.rotation);

        Vector3 theScale = projectile.transform.localScale;
        theScale.x *= transform.localScale.x / Mathf.Abs(transform.localScale.x);
        projectile.transform.localScale = theScale;

        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        Animator animator = projectile.GetComponent<Animator>();
        // Collider2D circleCollider = projectile.GetComponent<CircleCollider2D>();

        // Debug.Log(circleCollider.IsTouchingLayers(GroundLayer.value));

        rigidbody.AddForce(new Vector2(transform.localScale.x * fireForce, 0), ForceMode2D.Impulse);
        animator.SetBool("PlayOnce", true);

        // if (circleCollider.IsTouchingLayers(GroundLayer.value))
        // {
        //     Destroy(projectile);
        // }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
