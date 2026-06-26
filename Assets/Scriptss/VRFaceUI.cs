using UnityEngine;

public class VRFaceUI : MonoBehaviour
{
    [Header("Settings")]
    public Transform cameraTransform;   // Drag your Main Camera here
    public float distance = 1.5f;       // Distance from your face
    public float heightOffset = -0.1f;  // Slight adjustment up/down
    public float smoothSpeed = 8f;      // Higher = faster follow

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Calculate the target position in front of the camera
        // We only use the Y rotation (yaw) so the UI stays upright and doesn't tilt weirdly when you look up/down
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
        targetPosition.y += heightOffset;

        // 2. Smoothly move the Canvas to that position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // 3. Make the Canvas face the camera smoothly
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}