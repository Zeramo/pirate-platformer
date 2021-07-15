using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject swordFish;

    /*public float minXPosition = -20f;
    public float maxXPosition = 20f;
    public float yPosition = 2f;*/

    public Vector2[] swordfishSpawnPositions;
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
        int nextSpawnPosition = (int) Mathf.Round(Random.Range(0, swordfishSpawnPositions.Length));
        swordFishInstance.transform.position = swordfishSpawnPositions[nextSpawnPosition];
    }
}
