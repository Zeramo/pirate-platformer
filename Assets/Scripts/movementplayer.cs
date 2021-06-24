using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementplayer : MonoBehaviour
{
    public int playerSpeed = 10;
    public int playerJumpPower = 1250;

    public bool facingRight = true;
    
    private float moveX;
    public bool isGrounded;

    BoxCollider2D boxCollider;
    Rigidbody2D rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    void PlayerMove(){
        //Controlls
        moveX = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("Jump") ){ //&& isGrounded == true
            Jump();
        }
        //Animation
        //Player direction
        if(moveX < 0.0f && facingRight == true){
            FlipPlayer();
        }
        else if(moveX > 0.0f && facingRight ==false){
            FlipPlayer();
        }
        //Physics
        rigidBody.velocity = new Vector2 (moveX * playerSpeed, rigidBody.velocity.y);
    }

    void Jump(){
        rigidBody.AddForce (Vector2.up * playerJumpPower);
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
}