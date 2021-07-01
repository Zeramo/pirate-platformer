using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordfishFollow : MonoBehaviour
{
    public LayerMask groundLayer, playerLayer;

    [Header("Movement Properties")]
    public float enemySpeed = 3f;
    public float enemyJumpForce = 500f;
    public float dashSpeed = 12f;               //Speed of the dash and...
    public float dashDuration = .8f;            //...duration of the dash. Both combined determine the distance

    [Header("Enemy Properties")]
    public float sightRange;
    public float attackRange;
    bool playerInSight, playerInRange;
    public int enemyHealth;

    [Header("Attack Properties")]
    public float timeBetweenAttacks = 3f;
    bool hasAttacked;

    private Transform player;
    Vector2 moveDir;

    Rigidbody2D rigidBody;
    float spawnXScale;
    int direction = 1;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        groundLayer = LayerMask.GetMask("Platforms");
        playerLayer = LayerMask.GetMask("Player");

        rigidBody = GetComponent<Rigidbody2D>();
        spawnXScale = transform.localScale.x;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        moveDir = new Vector2(player.position.x - this.transform.position.x, player.position.y - this.transform.position.y);
        playerInSight = moveDir.magnitude < sightRange;
        playerInRange = moveDir.magnitude < attackRange;

        if (playerInSight && playerInRange && !hasAttacked) Attack();
        if (playerInSight && !playerInRange) Chase();
    }

    void Chase()
    {
        if (moveDir.x * direction < 0)
            FlipDirection();

        //If the enemy has not attacked within the last [timeBetweenAttacks] seconds, move at normal speed...
        if (!hasAttacked)
            rigidBody.velocity = new Vector2(moveDir.normalized.x * enemySpeed, rigidBody.velocity.y);
        //...otherwise move at half speed
        else
            rigidBody.velocity = new Vector2(moveDir.normalized.x * enemySpeed/2, rigidBody.velocity.y);
    }

    void Jump()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(Vector2.up * enemyJumpForce);
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
            

        Debug.Log("Attacking");
        rigidBody.velocity = new Vector2(moveDir.normalized.x * dashSpeed, rigidBody.velocity.y);

        Invoke("DisableAttack", dashDuration);
    }

    void DisableAttack()
    {
        hasAttacked = true;
        Invoke("EnableAttack", timeBetweenAttacks);
    }

    void EnableAttack()
    {
        hasAttacked = false;
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
}
