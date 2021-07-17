using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene3Sup : MonoBehaviour
{

    public Text score, highscore;
    private int localscore, localhighscore;

    void Start(){
        localscore = GameManager.GetScore();
        localhighscore = GameManager.GetHighScore();
        
        if(localscore > localhighscore){
            GameManager.SetHighScore(localscore);
            highscore.text = localscore.ToString();
        }else{
            highscore.text = localhighscore.ToString();
        }
        score.text = localscore.ToString();

    }
    void Update(){
        

        if(Input.GetKeyDown(KeyCode.Space)){
            GameManager.resetCounters();
            GameManager.ToScene(1);
           
        }
        else if(Input.GetKeyDown(KeyCode.Q)){
            GameManager.ToScene(0);
        }
    }

}
