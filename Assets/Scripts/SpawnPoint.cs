using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

public class SpawnPoint : MonoBehaviour
{
    private GameObject point, potion;
    public float spawnHeight = 0.5f;
    private int totalSpawnPoint = 100;
    private int totalSpawnPotion = 10;
    private float maxDistance = 30f;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectsToSpawn();

        SpawnPoints(totalSpawnPoint);
        SpawnPotions(totalSpawnPotion);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FindObjectsToSpawn()
    {
        // Get all assets in the project that are of type GameObject
        GameObject[] assetObjects = Resources.LoadAll<GameObject>("Spawn/");

        foreach (GameObject assetObject in assetObjects)
        {
            // Check if the assetObject has the desired script attached to it
            if (assetObject.GetComponent("PointScript") != null)
            {
                point = assetObject;
            }
            if (assetObject.GetComponent("PotionScript") != null)
            {
                potion = assetObject;
            }
        }
    }

    private void SpawnPoints(int totalSpawnPoint)
    {
        // spawn all points
        for (int pointIndex = 0; pointIndex < totalSpawnPoint; pointIndex++)
        {
            // get a location
            Vector3 randomLocation = GetRandomLocation(maxDistance, gameObject);

            // spawn a random point prefab at that location
            GameObject spawnedPoint = SpawnObjectAtLocation(point, randomLocation, spawnHeight);
        }
    }

    private void SpawnPotions(int totalSpawnPotion)
    {
        // spawn all potions
        for (int potionIndex = 0; potionIndex < totalSpawnPotion; potionIndex++)
        {
            // get a location
            Vector3 randomLocation = GetRandomLocation(maxDistance, gameObject);

            // spawn a random potion prefab at that location
            GameObject spawnedPotion = SpawnObjectAtLocation(potion, randomLocation, spawnHeight);
        }
    }

    private GameObject SpawnObjectAtLocation(GameObject objectToSpawn, Vector3 spawnPosition, float spawnHeight)
    {
        // spawn the object at the current spawn location
        GameObject spawnGameObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        spawnGameObject.transform.Translate(0, spawnHeight, 0);

        return spawnGameObject;
    }

    public static Vector3 GetRandomLocation(float maxDistance, GameObject centerPosition)
    {
        // Get Random Location inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + centerPosition.transform.position;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest location on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }
}
