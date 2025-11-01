using UnityEngine;

public class ApplyPhysicsMaterial : MonoBehaviour
{
    public PhysicMaterial physicMaterial;

    private void Start()
    {
        ApplyMaterialToChildren(transform, physicMaterial);
    }

    private void ApplyMaterialToChildren(Transform parent, PhysicMaterial material)
    {
        foreach (Transform child in parent)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.sharedMaterial = material;
            }

            // Recursively apply the material to the child's children, if any
            if (child.childCount > 0)
            {
                ApplyMaterialToChildren(child, material);
            }
        }
    }
}
