using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public LayerMask groundLayer, playerLayer;

    int collisionParamID;
    public Animator animator;

    public float explosionTime = .5f;
    public float autoTriggerTime = .5f;
    public float initialLifeTime = .01f;
    private bool collisionTestable = false;
    private bool hasCollided = false;
    private bool playerHasBeenDamaged = false;

    public BoxCollider2D boxColliderBomb;
    public BoxCollider2D boxColliderTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        groundLayer = LayerMask.GetMask("Platforms");
        playerLayer = LayerMask.GetMask("Player");
        collisionParamID = Animator.StringToHash("hasCollided");
        Invoke("TriggerExplosion", autoTriggerTime);
        Invoke("enableCollisionTest", initialLifeTime);
    }

    private void enableCollisionTest() {
        collisionTestable = true;
    }

    //Deletes the bomb
    private void DestroyBomb()
    {
        Destroy(gameObject);
    }

    //triggers explosion animation and behaviour
    private void TriggerExplosion()
    {
        animator.SetBool(collisionParamID, true);
        Invoke("DestroyBomb", explosionTime);
    }

    //checks collision to call TriggerExplosion
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!hasCollided && collisionTestable)
        {
            Debug.Log("bomb has collided");
            TriggerExplosion();
        }
        hasCollided = true;
    }

    //checks player damage once during explosion
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
