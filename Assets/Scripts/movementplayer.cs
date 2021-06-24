using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementplayer : MonoBehaviour
{
    public int playerSpeed = 10;
    public bool facingRight = true;
    public int playerJumpPower = 1250;
    private float moveX;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        

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
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2 (moveX * playerSpeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    void Jump(){
        GetComponent<Rigidbody2D>().AddForce (Vector2.up * playerJumpPower);
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