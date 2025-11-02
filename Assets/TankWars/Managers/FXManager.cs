using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FXManager : Singleton<FXManager>
{
    [System.Serializable]
    public class FXObject
    {
        public string name;
        public GameObject prefab;
    }

    public List<FXObject> fxObjects;

    private Dictionary<string, GameObject> fxObjectMap = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> instantiatedObjects = new Dictionary<int, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        foreach (FXObject fx in fxObjects)
        {
            fxObjectMap.Add(fx.name, fx.prefab);
        }
    }

    public GameObject SpawnFX(string fxName, Vector3 position, Quaternion rotation, Transform parent = null, float? duration = null, string text = null, Color? color = null)
    {
        if (!fxObjectMap.TryGetValue(fxName, out GameObject prefab))
        {
            Debug.LogError($"FXManager: Could not find FXObject with name {fxName}");
            return null;
        }

        GameObject fxInstance = Instantiate(prefab, position + prefab.transform.position, rotation * prefab.transform.rotation, parent);
        int instanceID = fxInstance?.GetInstanceID() ?? -1;
        if (instanceID == -1)
        {
            Debug.LogError("FXManager: Failed to instantiate FX.");
            return null;
        }

        instantiatedObjects.Add(instanceID, fxInstance);

        if (duration.HasValue)
        {
            Destroy(fxInstance, duration.Value);
        }

        if (text != null)
        {
            TextMeshPro tmp = fxInstance.GetComponent<TextMeshPro>();
            if (tmp)
            {
                tmp.text = text;
                tmp.color = color ?? Color.white;
            }
        }

        return fxInstance;
    }

    public GameObject SpawnFX(GameObject fxPrefab, Vector3 position, Quaternion rotation, Transform parent = null, float? duration = null)
    {
        GameObject fxInstance = Instantiate(fxPrefab, position + fxPrefab.transform.position, rotation * fxPrefab.transform.rotation, parent);
        int instanceID = fxInstance?.GetInstanceID() ?? -1;
        if (instanceID == -1)
        {
            Debug.LogError("FXManager: Failed to instantiate FX.");
            return null;
        }

        instantiatedObjects.Add(instanceID, fxInstance);

        if (duration.HasValue)
        {
            Destroy(fxInstance, duration.Value);
        }

        return fxInstance;
    }


    public void RemoveFX(int instanceID)
    {
        if (instantiatedObjects.TryGetValue(instanceID, out GameObject fxInstance))
        {
            Destroy(fxInstance);
            instantiatedObjects.Remove(instanceID);
        }
    }

    public void RemoveAllFX()
    {
        foreach (var fxInstance in instantiatedObjects.Values)
        {
            Destroy(fxInstance);
        }
        instantiatedObjects.Clear();
    }
}
