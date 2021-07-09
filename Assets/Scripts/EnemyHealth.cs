using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealth = 1;
    public int goldValue = 2;
    public GameObject goldPile;
    private bool enemyAlive = true;
    public bool hasBeenDamaged = false;

    BoxCollider2D[] subBoxColliders;
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        subBoxColliders = GetComponentsInChildren<BoxCollider2D>();
        rigidBody = rigidBody = GetComponent<Rigidbody2D>();
    }


    public void EnemyTakeDamage(int damage)
    {

        if (!hasBeenDamaged) {

            enemyHealth -= damage;
            hasBeenDamaged = true;

            //If enemy health is above 0, do nothing
            if (enemyHealth > 0 || !enemyAlive)
                return;

            //Destroy game object after 0.5 seconds, disable trigger colliders and allow enemy to fall over
            enemyAlive = false;
            Invoke(nameof(DestroyEnemy), .5f);
            foreach (BoxCollider2D collider in subBoxColliders)
            {
                if (collider.isTrigger)
                    collider.enabled = false;
            }
            rigidBody.freezeRotation = false;
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
        spawnGold();
    }

    void spawnGold()
    {
        GameObject goldPileInstance = Instantiate(goldPile);
        goldPileInstance.transform.position = gameObject.transform.position + Vector3.up;
        goldPileInstance.GetComponent<goldPile>().setGoldValue(goldValue);
    }

    public bool getHasBeenDamaged() {
        return hasBeenDamaged;
    }

    public void setHasBeenDamaged(bool hasBeenDamaged) {
        this.hasBeenDamaged = hasBeenDamaged;
    }
}
