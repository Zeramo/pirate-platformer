using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordfishBehavior : MonoBehaviour
{
    public LayerMask groundLayer, playerLayer;

    //Check animator if any of the properties change
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
    public int enemyDamage = 1;
    float groundRCLength = .5f;                //Length of the Raycast directed to the ground
    EnemyHealth health;
    int hp;
    private bool invincible;
    public int scoreOnDeath = 20;

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
    int damagedParamID;

    public AudioManager audioManager;

    void Start()
    {
        boxColliders = GetComponentsInChildren<BoxCollider2D>();
        boxColliderBody = boxColliders[0];
        boxColliderSword = boxColliders[1];

        player = GameObject.Find("Player").transform;
        //groundLayer = LayerMask.GetMask("Platforms");
        playerLayer = LayerMask.GetMask("Player");

        rigidBody = GetComponent<Rigidbody2D>();
        spawnXScale = transform.localScale.x;

        anim = GetComponent<Animator>();
        speedXParamID = Animator.StringToHash("SpeedX");
        jumpParamID = Animator.StringToHash("isJumping");
        damagedParamID = Animator.StringToHash("hasBeenDamaged");

        health = GetComponent<EnemyHealth>();
        hp = health.getInitialHealth(scoreOnDeath);

        GameManager.RegisterEnemy();

        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (GameManager.IsPlayerDead())
        {
            Idle();
            return;
        }

        hp = health.getRemainingHealth();
        updateStun();
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
        RaycastHit2D onGround = MyRaycast(new Vector2(0f, -boxColliderBody.size.y/2 + groundRCLength/2), Vector2.down, groundRCLength, groundLayer);

        //If enemy is on the ground, it can jump
        jumpAvailable = onGround ? true : false;

        //Draw raycast to see if there is an obstacle the enemy can jump over
        obstacle = MyRaycast(new Vector2(boxColliderBody.size.x / 2 * direction, -.3f), new Vector2(direction, 0f), .5f, groundLayer);
    }

    void Move()
    {
        //Calculate if player is in sight range. Line of sight is disregarded, but could be implemented with a raycast
        playerInSight = moveDir.magnitude < sightRange;
        //Calculate if player is in attack range. Same behavior as above
        playerInRange = moveDir.magnitude < attackRange;

        if (hp > 0)
        {
            //If player is not in sight, Idle. Can be replaced by a Patrolling function
            if (!playerInSight || invincible) Idle();
            //If player is in sight, in attack range, the enemy has not attacked, and there is no obstacle, Attack
            if (playerInSight && playerInRange && !hasAttacked && !obstacle && !invincible) {
                Attack();

                //plays according audio cue
                audioManager.Play("Swordfish");
            }
            //If player is in sight, but out of range and there is no obstacle, Chase
            if (playerInSight && !playerInRange && !obstacle && !invincible) Chase();
            //If there is an obstacle, and the enemy can jump, Jump
            if (obstacle && jumpAvailable && !invincible) Jump();
        }
        else Idle();
        
    }

    void Chase()
    {
        //If movement direction and facing direction don't align, flip direction
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
        //To make sure the enemy doesn't slide on the ground while not actively moving, set speed along x-axis to 0
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
        rigidBody.angularVelocity = 0f;
    }

    //This function is called automatically by Unity's physics engine/detector/whatever
    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Enemy has collided with " + col.collider);
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Enemy has collided with Player");
            //Uncomment, when player is supposed to take damage on contact with the enemy.
            //col.gameObject.GetComponent<movementplayer>().PlayerTakeDamage(enemyDamage);
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            //Debug.Log("Enemy has collided with platforms");
        }

    }
    
    //This function is called automatically by Unity's physics engine/detector/whatever
    //The sword of the swordfish is marked as a trigger. When it collides with something, this function is called
    private void OnTriggerEnter2D(Collider2D col)
    {
        //When the object the trigger collided with is on the player layer...
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && hp > 0)
        {
            Debug.Log("A trigger has collided with " + col.gameObject.name);
            //... the player takes damage
            col.gameObject.GetComponent<movementplayer>().PlayerTakeDamage(enemyDamage);
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

    void AnimateSwordfish()
    {
        if (invincible) {
            anim.SetBool(jumpParamID, false);
            anim.SetFloat(speedXParamID, 0);
        } else {
            anim.SetBool(jumpParamID, !jumpAvailable);
            anim.SetFloat(speedXParamID, Mathf.Abs(rigidBody.velocity.x));
        }
        anim.SetBool(damagedParamID, invincible);
    }

    void updateStun() {

        if (!invincible && health.getInvincible()) {
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            rigidBody.AddForce(new Vector2((player.position.x - rigidBody.position.x) * -7.5f, enemyJumpForce*.75f), ForceMode2D.Impulse);

            //plays according audio cue
            audioManager.Play("SwordfishHurt");
        }
        invincible = health.getInvincible();
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
