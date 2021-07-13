using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager current;

    public float deathDuration = 2f;

    Skiff winZoneSkiff;

    int goldCollected;
    int numDeaths;
    bool isGameOver;
    bool playerDead;
    int score;
    int highScore;

    int numEnemies;
    public bool allowExit;

    int sceneIndex;
    Color loadToColor = Color.black;
    // Start is called before the first frame update
    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        current.sceneIndex = 1;
        current.score = 0;
        current.highScore = 0;
        numEnemies = 0;

        allowExit = false;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return;

    }

    public static bool IsGameOver()
    {
        if (current == null)
            return false;

        return current.isGameOver;
    }

    public static void RegisterSkiff(Skiff skiff)
    {
        if (current == null)
            return;

        current.winZoneSkiff = skiff;
    }

    public static void RegisterEnemy()
    {
        current.numEnemies++;
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

        current.playerDead = true;
        current.Invoke("RestartScene", current.deathDuration);
    }

    public static bool IsPlayerDead()
    {
        return current.playerDead;
    }

    public static void PlayerWon()
    {
        if (current == null)
            return;

        Initiate.Fade("Level" + (current.sceneIndex + 1), current.loadToColor, 1f);
        current.sceneIndex++;
    }

    public void PlayerRespawned()
    {
        current.playerDead = false;
    }

    public static void IncreaseScore(int score)
    {
        current.score += score;
        if (current.score > current.highScore)
            current.highScore = current.score;
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

    void RestartScene()
    {
        Initiate.Fade("Level" + current.sceneIndex, current.loadToColor, .5f);
        Invoke("PlayerRespawned", 2f);
    }
}
