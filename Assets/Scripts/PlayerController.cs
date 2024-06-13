using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float wallCheckDistence;

    [SerializeField] private float groundCheckRadius;

    private float moveInput = 0f;
    private int facingDirection = 1;

    private bool isRunning;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isWallDectected;
    private bool canWallSlide;
    private bool isWallSliding;
    //private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            canDoubleJump = true;
            canWallSlide = false;
        }
        else if (!isGrounded && rb.velocity.y < 0)
        {
            canWallSlide = true;
        }
        
    }

    void Update()
    {
        CheckInput();

        if(isWallDectected && canWallSlide && !isGrounded)
        {
            isWallSliding = true;
            Move();
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.2f);
        }
        else
        {
            isWallSliding = false;
            Move();
        }
        CollisionCheck();
        FlipController();
        AnimationController();
    }
    

    private void CheckInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
        } 
    }
    
    private void Move()
    {
        rb.velocity = new Vector2(moveSpeed * moveInput, rb.velocity.y);
    }
    
    private void JumpButton()
    {

        if (isGrounded)
        {
            Jump();
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void FlipController()
    { 
        /*
        if(!isGrounded && isWallDectected)
        {
            if(isFacingRight && moveInput < 0)
            {
                Flip();
            }
            else if(!isFacingRight && moveInput > 0)
            {
                Flip();
            }
        }
        */
       
       if (rb.velocity.x > 0 && !isFacingRight)
       {
           Flip();
       }
       else if (rb.velocity.x < 0 && isFacingRight)
       {
           Flip();
       }

        
    }

    private void AnimationController()
    {
        isRunning = rb.velocity.x != 0;

        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void Flip()
    {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    private void CollisionCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isWallDectected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistence, whatIsGround);
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistence, 
                                                        wallCheck.position.y, 
                                                        wallCheck.position.z));
    }
}
