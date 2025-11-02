using UnityEngine;
using UnityEngine.UI;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Canvas canvas;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.worldCamera = mainCamera;
        }
    }

    private void Update()
    {
        // Make the Canvas face the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
