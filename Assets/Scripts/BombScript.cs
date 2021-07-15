using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    int collisionParamID;
    public Animator animator;

    public float explosionTime = .5f;
    private float explosionTimer = .5f;
    private bool hasCollided = false;
    private bool playerHasBeenDamaged = false;

    public BoxCollider2D boxColliderBomb;
    public BoxCollider2D boxColliderTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        collisionParamID = Animator.StringToHash("hasCollided");
        explosionTimer = explosionTime;
    }

    void Update()
    {
        // destroys actor after AOE timer
        if (explosionTimer <= 0) Destroy(gameObject);

        // destroy timer
        if (hasCollided) explosionTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("bomb has collided");
        hasCollided = true;
        animator.SetBool(collisionParamID, hasCollided);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("the player character has been caught in the explosion");

            if (!playerHasBeenDamaged)
            {
                col.gameObject.GetComponent<movementplayer>().PlayerTakeDamage(5);
                playerHasBeenDamaged = true;
            }
        }
    }
}
