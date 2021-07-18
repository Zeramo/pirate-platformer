using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goldPileScript : MonoBehaviour
{
    public int goldValue ;
    private float lifeTime = 3f;

    public AudioManager audioManager;

    void Start() {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up*7);
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
        Invoke("DestroyGold", lifeTime);
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("player has collected gold");
            //col.gameObject.GetComponent<movementplayer>().incrementGold(goldValue);


            //plays according audio cue
            audioManager.Play("goldPickUp");

            GameManager.IncreaseScore(goldValue);
            GameManager.AddGold(goldValue);
            

            DestroyGold();
        }
    }

    public void setGoldValue (int goldValue) {
        this.goldValue = goldValue;
    }

    private void DestroyGold ()
    {
        Destroy(gameObject);
    }
}