using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterdeath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D col)
    {
        //When the object the trigger collided with is on the player layer...
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") )
        {
            Debug.Log("Water has detected " + col.gameObject.name);
            //... the player takes damage
            GameManager.PlayerDrowned();
        }
    }
}
