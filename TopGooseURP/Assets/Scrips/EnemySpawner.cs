using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //The enemy to spawn
    [SerializeField]
    GameObject enemy;

    //The enemies will be spawned as children to these points, to keep track of which spawner that has active enemies
    [SerializeField]
    Transform[] spawnPoints;

    //Delay between spawns
    [SerializeField]
    float delay;
    float counter;

    //Amount of enemies to exist at a time that are assigned to a certain spawn
    [SerializeField]
    int enemiesPerPoint;

    //Max amount of enemies to spawn in the game
    [SerializeField]
    int maxTotalSpawns;
    int totalSpawns;

    //How far you have to be from a point for it to be able to spawn an enemy
    [SerializeField]
    int minDistance;

    private void Start()
    {
        counter = delay;
    }

    void Update()
    {
        if (totalSpawns >= maxTotalSpawns)
        {
            gameObject.SetActive(false);
        }

        counter -= Time.deltaTime;

        if (counter > 0) return;

        //Get a list of spawn points that player isn't looking at and that are out of a certain range from the player
        List<Transform> spawnPointsNotVisible = new List<Transform>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (LookingAt(spawnPoints[i]) || Vector3.Distance(spawnPoints[i].position, Camera.main.transform.position) < minDistance) continue;
            
            spawnPointsNotVisible.Add(spawnPoints[i]);
        }

        //Return if there is no desirable spawnpoint
        if (spawnPointsNotVisible.Count == 0) return;

        //Choose the spawnpoint with least enemies attached to it
        Transform spawnPoint = spawnPointsNotVisible[0];
        for (int i = 1; i < spawnPointsNotVisible.Count; i++)
        {
            if (spawnPointsNotVisible[i].childCount < spawnPoint.childCount)
            {
                if (spawnPointsNotVisible[i].childCount > enemiesPerPoint) continue;
                spawnPoint = spawnPointsNotVisible[i];
            }
        }

        //If it still has too many enemies, don't spawn at all
        if (spawnPoint.childCount >= enemiesPerPoint) return;

        //Spawn the enemy and set its path to the path that is assigned to the chosen spawnpoint
        var enemyInstance = Instantiate(enemy, spawnPoint.position, Quaternion.identity, spawnPoint);
        enemyInstance.GetComponent<PathUtility>().currentPath = spawnPoint.GetComponentInChildren<WaypointsPath>();

        counter = delay;
        totalSpawns++;
    }

    private bool LookingAt(Transform Object)
    {
        if (Vector3.Dot(Camera.main.transform.forward, (Object.position - Camera.main.transform.position).normalized) > 0.5f) return true;
        else return false;
    }
}
