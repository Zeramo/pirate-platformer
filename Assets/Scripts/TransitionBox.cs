using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionBox : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject colliderobj;
    public string message = "Head right to reach your ship";
    public bool initArena = false;
    public bool enterArena = false;

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D col)
    {   
        if(initArena){GameManager.SetEnemyNum(9999);}
        if(enterArena){GameManager.SetEnemyNum(20);}
        //When the object the trigger collided with is on the player layer...
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") )
        {
            Debug.Log("Shiptransition detected: " + col.gameObject.name);
            
            GameManager.SetMessage(message);
             colliderobj.SetActive(false);

        }
    }
}
