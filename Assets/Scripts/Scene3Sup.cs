using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene3Sup : MonoBehaviour
{
    void Update(){

        if(Input.GetKeyDown(KeyCode.Space)){
            GameManager.ToScene(1);
        }
        else if(Input.GetKeyDown(KeyCode.Q)){
            GameManager.ToScene(0);
        }
    }

}
