using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Collider2D circleCollider;
    private Rigidbody2D rb;
    private Animator animator;
    public LayerMask GroundLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (circleCollider.IsTouchingLayers(GroundLayer.value))
        {
            rb.velocity = Vector2.zero;
            animator.speed = 3;
            animator.SetBool("IsHit", true);
            Destroy(gameObject, .2f);
        }
    }
}
