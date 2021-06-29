using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementplayer : MonoBehaviour
{
    public bool drawRaycasts;

    [Header("Animator Variables")]
    public Animator animator;
    private bool isDashing = false;
    private bool animationDashing = false;
    private bool isStabbing = false;

    [Header("Physics Forces")]
    public int playerSpeed = 10;
    public int playerJumpPower = 750;
    public int playerDashPower = 10000;

    private float moveX;

    [Header("Player Status")]
    public bool isGrounded;
    public bool doubleJump;                 //Player has another jump available mid-air
    public bool facingRight = true;

    [Header("Collision Checks")]
    public float coyote = .08f;
    public float groundBuffer = .15f;
    public float footOffset = .41f;         //Adjust, if Box Collider X size is changed
    public LayerMask platformsLayer;

    float playerHeight;

    BoxCollider2D boxCollider;
    Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider2d;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();

        isGrounded = true;
        doubleJump = false;

        playerHeight = boxCollider.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJumpOrDash();
        CheckShootOrMelee();
        AnimatePlayer();
    }

    void FixedUpdate()
    {
        GroundedCheck();
         MovementXAxis();
        
    }

    void CheckJumpOrDash(){


        if(Input.GetButtonDown("Dash") && isGrounded == true)
        {
            doubleJump = false;
            Dash();
        } else if (Input.GetButtonDown("Dash") && doubleJump)
        {
            doubleJump = false;
            Dash();
        } else if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            doubleJump = true;
            Jump();
        } else if (Input.GetButtonDown("Jump") && doubleJump)
        {
            doubleJump = false;
            Jump();
        }
    }

    void CheckShootOrMelee(){
        if(Input.GetButtonDown("Melee") && isDashing == false)
        {
            Debug.Log("Melee");
            isStabbing = true;
        } else if (Input.GetButtonDown("Shoot") && isDashing == false)
        {
            Debug.Log("shooting");
            isStabbing = false;
        } else{
            isStabbing = false;
        }
        
    }

    void GroundedCheck()
    {
        /*
        //Default value is false, we want to check every update cycle
        isGrounded = false;

        //Calculate Raycast for both feet, with footOffset and half player height as offset
        RaycastHit2D leftLegHit = MyRaycast(new Vector2(-footOffset, -(playerHeight/2)), Vector2.down, groundBuffer, platformsLayer);
        RaycastHit2D rightLegHit = MyRaycast(new Vector2(footOffset, -(playerHeight/2)), Vector2.down, groundBuffer, platformsLayer);
        
        //If one foot touches the ground, player is considered on the ground
        if (leftLegHit || rightLegHit)
        {
            isGrounded = true;
        }
        */       

        float extraHeightText = 0.51f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, platformsLayer);

        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider2d.bounds.extents.x * 2f), rayColor);

        Debug.Log(raycastHit.collider);
        if(raycastHit.collider != null){
            isGrounded = true;
            animationDashing = false;
        } else{
            isGrounded = false;
        }
              
    }

    void MovementXAxis(){


        if(rigidBody.velocity.x < -10.0 || rigidBody.velocity.x > 10.0 )
        {
            isDashing = false;
            Debug.Log("End Dash");
        }
        if(isDashing == false)  //Move on x Axis if Dash is Over
        {
            //get input speed
            moveX = Input.GetAxis("Horizontal");
            //Player direction update
            if (moveX < 0.0f && facingRight == true){
                FlipPlayer();
            }
            else if(moveX > 0.0f && facingRight ==false){
                FlipPlayer();
            }

            //Physics
            rigidBody.velocity = new Vector2 (moveX * playerSpeed, rigidBody.velocity.y);

        }



    }

    void AnimatePlayer() {
        animator.SetFloat("SpeedX", Mathf.Abs(Input.GetAxis("Horizontal")));
        animator.SetFloat("SpeedY", Mathf.Abs(rigidBody.velocity.y));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isStabbing", isStabbing);
    }

    void Jump(){
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(Vector2.up * playerJumpPower);
    }

    void Dash(){
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;

        if(facingRight == false)
        {
            rigidBody.AddForce(Vector2.left * playerDashPower);
        }else
        {
            rigidBody.AddForce(Vector2.right * playerDashPower);
        }
        isDashing = true;
        animationDashing = true;
        Debug.Log("Start Dash");
    }

    void FlipPlayer(){ // replace once Animations come
        facingRight = !facingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

    }

    void OnCollision2D(Collision2D col){
        Debug.Log("Player has collided with " + col.collider);
        if(col.gameObject.tag == "ground"){
            isGrounded = true;

        }
        
    }

    RaycastHit2D MyRaycast(Vector2 offset, Vector2 direction, float length, LayerMask mask)
    {
        //Get position of player
        Vector2 pos = transform.position;
        //Calculate Raycast hit with offset from pos
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, direction, length, mask);

        //REMOVE after game debugging is finished
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, direction * length, color);
        

        return hit;
    }
}