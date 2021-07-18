using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager current;

    public float deathDuration = 0.1f;                //Duration before scene transition is started
    public int lastLevelNumber = 4;

    Skiff winZoneSkiff;

    int goldCollected;
    int numDeaths;
    bool isGameOver;                                 
    bool playerDead;                                //Is the player currently dead?
    int score;  
    int highScore;

    int numEnemies;                                 //Number of enemies that have to be killed player can exit
    bool allowExit;                                 //Is the player allowed to exit?
    bool doneFading;                                 //Has the scene faded in and out completely?
    string uiMessage;
    [Range(1, 10)]
    public int initialGold = 5;
    public bool debugmode = false;
    //public bool trackLives = false;

    int sceneIndex;                                 //Index of current scene (starting at 0 with the menu)
    Color loadToColor = Color.black;                //Fade color

    private void Awake()
    {
        //There can only be one GameManager. If there is already another one, destroy this iteration...
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        //...otherwise, this is the GameManager
        current = this;
        //Set relevant variables
        current.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        current.score = 0;
        current.highScore = 0;
        current.doneFading = true;
        current.numEnemies = 5;      //Change to 0 if there is a set number of enemies per level
        if(debugmode){goldCollected = initialGold;} // enable for level testing
        
        allowExit = false;

        //The GM should not be destroyed when a new scene is loaded
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        Debug.Log(current.numEnemies);
    }

    public static bool IsGameOver()
    {
        if (current == null)
            return false;

        return current.isGameOver;
    }

    public static void RegisterEnemy()
    {
        //Uncomment, if there is a set number of enemies per level
        //current.numEnemies++;
    }

    public static void RemoveEnemy()
    {
        current.numEnemies--;
        if (current.numEnemies <= 0)
            current.allowExit = true;
    }

    public static bool exitStatus()
    {
        return current.allowExit;
    }

    public static void TotalGoldCount()
    {

    }

    public static void PlayerDied()
    {
        //current.numDeaths++;
        //UIManager.UpdateDeathUI(current.numDeaths);
         if(current.debugmode){
            current.Invoke("RestartScene", current.deathDuration);
            return;
         }
        current.playerDead = true;
         HardCutToScene(current.lastLevelNumber);
    }
        public static void PlayerDrowned()
    {
        //current.numDeaths++;
        //UIManager.UpdateDeathUI(current.numDeaths);

        current.playerDead = true;
        if(current.goldCollected >= 1){
        current.goldCollected = 1;
        current.Invoke("RestartScene", current.deathDuration);
        }else{
            HardCutToScene(current.lastLevelNumber);
        }
        
    }

    public static bool IsPlayerDead()
    {
        return current.playerDead;
    }

    public static void PlayerWon()
    {
        if (current == null)
            return;

        //REMOVE if there is a set number of enemies per level
        current.numEnemies = 5;

        Initiate.Fade("Scene" + (current.sceneIndex + 1), current.loadToColor, 1f);
        current.sceneIndex++;
        current.allowExit = false;
    }

    public void PlayerRespawned()
    {
        current.playerDead = false;
    }

    public static void IncreaseScore(int score)
    {
        current.score += score;
        //if (current.score > current.highScore)
        //    current.highScore = current.score;
        Debug.Log(current.score);
    }

    public static void DecreaseScore(int score)
    {
        if (current.score - score <= 0)
            current.score = 0;
        else
            current.score -= score;
        
        Debug.Log(current.score);
    }

    public static int GetHighScore()
    {
        return current.highScore;
    }
    public static void SetHighScore(int i){
        current.highScore = i;
    }
    public static int GetScore(){
        return current.score;
    }


    public static void EnableNextScene()
    {
        current.doneFading = true;
    }

    void RestartScene()
    {
        current.allowExit = false;

        //REMOVE if there is a set number of enemies per level
        current.numEnemies = 5;

        if(debugmode){current.goldCollected = initialGold;}

        Initiate.Fade("Scene" + current.sceneIndex, current.loadToColor, .5f);
        current.doneFading = false;
        Invoke("PlayerRespawned", 0.5f);
    }

    public static void NextScene()
    {
        if (!current.doneFading)
            return;

        current.allowExit = false;
        Initiate.Fade("Scene" + (current.sceneIndex + 1), current.loadToColor, 1f);

        //REMOVE if there is a set number of enemies per level
        current.numEnemies = 5;

        current.sceneIndex++;
        current.doneFading = false;

        Debug.Log("Sceneindex: " +  current.sceneIndex);      
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static void ToScene(int i)
    {
        current.allowExit = false;
        Initiate.Fade("Scene" + i, current.loadToColor, 1f);

        //REMOVE if there is a set number of enemies per level
        current.numEnemies = 5;

        current.sceneIndex = i;

        current.doneFading = false;
    }

    public static void HardCutToScene(int i)
    {
        //REMOVE if there is a set number of enemies per level
        current.numEnemies = 5;

        current.allowExit = false;
        SceneManager.LoadScene(i);
        current.sceneIndex = i;
    }

    public static void resetCounters(){
        current.score = 0;
        current.goldCollected = current.initialGold;
        current.playerDead = false;
    }
    public static int GetGold(){
        return current.goldCollected;
    }
    public static void SetGold(int i){
        current.goldCollected = i;
        //Debug.Log("Gold:" + current.goldCollected);
    }
    public static void AddGold(int goldValue){
        current.goldCollected += goldValue;
        //Debug.Log("Gold:" + current.goldCollected);
    }
    public static string GetMessage(){
        return current.uiMessage;
    }
    public static void SetMessage(string i){
        current.uiMessage = i;
    }



}
