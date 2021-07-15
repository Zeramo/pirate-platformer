using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    int collisionParamID;
    public Animator animator;

    public float explosionTime = .5f;
    private bool hasCollided = false;
    private bool playerHasBeenDamaged = false;

    public BoxCollider2D boxColliderBomb;
    public BoxCollider2D boxColliderTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        collisionParamID = Animator.StringToHash("hasCollided");
    }

    /*void Update()
    {
        if (hasCollided && !animator.GetCurrentAnimatorStateInfo(0).IsName("bomb_explode"))
            DestroyBomb();
    }*/

    private void DestroyBomb()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!hasCollided) 
        {
            Debug.Log("bomb has collided");
            animator.SetBool(collisionParamID, true);
            Invoke("DestroyBomb", explosionTime);
        }
        hasCollided = true;
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
