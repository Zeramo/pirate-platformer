using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyType;

    private ArrayList spawnPositions = new ArrayList();
    public float spawnTime = 5f;
    public float intervalReduction = .1f;
    public float minSpawnTime = 1f;

    void Start() {

        foreach (Transform spawn in transform) {
            if (spawn != this.transform) {
                spawnPositions.Add(spawn.position);
            }
        }
        Invoke("spawnEnemy", spawnTime);
    }

    void spawnEnemy(){
        GameObject enemyInstance = Instantiate(enemyType);
        int nextSpawnPosition = (int) Mathf.Round(Random.Range(0, spawnPositions.Count));
        enemyInstance.transform.position = (Vector3) spawnPositions[nextSpawnPosition];

        if (spawnTime - intervalReduction >= minSpawnTime) spawnTime -= intervalReduction;
        Invoke("spawnEnemy", spawnTime);
    }
}
