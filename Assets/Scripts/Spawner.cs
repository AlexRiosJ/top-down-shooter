using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpaw;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    void Start () {
        NextWave ();
    }

    void Update () {
        if (enemiesRemainingToSpaw > 0 && Time.time > nextSpawnTime) {
            enemiesRemainingToSpaw--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate (enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath () {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0) {
            NextWave ();
        }
    }

    void NextWave () {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpaw = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpaw;
        }
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount;
        public float timeBetweenSpawns;
    }

}