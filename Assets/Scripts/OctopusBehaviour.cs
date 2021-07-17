using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBehaviour : MonoBehaviour
{
    public LayerMask playerLayer;


    [Header("Enemy Properties")]
    public float minSightRange = 5;
    public float maxSightRange = 100;                    //If the player is in sight range, the enemy will start moving
    bool playerInSight;
    public int enemyDamage = 1;
    EnemyHealth health;
    int hp;
    private bool invincible;
    public float enemyJumpForce;

    [Header("Attack Properties")]
    public float timeBetweenAttacks = 5f;
    public float bombTravelLength = 10f;
    public bool damageOnContact = false;
    bool hasAttacked;
    public GameObject bomb;

    private Transform player;
    Vector2 lookDir;

    BoxCollider2D boxCollider;

    Rigidbody2D rigidBody;
    float spawnXScale;
    int direction = 1;

    Animator anim;
    bool isAttacking = false;

    int throwParamID;
    int damagedParamID;

    public AudioManager audioManager;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponentsInChildren<BoxCollider2D>()[0];

        player = GameObject.Find("Player").transform;
        playerLayer = LayerMask.GetMask("Player");

        spawnXScale = transform.localScale.x;

        anim = GetComponent<Animator>();
        throwParamID = Animator.StringToHash("isThrowing");
        damagedParamID = Animator.StringToHash("hasBeenDamaged");

        health = GetComponent<EnemyHealth>();
        hp = health.getInitialHealth();

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
        CheckForAttack();
        AnimateSwordfish();
    }

    void CheckForAttack()
    {
        //Calculate a vector from the enemy's position to the player's
        lookDir = new Vector2(player.position.x - this.transform.position.x, player.position.y - this.transform.position.y);

        //Calculate if player is in sight range. Line of sight is disregarded, but could be implemented with a raycast
        playerInSight = lookDir.magnitude < maxSightRange && lookDir.magnitude > minSightRange;

        if (hp > 0)
        {
            //If player is not in sight, Idle. Can be replaced by a Patrolling function
            if (!playerInSight || invincible) Idle();
            //If player is in sight, in attack range, the enemy has not attacked, and there is no obstacle, Attack
            if (playerInSight && !hasAttacked && !invincible) {
                Attack();

                //plays according audio cue
                audioManager.Play("Swordfish");
            }
        }
        else Idle();
    }

    void Attack()
    {
        //Cancel attack if the player has forced the enemy to change directions
        if (lookDir.x * direction < 0)
        {
            DisableAttack();
            FlipDirection();
            return;
        }

        isAttacking = true;

        Invoke("ThrowBomb", .5f);

        DisableAttack();
    }

    void ThrowBomb()
    {
        //determine vector via x distance between player and enemy
        float deltaX = (player.position.x - rigidBody.position.x) / 2;
        float deltaY = Mathf.Sqrt(Mathf.Abs(Mathf.Pow((bombTravelLength * .75f), 2) - Mathf.Pow(deltaX, 2))) + (player.position.y - rigidBody.position.y) / 2;
        Vector2 bombVector = new Vector2(deltaX, deltaY);

        //spawn bomb and apply impulse
        GameObject bombInstance = Instantiate(bomb);
        bombInstance.transform.position = rigidBody.position + Vector2.up;
        bombInstance.GetComponent<Rigidbody2D>().AddForce(bombVector, ForceMode2D.Impulse);
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
    //The sword of the swordfish is marked as a trigger. When it collides with something, this function is called
    private void OnCollisionEnter2D(Collision2D col)
    {
        //When the object the trigger collided with is on the player layer...
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && hp > 0 && damageOnContact)
        {
            Debug.Log("the player has collided with " + col.gameObject.name);
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
        anim.SetBool(throwParamID, isAttacking);
        isAttacking = false;
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
}
