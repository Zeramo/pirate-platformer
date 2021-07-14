using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3Sup : MonoBehaviour
{
    bool fading = false;

    void Update(){

        if(Input.GetButtonDown("Jump")&& !fading){
            GameManager.ToScene(1);
            fading = true;
        }
        else if(Input.GetButtonDown("Melee") && !fading){
            GameManager.ToScene(0);
            fading = true;
        }
    }

}
