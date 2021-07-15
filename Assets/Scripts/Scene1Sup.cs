using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Sup : MonoBehaviour
{
    void Start(){
        GameManager.resetCounters();

    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            GameManager.NextScene();
        }else if(Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.NextScene();
        }
    }
}