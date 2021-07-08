using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaUrchinBehaviour : MonoBehaviour
{
    public LayerMask groundLayer, playerLayer;

    //Check animator if any of the properties change
    [Header("Movement Properties")]
    public float enemySpeed = 10f;
    public float enemyJumpForce = 20f;
    bool jumpAvailable;

    [Header("Enemy Properties")]
    public float sightRange;                    //If the player is in sight range, the enemy will start moving
    bool playerInSight;
    public int enemyHealth = 1;
    public int enemyDamage = 1;
    float groundRCLength = .5f;                //Length of the Raycast directed to the ground

    [Header("Attack Properties")]
    private Transform player;
    Vector2 moveDir;

    BoxCollider2D boxCollider;

    Rigidbody2D rigidBody;
    float spawnXScale;
    int direction = -1;

    RaycastHit2D obstacle;

    Animator anim;

    int speedXParamID;
    int jumpParamID;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        player = GameObject.Find("Player").transform;
        groundLayer = LayerMask.GetMask("Platforms");
        playerLayer = LayerMask.GetMask("Player");

        rigidBody = GetComponent<Rigidbody2D>();
        spawnXScale = -transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();
        Move();
    }

    void CheckSurroundings()
    {
        //Start without a jump
        jumpAvailable = false;

        //Calculate a vector from the enemy's position to the player's
        moveDir = new Vector2(player.position.x - this.transform.position.x, player.position.y - this.transform.position.y);

        //Draw raycast to see if the enemy is considered to be on the ground
        RaycastHit2D onGround = MyRaycast(new Vector2(0f, -boxCollider.size.y/2 + groundRCLength/2), Vector2.down, groundRCLength, groundLayer);

        //If enemy is on the ground, it can jump
        jumpAvailable = onGround ? true : false;

        //Draw raycast to see if there is an obstacle the enemy can jump over
        obstacle = MyRaycast(new Vector2(boxCollider.size.x / 2 * direction, .0f), new Vector2(direction, 0f), 1f, groundLayer);
    }

    void Move()
    {
        //Calculate if player is in sight range. Line of sight is disregarded, but could be implemented with a raycast
        playerInSight = moveDir.magnitude < sightRange;

        if (enemyHealth > 0)
        {
            //If player is not in sight, Idle. Can be replaced by a Patrolling function
            if (!playerInSight) Idle();
            //If player is in sight, but out of range and there is no obstacle, Chase
            if (playerInSight && !obstacle) Chase();
            //If there is an obstacle, and the enemy can jump, Jump
            if (obstacle && jumpAvailable) Jump();
        }
        else Idle();
        
    }

    void Chase()
    {
        //If movement direction and facing direction don't align, flip direction
        if (moveDir.x * direction < 0) {
            FlipDirection();
        }

        rigidBody.velocity = new Vector2(moveDir.normalized.x * enemySpeed, rigidBody.velocity.y);
    }

    void Jump()
    {
        //Reduce all speeds to 0
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;

        //ForceMode Impulse to avoid catapult-like jumps that exceed enemyJumpForce.
        //Jump height is calculated differently here, so enemyJumpForce is a lot lower than that of the player.
        rigidBody.AddForce(Vector2.up * enemyJumpForce, ForceMode2D.Impulse);
    }

    void Idle()
    {
        //To make sure the enemy doesn't slide on the ground while not actively moving, set speed along x-axis to 0
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
        rigidBody.angularVelocity = 0f;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Enemy has collided with " + col.collider);
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Enemy has collided with Player, Player damaged");
            col.gameObject.GetComponent<movementplayer>().PlayerTakeDamage(enemyDamage);

        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            //Debug.Log("Enemy has collided with platforms");
        }

    }

    void FlipDirection()
    {
        //Flip direction
        direction *= -1;
        //Get current scale
        Vector3 scale = transform.localScale;
        //Set x scale to scale recorded on spawn times the direction
        scale.x = spawnXScale * direction;
        //Apply scale
        transform.localScale = scale;
    }

    RaycastHit2D MyRaycast(Vector2 offset, Vector2 direction, float length, LayerMask mask)
    {
        //Get position of Character
        Vector2 pos = transform.position;
        //Calculate Raycast hit with offset from pos
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, direction, length, mask);

        //REMOVE after game debugging is finished
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, direction * length, color);

        return hit;
    }
}
