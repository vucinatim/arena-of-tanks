using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LaserAbility : Ability
{
    [SerializeField] private GameObject triggerFX;
    [SerializeField] private Material beamMaterial;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private float beamLength = 10f;
    [SerializeField] private int maxReflections = 3;

    [SerializeField] private float damagePerSecond = 10f;

    [SerializeField] private float damageInterval = 0.2f;

    [SerializeField] private StatusEffect statusEffect;

    private LineRenderer lineRenderer;
    private Transform shootingPoint;
    private GameObject spawnedFX;
    private Vector3 startPosition;
    private Vector3 direction;
    private float damageTimer;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        shootingPoint = parent.transform.Find("ShootingPoint");
        lineRenderer = shootingPoint.gameObject.GetComponent<LineRenderer>();
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.material = beamMaterial;
        lineRenderer.startWidth = 2.0f;
        lineRenderer.endWidth = 2.0f;
        lineRenderer.enabled = true;

        spawnedFX = FXManager.Instance.SpawnFX(triggerFX, shootingPoint.transform.position, shootingPoint.transform.rotation, parent.transform);
        damageTimer = 0f; // Reset damage timer
    }

    public override void update(GameObject parent)
    {
        base.update(parent);
        startPosition = shootingPoint.position;
        direction = shootingPoint.forward;
        Vector3 endPosition;

        spawnedFX.transform.position = shootingPoint.position;
        spawnedFX.transform.rotation = shootingPoint.rotation;

        List<Vector3> positions = new List<Vector3>();
        positions.Add(startPosition);

        for (int i = 0; i < maxReflections; i++, startPosition = endPosition)
        {
            // Cast a ray from the position and forward direction of this GameObject's transform
            Ray ray = new Ray(startPosition, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, beamLength, collisionLayerMask))
            {   
                float appliedDamage = 0f;
                HealthSystem damageHandler = hit.collider?.GetComponentInParent<HealthSystem>();
                if (damageHandler != null)
                {
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= damageInterval)
                    {
                        appliedDamage = damageHandler.ApplyDamage(parent, damagePerSecond * damageInterval);
                        damageTimer = 0f; // Reset damage timer
                    }
                }

                StatusEffectsSystem statusEffectsSystem = hit.collider?.GetComponentInParent<StatusEffectsSystem>();
                if (statusEffectsSystem != null)
                {
                    statusEffectsSystem.AddStatusEffect(parent, statusEffect);
                }

                // Check if the collider is explodable
                IExplodable explodable = hit.collider?.GetComponentInParent<IExplodable>();
                explodable?.Explode();
                
                if (appliedDamage == 0f) {
                    Vector3 modifiedNormal = new Vector3(hit.normal.x, 0f, hit.normal.z);
                    direction = Vector3.Reflect(direction, modifiedNormal);
                }
                

                endPosition = hit.point;
            }
            else
            {
                endPosition = startPosition + (direction * beamLength);
            }
            positions.Add(endPosition);
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);
        lineRenderer.enabled = false;
        FXManager.Instance.RemoveFX(spawnedFX.GetInstanceID());
    }

}
