using UnityEngine;
using UnityEngine.AI;

public static class GameUtility
{
    public static int CalculateDamage(
        float blastRadius,
        float minDamage,
        float maxDamage,
        Transform target,
        Transform blastPoint
    )
    {
        // Calculate the distance between the target's position and the blast point
        float distance = Vector3.Distance(target.position, blastPoint.position);

        // Calculate the normalized distance (0 to 1) within the blast radius
        float normalizedDistance = Mathf.Clamp01(distance / blastRadius);

        // Calculate the damage based on the normalized distance
        int damage = Mathf.RoundToInt(Mathf.Lerp(maxDamage, minDamage, normalizedDistance));

        return damage;
    }

    public static Vector3 GetRandomPointInNavMesh()
    {
        // Define a random point in the world
        Vector3 randomPoint = new Vector3(
            Random.Range(-80f, 80f), // Adjust these ranges to match your level size
            0,
            Random.Range(-80f, 80f)
        );

        // Use NavMesh.SamplePosition to find the closest point on the NavMesh
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 50.0f, NavMesh.AllAreas))
        {
            return hit.position; // Return the valid point on the NavMesh
        }

        Debug.LogWarning("Failed to sample a random point on the NavMesh.");
        return Vector3.zero; // Fallback to the origin
    }
}
