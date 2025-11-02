using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : Singleton<LevelManager>
{
    private List<Transform> spawnPoints;

    protected override void Awake()
    {
        base.Awake();
        // Get all SpawnPoints in the scene
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").Select(sp => sp.transform).ToList();
        Debug.Log("Found " + spawnPoints.Count + " spawn points!");
    }

    public void Reset()
    {
        ResetSpawnPoints();
        // Destroy all spawned objects
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Spawned"))
        {
            Destroy(go);
        }

        // Go throught ALL child objects and Re-enable all objects that were deactivated
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

    }

    // This method returns a random spawn point and removes it from the list
    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No more available spawn points!");
            return null;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform chosenSpawnPoint = spawnPoints[randomIndex];
        spawnPoints.RemoveAt(randomIndex);

        Debug.Log("Returning spawn point " + chosenSpawnPoint.name);
        return chosenSpawnPoint;
    }

    // This method is used when the level is reset or a new game is started
    public void ResetSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").Select(sp => sp.transform).ToList();
    }

    public Transform GetFurthestSpawnPoint(List<Transform> transforms, float respawnPlayerDistance = 10f)
    {
        // Get all spawn points that are not currently occupied by other players
        List<Transform> availableSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in spawnPoints)
        {
            bool isOccupied = false;
            foreach (Transform t in transforms)
            {
                if (Vector3.Distance(spawnPoint.position, t.position) < respawnPlayerDistance)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                availableSpawnPoints.Add(spawnPoint);
            }
        }

        // Find the furthest spawn point from all transforms
        Transform furthestSpawnPoint = null;
        float furthestDistance = 0f;
        foreach (Transform spawnPoint in availableSpawnPoints)
        {
            float distance = 0f;
            foreach (Transform t in transforms)
            {
                distance += Vector3.Distance(spawnPoint.position, t.position);
            }
            if (distance > furthestDistance)
            {
                furthestSpawnPoint = spawnPoint;
                furthestDistance = distance;
            }
        }

        return furthestSpawnPoint;
    }


}
