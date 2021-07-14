using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Sup : MonoBehaviour
{
    
    bool fading = false;


    void Start(){
        GameManager.resetCounters();

    }

    void Update(){
        if(Input.GetButtonDown("Jump") && !fading){
            GameManager.NextScene();
            fading = true;
        }else if(Input.GetButtonDown("Melee")&& !fading)
        {
            GameManager.NextScene();
            fading = true;
        }
    }
}