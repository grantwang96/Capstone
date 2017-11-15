using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public Transform[] enemySpawns;
    public Transform meleeEnemyPrefab;

    Transform player;

    public float spawnTime = 7f;
    public float minSpawnTime = 1f;
    float startTime;

    public bool isSpawning;

	// Use this for initialization
	void Start () {
        Instance = this;
        startTime = Time.time;
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - startTime > spawnTime && isSpawning)
        {
            startTime = Time.time;
            Vector3 loc = enemySpawns[Random.Range(0, enemySpawns.Length)].position;
            Vector3 dir = -loc;
            dir.y = 0;
            Transform newEnemy = Instantiate(meleeEnemyPrefab, loc, Quaternion.LookRotation(dir));
            spawnTime -= Time.deltaTime * 10;
            if(spawnTime < minSpawnTime) { spawnTime = minSpawnTime; }
        }
	}
}
