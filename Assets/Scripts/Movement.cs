using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bxc;
    public float speed = 5f;
    public float midAirSpeed = 5f;
    public float jumpforce = 15f;
    public float wallJumpForce = 10f;
    public float glideforce = 5f;
    public float dashforce = 5000f;
    public float normalGravity = 4f;
    public float fallGravity = 8f;
    public float dropkickBaseTime = 0.5f;
    public float airslashBaseTime = 0.5f;
    public float checkDirectionBaseTime = 0.2f;
    public float distanceToGround = .5f;
    public float distanceToEnemy = 1.5f;
    public float airSlashingForce = 2000f;
    public float airSlashingThrough = 500f;
    public const float smallAmount = 0.2f;
    private float dropkickTimer;
    private float airslashTimer;
    private float checkDirectionTimer;
    private float moveInput;
    public int extrajumps = 1;   
    public bool isGrounded;
    private bool jumped;
    public bool knockBacked;
    private bool dropKicking;
    private bool airslashing;
    private bool airslashingRight;
    public bool onEnemy;
    public bool onWall;
    public bool touchedWall;
    public bool facingRight;
    public bool changedDirection;
    private bool canJump;
    public LayerMask layerGround;
    public LayerMask layerEnemy;
    public LayerMask layerWall;
    public Transform feetPos;
    public Transform checkEnemy;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bxc = GetComponent<BoxCollider2D>();
        rb.gravityScale = normalGravity;
    }


    void FixedUpdate()
    {
        BasicMovement();
    }
    void Update()
    {
        defaultUpdate();
        Jump();
        glide();
        checkGround();
        gravityUpdate();
        dash();
        dropKick();
        knockBack();
        airSlashing();
        handleWallMovement();
        faceDirection();
    }
    void defaultUpdate()
    {
        if (isGrounded)
        {
            extrajumps = 1;
            knockBacked = false;
            dropKicking = false;
            airslashing = false;
            jumped = false;
            dropkickTimer = dropkickBaseTime;
            airslashTimer = airslashBaseTime; 
        }
        if (!isGrounded)
        {
            dropkickTimer -= Time.deltaTime;
            airslashTimer -= Time.deltaTime;
        }
        if (airslashTimer <= 0)
        {
            airslashing = false;
        }
        if(dropkickTimer <= 0)
        {
            dropKicking = false;
        }
        if (jumped)
        {
            extrajumps = 1;
            jumped = false;
        }
        if (extrajumps > 0)
        {
            canJump = true;
        }
        if (extrajumps <= 0)
        {
            canJump = false;
            if (rb.velocity.x < 0)
            {
                rb.velocity += Vector2.left * midAirSpeed;
            }
            if (rb.velocity.x > 0)
            {
                rb.velocity += Vector2.right * midAirSpeed;
            }
        }
        if (!onWall)
        {
            touchedWall = false;
        }
        if (changedDirection)
        {
            checkDirectionTimer -= Time.deltaTime;
            if (checkDirectionTimer <= 0)
            {
                changedDirection = false;
            }
        }
        
    }
    void BasicMovement()
    {
        if (!onWall)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
          
    }
    void Jump()
    {
        if (!onWall)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                extrajumps--;
                if (canJump)
                {
                    if (extrajumps == 0)
                    {
                        jumped = false;
                    }
                    else
                    {
                        jumped = true;
                    }


                    if (!isGrounded)
                    {
                        rb.velocity = Vector2.up * (+jumpforce * 3 / 4);
                    }

                    else
                    {
                        rb.velocity = Vector2.up * jumpforce;

                    }

                }
            }
        }
    }
    bool checkGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bxc.bounds.center, bxc.bounds.size, 0f, Vector2.down, distanceToGround, layerGround);
        if (raycastHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        RaycastHit2D raycastHitEnemy = Physics2D.BoxCast(bxc.bounds.center, bxc.bounds.size*1.3f, 0f, Vector2.one + Vector2.one *-1, distanceToGround, layerEnemy);
        if (raycastHitEnemy.collider != null)
        {
            onEnemy = true;
        }
        else
        {
            onEnemy = false;
        }
        RaycastHit2D raycastDetectWall = Physics2D.BoxCast(bxc.bounds.center, bxc.bounds.size * 1.3f, 0f, new Vector2(transform.localScale.x,0), distanceToGround, layerWall);
        if (raycastDetectWall.collider != null && !isGrounded)
        {
            onWall = true;
        }
        else
        {
            onWall = false;
        }
        return isGrounded;
    }
    void gravityUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    } //less gravity while jumping up more gravity while falling down
    void glide()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isGrounded)
        {
            switch (moveInput)
            {
                case 1:
                    rb.velocity = Vector2.right * glideforce;
                    break;
                case -1:
                    rb.velocity = Vector2.left * glideforce;
                    break;
                default:
                    if (rb.velocity.y < 0)
                    {
                        rb.velocity = Vector2.down * (glideforce / 10);
                    }
                    break;

            }
        }
    }
    void dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (rb.velocity.x > 0)
            {
                rb.AddForce(transform.right * dashforce);
               
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (rb.velocity.x < 0)
            {
                rb.AddForce(transform.right * dashforce * -1);
            }
        }
    }
    void knockBack()
    {
        if(onEnemy && !dropKicking && !isGrounded && !airslashing)
        {
            knockBacked = true;
            isGrounded = false;
            if (moveInput == 1)
            {
                rb.AddForce(transform.right * 300f * -1);
            }
            if (moveInput == -1)
            {
                rb.AddForce(transform.right * 300f);
            }
            if (moveInput == 0)
            {
                rb.AddForce(transform.right * 300f);
            }
        }
        if (isGrounded && onEnemy)
        {
            if (moveInput == 1)
            {
                rb.AddForce(transform.right * 300f * -1);
            }
            if (moveInput == -1)
            {
                rb.AddForce(transform.right * 300f);
            }
            if (moveInput == 0)
            {
                rb.AddForce(transform.right * 300f);
            }
        }
    }
    void dropKick()
    {
        if (!onWall)
        {
            if ((!canJump || jumped) && !knockBacked)
            {
                if (moveInput == 1 && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.velocity = new Vector3(50, -20, 0);
                    dropKicking = true;
                    knockBacked = false;
                    dropkickTimer = dropkickBaseTime;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }

                }
                if (moveInput == -1 && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.velocity = new Vector3(-50, -20, 0);
                    dropKicking = true;
                    knockBacked = false;
                    dropkickTimer = dropkickBaseTime;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }
                }
                if (moveInput == 0 && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.velocity = new Vector3(0, -20, 0);
                    dropKicking = true;
                    knockBacked = false;
                    dropkickTimer = dropkickBaseTime;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }
                }

            }
            if (rb.velocity.y < 0 && dropKicking && onEnemy)
            {
                rb.velocity = new Vector3(0, 13, 0);
                if (extrajumps <= 0)
                {
                    extrajumps++;
                }
                // dropKicking = false;
                knockBacked = false;
            }
        }
    }
    void airSlashing()
    {
        if (!onWall)
        {
            if (!isGrounded && !knockBacked)
            {
                if (moveInput == 1 && Input.GetMouseButtonDown(0))
                {
                    airslashing = true;
                    airslashingRight = true;
                    airslashTimer = airslashBaseTime;
                    //  rb.AddForce(transform.right * airSlashingForce);
                    rb.AddForce(transform.right * airSlashingForce);
                    knockBacked = false;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }

                }
                if (moveInput == -1 && Input.GetMouseButtonDown(0))
                {
                    airslashing = true;
                    airslashingRight = false;
                    airslashTimer = airslashBaseTime;
                    rb.AddForce(transform.right * airSlashingForce * -1);
                    knockBacked = false;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }
                }
                /*if (moveInput == 0 && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.velocity = new Vector3(0, -20, 0);
                    dropKicking = true;
                    knockBacked = false;
                    dropkickTimer = dropkickBaseTime;
                    if (extrajumps <= 0)
                    {
                        extrajumps++;
                    }
                }*/
                if (airslashing && onEnemy)
                {
                    rb.AddForce(transform.right * airSlashingThrough);
                    if (airslashingRight)
                    {
                        rb.velocity = new Vector3(0, 13, 0);
                        if (extrajumps <= 0)
                        {
                            extrajumps++;
                        }
                    }
                    knockBacked = false;
                }
                if (airslashing && onEnemy)
                {
                    rb.AddForce(transform.right * airSlashingThrough * -1);
                    if (!airslashingRight)
                    {
                        rb.velocity = new Vector3(0, 13, 0);
                        if (extrajumps <= 0)
                        {
                            extrajumps++;
                        }
                    }
                    knockBacked = false;
                }

            }
        }
    }
    void handleWallMovement()
    {
        if (onWall && moveInput !=0)
        {
            rb.gravityScale = 0;
            while (rb.gravityScale <= .3f)
            {
               rb.gravityScale += Time.deltaTime;
            }
            if (!touchedWall)
            {
                rb.velocity = Vector2.zero;
                touchedWall = true;
            }
            if (facingRight)
            {
                if (Input.GetKeyDown(KeyCode.Space) && moveInput != 0 && Input.GetKey(KeyCode.A))
                {
                    rb.gravityScale = normalGravity;
                    rb.velocity = Vector2.one;
                    moveInput = Input.GetAxisRaw("Horizontal");
                    rb.velocity = new Vector2(moveInput * speed, rb.velocity.y * wallJumpForce);
                    facingRight = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) && moveInput != 0 &&  Input.GetKey(KeyCode.D))
                {
                    rb.gravityScale = normalGravity;
                    rb.velocity = Vector2.one;
                    moveInput = Input.GetAxisRaw("Horizontal");
                    rb.velocity = new Vector2(moveInput * speed, rb.velocity.y * wallJumpForce);
                    facingRight = true;
                }
            }

        }
        else
            gravityUpdate();
    }
    public bool faceDirection()
    {
        if (!onWall)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                facingRight = true;
                changedDirection = true;
                checkDirectionTimer = checkDirectionBaseTime;
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                facingRight = false;
                changedDirection = true;
                checkDirectionTimer = checkDirectionBaseTime;
            }
        }
        return facingRight;
    }
}
