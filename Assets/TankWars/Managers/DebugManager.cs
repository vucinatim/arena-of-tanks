using UnityEngine;

class DebugManager : Singleton<DebugManager> {

    public void ShowBlastRadiusSphere(Vector3 position, float blastRadius, float duration, Color color)
    {
        color.a = 0.1f;
        // If the blast radius sphere GameObject does not exist, create it
        var blastRadiusSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(blastRadiusSphere.GetComponent<Collider>());
        blastRadiusSphere.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));
        blastRadiusSphere.GetComponent<Renderer>().material.color = color;

        // Set the position and scale of the blast radius sphere
        blastRadiusSphere.transform.position = position;
        blastRadiusSphere.transform.localScale = new Vector3(blastRadius * 2f, blastRadius * 2f, blastRadius * 2f);

        Destroy(blastRadiusSphere, duration);
    }
}