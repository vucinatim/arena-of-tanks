using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public CorpseData corpseData;

    void Awake()
    {
        var destructSystem = GetComponent<DestructSystem>();
        destructSystem.Initialize(corpseData.destructData);
        destructSystem.SelfDestruct();

        destructSystem.OnDestruct += OnDestruct;
    }

    void OnDestruct()
    {
        Debug.Log("CorpseController.OnDestruct");
    }

}
