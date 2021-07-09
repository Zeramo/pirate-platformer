using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealth = 1;
    public int goldValue = 2;
    public GameObject goldPile;

    BoxCollider2D[] subBoxColliders;
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start(int goldValue)
    {
        subBoxColliders = GetComponentsInChildren<BoxCollider2D>();
        rigidBody = rigidBody = GetComponent<Rigidbody2D>();
        this.goldValue = goldValue;
    }


    public void EnemyTakeDamage(int damage)
    {
        enemyHealth -= damage;

        //If enemy health is above 0, do nothing
        if (enemyHealth > 0)
            return;

        //Destroy game object after 0.5 seconds, disable trigger colliders and allow enemy to fall over
        Invoke(nameof(DestroyEnemy), .5f);
        foreach (BoxCollider2D collider in subBoxColliders)
        {
            if (collider.isTrigger)
                collider.enabled = false;
        }
        rigidBody.freezeRotation = false;
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
        spawnGold();
    }

    void spawnGold()
    {
        GameObject goldPileInstance = Instantiate(goldPile) as GameObject;
        goldPileInstance.transform.position = gameObject.transform.position;
    }
}
