using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpaw;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThreasholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool playerIsCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start () {
        playerEntity = FindObjectOfType<Player> ();
        playerT = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;

        map = FindObjectOfType<MapGenerator> ();
        NextWave ();
    }

    void Update () {
        if (!isDisabled) {
            if (Time.time > nextCampCheckTime) {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                playerIsCamping = (Vector3.Distance (playerT.position, campPositionOld) < campThreasholdDistance);
                campPositionOld = playerT.position;
            }

            if (enemiesRemainingToSpaw > 0 && Time.time > nextSpawnTime) {
                enemiesRemainingToSpaw--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine (SpawnEnemy ());
            }
        }
    }

    IEnumerator SpawnEnemy () {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        Transform spawnTile = map.GetRandomOpenTile ();
        if (playerIsCamping) {
            spawnTile = map.GetTileFromPosition (playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer> ().material;
        Color originalColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay) {
            tileMat.color = Color.Lerp (originalColor, flashColor, Mathf.PingPong (spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate (enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath () {
        isDisabled = true;
    }

    void OnEnemyDeath () {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0) {
            NextWave ();
        }
    }

    void ResetPlayerPosition () {
        playerT.position = map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave () {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpaw = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpaw;

            if (OnNewWave != null) {
                OnNewWave (currentWaveNumber);
            }
            ResetPlayerPosition ();
        }
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount;
        public float timeBetweenSpawns;
    }

}