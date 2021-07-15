using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Sup : MonoBehaviour
{
    void Start(){
        GameManager.resetCounters();

    }

    void Update(){
        if(Input.GetButtonDown("Jump")){
            GameManager.NextScene();
        }else if(Input.GetButtonDown("Melee"))
        {
            GameManager.NextScene();
        }
    }
}