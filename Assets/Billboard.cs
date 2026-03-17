using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Find the main camera at the start
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found! Please tag a camera as 'MainCamera'.");
            enabled = false; // Disable the script if no camera is found
        }
    }

    void LateUpdate()
    {
        // Ensure the object is updated after all other movement calculations for smoothness
        if (mainCameraTransform != null)
        {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}
