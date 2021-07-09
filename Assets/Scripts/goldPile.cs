using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goldPile : MonoBehaviour
{
    public int goldAmount;
    private float lifeTime = 0f;
    private float minLifeTime = .5f;
    private float maxLifeTime = 15f;

    void Start(int goldAmount) {
        this.goldAmount = goldAmount;
    }

    void Update() {
        lifeTime += Time.deltaTime;
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") && lifeTime >= minLifeTime || lifeTime >= maxLifeTime)
        {
            Debug.Log("player has collected gold");
            col.gameObject.GetComponent<movementplayer>().incrementGold(goldAmount);
            Destroy(gameObject);
        }
    }
}