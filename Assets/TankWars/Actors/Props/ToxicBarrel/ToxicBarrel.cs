using UnityEngine;

public class ToxicBarrel : MonoBehaviour, IExplodable
{
    public GameObject explosionEffect;  // Drag your Explosion Effect prefab here
    public GameObject poisonCloudPrefab;  // Drag your Green Sludge Effect prefab here
    public AudioClip explosionSound;    // Drag your Explosion Sound here

    private bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Instantiate explosion effect
        FXManager.Instance.SpawnFX(explosionEffect, transform.position, Quaternion.identity, null, 5f);

        // Play explosion sound
        if (explosionSound != null)
        {
            AudioManager.Instance.PlaySFX(explosionSound);
        }

        // Instantiate green sludge effect
        Instantiate(poisonCloudPrefab, transform.position, Quaternion.identity);

        // Destroy the barrel
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
