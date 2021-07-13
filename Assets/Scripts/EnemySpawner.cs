using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject swordFish;
    public float minXPosition = -20f;
    public float maxXPosition = 20f;
    public float yPosition = 2f;
    public float spawnTime = 5f;
    private float spawnTimer;

    void Start() {
        spawnTimer = spawnTime;
    }

    void Update() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0) {
            spawnEnemy();
            if (spawnTimer > 1) spawnTimer -= .2f;
            spawnTimer = spawnTime;
        }
    }

    void spawnEnemy(){
        GameObject swordFishInstance = Instantiate(swordFish);
        float spawnXPosition = Random.Range(minXPosition, maxXPosition);
        swordFishInstance.transform.position = new Vector2(spawnXPosition, yPosition); 
    }
}
