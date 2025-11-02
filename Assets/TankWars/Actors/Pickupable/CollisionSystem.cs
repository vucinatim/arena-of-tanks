using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    public delegate void OnCollisionDelegate(GameObject me, GameObject other);
    public event OnCollisionDelegate OnCollision;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(gameObject, collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollision?.Invoke(gameObject, other.gameObject);
    }
}
