using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{

    public GameObject treasure1, treasure2, treasure3, treasure4;
    public Text score, gold;
    private int localscore, localgold;

    // Start is called before the first frame update
    void Start()
    {
        //score.text = "100000";
        
    }

    // Update is called once per frame
    void Update()
    {
        localscore = GameManager.GetScore();
        localgold = GameManager.GetGold();
        /*if(gamemanager != null){
        score.text = gamemanager.GetComponent<GameManager>().score.ToString();
        }
        */        
        score.text = localscore.ToString();
        gold.text = localgold.ToString();
        SetGoldIcons(localgold);


    }
    void SetGoldIcons(int i){

        if(i > 4){i = 4;}
        switch(i){
            case 0:
                treasure1.gameObject.SetActive (false);
                treasure2.gameObject.SetActive (false);
                treasure3.gameObject.SetActive (false);
                treasure4.gameObject.SetActive (false);
            break;
            case 1:
                treasure1.gameObject.SetActive (true);
                treasure2.gameObject.SetActive (false);
                treasure3.gameObject.SetActive (false);
                treasure4.gameObject.SetActive (false);
            break;
            case 2:
                treasure1.gameObject.SetActive (true);
                treasure2.gameObject.SetActive (true);
                treasure3.gameObject.SetActive (false);
                treasure4.gameObject.SetActive (false);
            break;
            case 3:
                treasure1.gameObject.SetActive (true);
                treasure2.gameObject.SetActive (true);
                treasure3.gameObject.SetActive (true);
                treasure4.gameObject.SetActive (false);
            break;
            case 4:
            default:
                treasure1.gameObject.SetActive (true);
                treasure2.gameObject.SetActive (true);
                treasure3.gameObject.SetActive (true);
                treasure4.gameObject.SetActive (true);
            break;
        }
    }
}
