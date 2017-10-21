using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Transform[] enemySpawns;
    public Transform meleeEnemyPrefab;

    float spawnTime = 7f;
    float minSpawnTime = 1f;
    float startTime;

    public bool isSpawning;

	// Use this for initialization
	void Start () {
        Instance = this;
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime > spawnTime && isSpawning)
        {
            startTime = Time.time;
            Transform newEnemy = Instantiate(meleeEnemyPrefab, enemySpawns[Random.Range(0, enemySpawns.Length)].position, Quaternion.identity);
            spawnTime -= Time.deltaTime * 10;
            if(spawnTime < minSpawnTime) { spawnTime = minSpawnTime; }
        }
	}
}
