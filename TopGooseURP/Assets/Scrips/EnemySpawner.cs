using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemy;

    [SerializeField]
    Transform[] spawnPoints;

    int[] spawnPointSpawnAmount;

    [SerializeField]
    float delay;
    float counter;

    [SerializeField]
    int maxSpawnsPerPoint;

    [SerializeField]
    int maxTotalSpawns;

    int totalSpawns;

    private void Start()
    {
        counter = delay;

        spawnPointSpawnAmount = new int[spawnPoints.Length];
    }

    void Update()
    {
        if (totalSpawns >= maxTotalSpawns)
        {
            Debug.Log("Reached max amount of spawns");
            gameObject.SetActive(false);
        }

        counter -= Time.deltaTime;

        if (counter > 0) return;

        //Gammal
        //Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        List<Transform> spawnPointsNotVisible = new List<Transform>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (LookingAt(spawnPoints[i])) continue;
            
            spawnPointsNotVisible.Add(spawnPoints[i]);
        }

        //Choose the spawnpoint with least enemies attached to it.
        Transform spawnPoint = spawnPointsNotVisible[0];
        for (int i = 1; i < spawnPointsNotVisible.Count; i++)
        {
            if (spawnPointsNotVisible[i].childCount < spawnPoint.childCount)
            {
                if (spawnPointSpawnAmount[i] > maxSpawnsPerPoint) continue;
                spawnPoint = spawnPointsNotVisible[i];
            }
        }
        

        //If it still has too many enemies, don't spawn at all.
        if (spawnPoint.childCount >= maxSpawnsPerPoint) return;

        Instantiate(enemy, spawnPoint.position, Quaternion.identity, spawnPoint);

        int spawnPointIndex = 0;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == spawnPoint)
            {
                spawnPointIndex = i;
                break;
            }
        }

        Debug.Log($"Spawned new enemy at spawnpoint {spawnPointIndex}");

        counter = delay;
        totalSpawns++;
        spawnPointSpawnAmount[spawnPointIndex]++;
    }

    private bool LookingAt(Transform Object)
    {
        if (Vector3.Dot(Camera.main.transform.forward, (Object.position - Camera.main.transform.position).normalized) > 0.5f) return true;
        else return false;
    }
}
