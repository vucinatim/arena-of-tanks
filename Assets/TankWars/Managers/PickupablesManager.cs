using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupablesManager : Singleton<PickupablesManager>
{
    [Header("Configurations")]
    public GameObject pickupablePrefab;
    public GameObject defaultPickupableVisual;
    public List<PickupableData> pickupables;
    public float spawnInterval = 5f;
    public int maxPickupables = 10;
    public int initialPoolSize = 20;

    private GameObject ground;
    private List<GameObject> spawnedPickupables = new List<GameObject>();
    private Queue<GameObject> pickupablePool = new Queue<GameObject>();
    private float[] probabilities;

    private void Start()
    {
        ground = GameObject.Find("Ground");
        InitializeProbabilities();
        InitializePool();
    }

    private void InitializeProbabilities()
    {
        probabilities = new float[pickupables.Count];
        float totalProbability = 0f;
        foreach (var pickupable in pickupables)
        {
            totalProbability += Constants.rarityWeights[pickupable.rarityType];
        }
        for (int i = 0; i < pickupables.Count; i++)
        {
            probabilities[i] =
                Constants.rarityWeights[pickupables[i].rarityType] / totalProbability;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newPickup = Instantiate(pickupablePrefab);
            newPickup.transform.parent = transform;
            newPickup.SetActive(false);
            pickupablePool.Enqueue(newPickup);
        }
    }

    private GameObject GetPooledPickup()
    {
        if (pickupablePool.Count > 0)
        {
            return pickupablePool.Dequeue();
        }
        else
        {
            return Instantiate(pickupablePrefab);
        }
    }

    private void ReturnToPool(GameObject pickup)
    {
        RemoveVisualFromPickup(pickup);
        pickup.SetActive(false);
        pickupablePool.Enqueue(pickup);
    }

    private IEnumerator SpawnPickupablesContinuously()
    {
        while (true)
        {
            SpawnPickupable();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void StartSpawningPickupables()
    {
        StartCoroutine(SpawnPickupablesContinuously());
    }

    public void StopSpawningPickupables()
    {
        StopAllCoroutines();
    }

    public void SpawnPickupable()
    {
        if (spawnedPickupables.Count >= maxPickupables)
        {
            return;
        }

        if (pickupables.Count == 0 || ground == null)
        {
            Debug.LogError("Either Pickupables list is empty or Ground object not found!");
            return;
        }

        Vector3 randomPosition = GameUtility.GetRandomPointInNavMesh();
        var pickupableInstance = GetPooledPickup();
        pickupableInstance.transform.position = randomPosition;
        pickupableInstance.transform.parent = transform;
        pickupableInstance.SetActive(true);

        PickupableData selectedPickupable = GetRandomPickupableByProbability();
        GameObject visualPrefab = selectedPickupable.GetPickupableVisual();
        GameObject visualInstance = Instantiate(visualPrefab, pickupableInstance.transform);

        pickupableInstance.GetComponent<Pickupable>().SetPickupable(selectedPickupable);

        spawnedPickupables.Add(pickupableInstance);

        // Play spawn sound
        AudioManager.Instance.PlaySFX("PickupSpawned");

        // Play spawn particle effect
        FXManager.Instance.SpawnFX("PickupSpawn", randomPosition, Quaternion.identity, null, 2f);
    }

    private PickupableData GetRandomPickupableByProbability()
    {
        float randomValue = Random.value;
        float sum = 0f;
        int index = 0;
        while (sum < randomValue)
        {
            sum += probabilities[index];
            index++;
        }
        index--;

        return pickupables[index];
    }

    private void RemoveVisualFromPickup(GameObject pickup)
    {
        foreach (Transform child in pickup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DestroyPickupable(GameObject pickupable)
    {
        if (spawnedPickupables.Contains(pickupable))
        {
            spawnedPickupables.Remove(pickupable);
            ReturnToPool(pickupable);
        }
    }

    public void ClearPickupables()
    {
        foreach (var pickupable in spawnedPickupables)
        {
            ReturnToPool(pickupable);
        }
        spawnedPickupables.Clear();
    }
}
