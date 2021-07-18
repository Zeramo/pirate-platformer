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
    public float initialSpawntime = 5f;
    public bool checkPlayerDistance = true;
    public float maxPlayerDistance = 25f;
    public GameObject player;

    void Start() {

        foreach (Transform spawn in transform) {
            if (spawn != this.transform) {
                spawnPositions.Add((Vector3) spawn.position);
            }
        }
        Invoke("spawnEnemy", initialSpawntime);
    }

    void spawnEnemy(){
        if (!checkPlayerDistance) {

            int nextSpawnPosition = (int) Mathf.Round(Random.Range(0, spawnPositions.Count));
            GameObject enemyInstance = Instantiate(enemyType);
            enemyInstance.transform.position = (Vector3) spawnPositions[nextSpawnPosition];

            if (spawnTime - intervalReduction >= minSpawnTime) spawnTime -= intervalReduction;
        }
        else {
            bool spawnSuccessful = false;
            foreach (Vector3 nextSpawnPosition in spawnPositions) {
                if ((nextSpawnPosition - (Vector3) player.transform.position).magnitude <= maxPlayerDistance) {
                    GameObject enemyInstance = Instantiate(enemyType);
                    enemyInstance.transform.position = nextSpawnPosition;
                    spawnSuccessful = true;
                }
            }

            if (spawnSuccessful && spawnTime - intervalReduction >= minSpawnTime) spawnTime -= intervalReduction;
        }
        Invoke("spawnEnemy", spawnTime);
    }
}
