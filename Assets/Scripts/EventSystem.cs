using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public Transform[] spawnPoint;
    public int enemyCount = 0;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> ActiveEnemies = new List<GameObject>();
    public float spawnCooldown = 2f;
   // public bool cantSpawn;
    private float nextSpawnTime;
    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)//spawn
        {

            SpawnEnemy();
            nextSpawnTime = Time.time + spawnCooldown;
            //level.LevelCheck();

        }
    }
    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, Enemies.Count);
        int randomSpawn = Random.Range(0, spawnPoint.Length);
       
        GameObject enemy = Instantiate(Enemies[randomIndex], spawnPoint[randomSpawn].position, Quaternion.identity);
        
        Enemies.Add(enemy);
        enemyCount++;
    }
}
