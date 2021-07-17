using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene1Sup : MonoBehaviour
{
    public Text highscore;
    private int localhighscore;

    void Start(){
        GameManager.resetCounters();
        localhighscore = GameManager.GetHighScore();
        highscore.text = "Highscore :" + localhighscore.ToString();

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