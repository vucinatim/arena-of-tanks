using System;
using UnityEngine;

[CreateAssetMenu]
public class MineAbility : Ability
{
    public GameObject minePrefab;
    public GameObject fxPrefab;
    public float damage = 60f;
    public AudioClip explodeSound;
    private GameObject parent;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        this.parent = parent;

        var placePoint = parent.transform.Find("PlacePoint");
        var spawnedMine = Instantiate(minePrefab, placePoint.transform.position, placePoint.transform.rotation);
        spawnedMine.GetComponent<CollisionSystem>().OnCollision += OnCollision;
        if (parent.CompareTag("Player")) {
            spawnedMine.GetComponent<Renderer>().material.color = parent.GetComponent<Player>().color;
        }
    }

    private void OnCollision(GameObject mine, GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            CameraShakeEvent.Shake(0.5f, 0.8f);
            var spawnedFX = FXManager.Instance.SpawnFX(fxPrefab, mine.transform.position, mine.transform.rotation, null, 1f);
            var healthSystem = other.GetComponent<HealthSystem>();
            healthSystem.ApplyDamage(parent, damage);
            mine.GetComponent<CollisionSystem>().OnCollision -= OnCollision;
            Destroy(mine);
            if (explodeSound != null)
            {
                AudioManager.Instance.PlaySFX(explodeSound);
            }
        }
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
    }
}
