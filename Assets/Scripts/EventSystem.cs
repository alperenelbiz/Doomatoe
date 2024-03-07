using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EventSystem : MonoBehaviour
{
    public Transform playerPos;
    public float spawnRadius = 15f;
    public float minDistance = 5f;
    public Transform[] spawnPoint;
    public int enemyCount = 0;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> ActiveEnemies = new List<GameObject>();
    public float spawnCooldown = 2f;
   // public bool cantSpawn;
    private float nextSpawnTime;

    public List<GameObject> pooledEnemies;

    void Start()
    {
        // for (int i = 0; i < 20; i++)
        // {
        //     GameObject enemy = Instantiate(Enemies[0]);
        //     enemy.SetActive(false);
        //     pooledEnemies.Add(enemy);
        // }
    }

    void Update()
    {
        // if (Time.time >= nextSpawnTime)//spawn
        // {
        //
        //     SpawnEnemy();
        //     nextSpawnTime = Time.time + spawnCooldown;
        //     //level.LevelCheck();
        //
        // }
        
        if (Time.time >= nextSpawnTime)//spawn
        {
            StartCoroutine(GetRandomPointOnNavMesh(SpawnOnMesh));
            nextSpawnTime = Time.time + spawnCooldown;
            //level.LevelCheck();
        }

        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     for (int i = 0; i < 5; i++)
        //     {
        //         StartCoroutine(GetRandomPointOnNavMesh(SpawnOnMesh));
        //     }
        // }
    }
    // private void SpawnEnemy()
    // {
    //     int randomIndex = Random.Range(0, Enemies.Count);
    //     int randomSpawn = Random.Range(0, spawnPoint.Length);
    //    
    //     GameObject enemy = Instantiate(Enemies[randomIndex], spawnPoint[randomSpawn].position, Quaternion.identity);
    //     
    //     Enemies.Add(enemy);
    //     enemyCount++;
    // }

    private void SpawnOnMesh(Vector3 point)
    {
        int randomIndex = Random.Range(0, Enemies.Count);
        GameObject enemy = Instantiate(Enemies[randomIndex], point, Quaternion.identity);
        // GameObject enemy = pooledEnemies[0];
        // enemy.transform.position = point;
        // enemy.transform.rotation = Quaternion.identity;
        // enemy.SetActive(true);
        //
        // pooledEnemies.RemoveAt(0);
        Enemies.Add(enemy);
        enemyCount++;
    }

    private IEnumerator GetRandomPointOnNavMesh(System.Action<Vector3> callback)
    {
        RaycastHit[] hits = new RaycastHit[10];

        int tries = 50;
        int checksPerFrame = 5;
        int checksPerformed = 0;

        for (int i = 0; i < tries; i++)
        {
            //Adding y to z because using circle instead of sphere
            var randomCircle = Random.insideUnitCircle * spawnRadius;
            var randomPoint = new Vector3(playerPos.position.x + randomCircle.x, 5f, playerPos.position.z + randomCircle.y);

            float castDistance = 10f;

            Vector3 castBottom = randomPoint + Vector3.up * minDistance;
            Vector3 castTop = randomPoint + Vector3.up * (minDistance + castDistance);

            int hitCount = Physics.CapsuleCastNonAlloc(castBottom, castTop, 0.1f, Vector3.down, hits, castDistance);

            for (int x = 0; x < hitCount; x++)
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hits[x].point, out navHit, 0.2f, NavMesh.AllAreas))
                {
                    callback(navHit.position);
                    yield break;
                }
            }

            checksPerformed++;

            if (checksPerformed >= checksPerFrame)
            {
                checksPerformed = 0;
                yield return null;
            }
        }
    }
}
