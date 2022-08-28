using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 4.0f;
    private float inputDirtection;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isRunning;
    private bool isFacingRight = true;

    public float jumpForce = 8.0f;
    public float fallMultiplier;
    public float lowJumpMultiplier;

    public int availableJumps = 1;
    private int jumpsLeft;

    private bool canJump;
       
    public Transform groundCheck;
    public LayerMask groundType;
    public float groundCircle;
    private bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpsLeft = availableJumps;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckDirection();
        UpdateAnimation();
        
    }

    private void FixedUpdate()
    {
        Move();
        CheckEnvironment();
        CheckJump();

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void CheckInput()
    {
        inputDirtection = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }
    }

    private void CheckJump()
    {
        if(isGrounded && rb.velocity.y <= 3)
        {
            jumpsLeft = availableJumps;
        }
        if(jumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDirtection * speed, rb.velocity.y);
    }

    private void CheckDirection()
    {
        if(isFacingRight && inputDirtection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && inputDirtection > 0)
        {
            Flip();
        }

        if (rb.velocity.x <= -0.5f || rb.velocity.x >= 0.5f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void UpdateAnimation()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void CheckEnvironment()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCircle,groundType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCircle);
    }

}
