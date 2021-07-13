using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goldPile : MonoBehaviour
{
    public int goldValue;
    private float lifeTime = 0f;
    private float minLifeTime = .5f;
    private float maxLifeTime = 15f;

    //public AudioManager audioManager;

    void Start() {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up*10);
    }

    void Update() {
        lifeTime += Time.deltaTime;
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") /*&& lifeTime >= minLifeTime*/ || lifeTime >= maxLifeTime)
        {
            Debug.Log("player has collected gold");
            col.gameObject.GetComponent<movementplayer>().incrementGold(goldValue);

             //plays according audio cue
            //audioManager.Play("goldPickUp");

            Destroy(gameObject);
        }
    }

    public void setGoldValue (int goldValue) {
        this.goldValue = goldValue;
    }
}