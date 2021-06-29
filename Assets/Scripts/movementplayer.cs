using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementplayer : MonoBehaviour
{
    public bool drawRaycasts;

    [Header("Physics Forces")]
    public int playerSpeed = 10;
    public int playerJumpPower = 750;

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
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        isGrounded = true;
        doubleJump = false;

        playerHeight = boxCollider.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsCheck();
        PlayerMove();
    }

    void PhysicsCheck()
    {
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
              
    }

    void PlayerMove(){
        //Controlls
        moveX = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("Jump") && isGrounded == true)
        {
            doubleJump = true;
            Jump();
        } else if (Input.GetButtonDown("Jump") && doubleJump)
        {
            doubleJump = false;
            Jump();
        }

        //Animation
        //Player direction
        if (moveX < 0.0f && facingRight == true){
            FlipPlayer();
        }
        else if(moveX > 0.0f && facingRight ==false){
            FlipPlayer();
        }
        //Physics
        rigidBody.velocity = new Vector2 (moveX * playerSpeed, rigidBody.velocity.y);
    }

    void Jump(){
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(Vector2.up * playerJumpPower);
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