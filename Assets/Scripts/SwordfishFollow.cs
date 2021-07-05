using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SwordfishFollow : MonoBehaviour
{
    public LayerMask groundLayer, playerLayer;

    [Header("Movement Properties")]
    public float enemySpeed = 5f;
    public float enemyJumpForce = 15f;
    public float dashSpeed = 9f;               //Speed of the dash and...
    public float dashDuration = .8f;            //...duration of the dash. Both combined determine the distance
    bool jumpAvailable;

    [Header("Enemy Properties")]
    public float sightRange;                    //If the player is in sight range, the enemy will start moving
    public float attackRange;                   //If the player is in attack range, the enemy will start dashing
    bool playerInSight, playerInRange;
    public int enemyHealth;
    float groundRCLength = .25f;                //Length of the Raycast directed to the ground

    [Header("Attack Properties")]
    public float timeBetweenAttacks = 3f;
    bool hasAttacked;

    private Transform player;
    Vector2 moveDir;

    BoxCollider2D[] boxColliders;
    BoxCollider2D boxColliderBody;
    BoxCollider2D boxColliderSword;

    Rigidbody2D rigidBody;
    float spawnXScale;
    int direction = 1;

    RaycastHit2D obstacle;

    Animator anim;

    int speedXParamID;
    int jumpParamID;

    void Start()
    {
        boxColliders = GetComponentsInChildren<BoxCollider2D>();
        boxColliderBody = boxColliders[0];
        boxColliderSword = boxColliders[1];

        player = GameObject.Find("Player").transform;
        groundLayer = LayerMask.GetMask("Platforms");
        playerLayer = LayerMask.GetMask("Player");

        rigidBody = GetComponent<Rigidbody2D>();
        spawnXScale = transform.localScale.x;

        anim = GetComponent<Animator>();
        speedXParamID = Animator.StringToHash("SpeedX");
        jumpParamID = Animator.StringToHash("isJumping");
    }

    void Update()
    {
        CheckSurroundings();
        Move();
        AnimateSwordfish();
    }

    void CheckSurroundings()
    {
        //Start without a jump
        jumpAvailable = false;

        //Calculate a vector from the enemy's position to the player's
        moveDir = new Vector2(player.position.x - this.transform.position.x, player.position.y - this.transform.position.y);

        //Draw raycast to see if the enemy is considered to be on the ground
        RaycastHit2D onGround = MyRaycast(new Vector2(0f, -(boxColliderBody.size.y/2 - groundRCLength)), Vector2.down, groundRCLength, groundLayer);

        //If enemy is on the ground, it can jump
        jumpAvailable = onGround ? true : false;

        //Draw raycast to see if there is an obstacle the enemy can jump over
        obstacle = MyRaycast(new Vector2(boxColliderBody.size.x / 2 * direction, -.3f), new Vector2(direction, 0f), .5f, groundLayer);
    }

    void Move()
    {
        playerInSight = moveDir.magnitude < sightRange;
        playerInRange = moveDir.magnitude < attackRange;

        if (!playerInSight) Idle();
        if (playerInSight && playerInRange && !hasAttacked && !obstacle) Attack();
        if (playerInSight && !playerInRange && !obstacle) Chase();
        if (obstacle && jumpAvailable) Jump();
    }

    void Chase()
    {
        if (moveDir.x * direction < 0)
            FlipDirection();

        //If the enemy has not attacked within the last [timeBetweenAttacks] seconds, move at normal speed...
        if (!hasAttacked)
            rigidBody.velocity = new Vector2(moveDir.normalized.x * enemySpeed, rigidBody.velocity.y);
        //...otherwise move at a third of the normal speed
        else
            rigidBody.velocity = new Vector2(moveDir.normalized.x * enemySpeed/3, rigidBody.velocity.y);
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

    void Attack()
    {
        //Cancel attack if the player has forced the enemy to change directions
        if (moveDir.x * direction < 0)
        {
            DisableAttack();
            FlipDirection();
            return;
        }
        
        //Move the enemy toward the player using dashSpeed
        Debug.Log("Attacking");
        rigidBody.velocity = new Vector2(moveDir.normalized.x * dashSpeed, rigidBody.velocity.y);

        //After [dashDuration] seconds, disable attacking
        Invoke("DisableAttack", dashDuration);
    }

    void DisableAttack()
    {
        hasAttacked = true;

        //After [timeBetweenAttacks] seconds, enable another attack
        Invoke("EnableAttack", timeBetweenAttacks);
    }

    void EnableAttack()
    {
        hasAttacked = false;
    }

    void Idle()
    {
        //To make sure the enemy doesn't slide on the ground while not actively moving, set speeds to 0
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0) Invoke(nameof(DestroyEnemy), .5f);
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
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

    void AnimateSwordfish()
    {
        anim.SetBool(jumpParamID, !jumpAvailable);
        anim.SetFloat(speedXParamID, Mathf.Abs(rigidBody.velocity.x));
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
