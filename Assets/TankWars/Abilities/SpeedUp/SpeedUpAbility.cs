using UnityEngine;

[CreateAssetMenu]
public class SpeedUpAbility : Ability
{
    public GameObject fxPrefab;
    public float speedModifier = 3f;

    private GameObject spawnedFx;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);

        spawnedFx = FXManager.Instance.SpawnFX(fxPrefab, parent.transform.position, parent.transform.rotation, parent.transform);
        parent.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(speedModifier);
    }

    public override void Deactivate(GameObject parent)
    {
        base.Deactivate(parent);

        parent.GetComponent<PhysicsMovementSystem>().ChangeMaxSpeed(-speedModifier);
        FXManager.Instance.RemoveFX(spawnedFx.GetInstanceID());
    }
}
