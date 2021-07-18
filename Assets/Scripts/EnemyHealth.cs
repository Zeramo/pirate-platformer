using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int initialEnemyHealth = 1;
    private int enemyHealth;
    public float invincibilityDuration = .25f;
    public int goldValue = 1;
    public GameObject goldPile;

    public bool invincible = false;
    public bool rewardsGold = true;
    private int scoreToGive = 0;

    BoxCollider2D[] subBoxColliders;
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        subBoxColliders = GetComponentsInChildren<BoxCollider2D>();
        rigidBody = rigidBody = GetComponent<Rigidbody2D>();

        enemyHealth = initialEnemyHealth;
    }


    public void EnemyTakeDamage(int damage)
    {
        if (!invincible) {

            enemyHealth -= damage;
            invincible = true;
            Invoke("RemoveInvincibility", invincibilityDuration);

            //If enemy health is above 0, do nothing
            if (enemyHealth > 0)
                return;

            //Destroy game object after 0.5 seconds, disable trigger colliders and allow enemy to fall over
            GameManager.AddScore(scoreToGive);
            Invoke(nameof(DestroyEnemy), .5f);
            GameManager.RemoveEnemy();

            foreach (BoxCollider2D collider in subBoxColliders)
            {
                if (collider.isTrigger)
                    collider.enabled = false;
            }
        }
    }

    public void RemoveInvincibility()
    {
        invincible = false;
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
        if (rewardsGold) {
            spawnGold();
        }
    }

    void spawnGold()
    {
        GameObject goldPileInstance = Instantiate(goldPile);
        goldPileInstance.transform.position = gameObject.transform.position + Vector3.up;
        goldPileInstance.GetComponent<goldPileScript>().setGoldValue(goldValue);
    }

    public bool getInvincible() {
        return invincible;
    }

    public void setInvincible(bool invincible) {
        this.invincible = invincible;
    }

    public int getInitialHealth(int score) {
        scoreToGive = score;
        return initialEnemyHealth;
    }

    public int getRemainingHealth() {
        return enemyHealth;
    }

    public void disableGold() {
        rewardsGold = false;
    }
}
