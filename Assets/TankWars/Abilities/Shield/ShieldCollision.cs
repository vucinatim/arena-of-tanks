using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCollision : MonoBehaviour
{

    [SerializeField] string[] _collisionTag;
    float hitTime;
    Material mat;

    void Start()
    {
        if (GetComponent<Renderer>())
        {
            mat = GetComponent<Renderer>().sharedMaterial;
        }

    }

    void Update()
    {

        if (hitTime > 0)
        {
            float myTime = Time.fixedDeltaTime * 1000;
            hitTime -= myTime;
            if (hitTime < 0)
            {
                hitTime = 0;
            }
            mat.SetFloat("_HitTime", hitTime);
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        for (int i = 0; i < _collisionTag.Length; i++)
        {
            if (_collisionTag.Length > 0 && collider.CompareTag(_collisionTag[i]))
            {
                // Get hit position - Note: It won't be as accurate as ContactPoint
                Vector3 hitPosition = collider.ClosestPointOnBounds(transform.position);
                
                // Convert to local space if necessary
                Vector3 localHitPosition = transform.InverseTransformPoint(hitPosition);

                mat.SetVector("_HitPosition", localHitPosition);
                hitTime = 500;
                mat.SetFloat("_HitTime", hitTime);
            }
        }
    }

}

