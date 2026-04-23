using UnityEngine;

public class VRObjectiveUI : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Positioning")]
    public float distance = 1.2f;       // How far in front
    public float horizontalOffset = -0.5f; // Negative = Left, Positive = Right
    public float verticalOffset = 0.3f;   // Negative = Down, Positive = Up
    
    [Header("Look & Tilt")]
    public Vector3 tiltOffset = new Vector3(0, 20, 0); // Tilt it toward the center
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null) cameraTransform = Camera.main.transform;
            else return;
        }

        // 1. Calculate the Corner Position
        // Start at camera, go forward, then move left (Right * horizontal), then move up (Up * vertical)
        Vector3 targetPosition = cameraTransform.position 
                                 + (cameraTransform.forward * distance) 
                                 + (cameraTransform.right * horizontalOffset) 
                                 + (cameraTransform.up * verticalOffset);

        // 2. Smoothly Move
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // 3. Rotation: Face the player first
        Quaternion facePlayerRotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        
        // 4. Apply the custom "Objective Tilt"
        // This angles it slightly so it looks like it's wrapping around your head
        Quaternion finalRotation = facePlayerRotation * Quaternion.Euler(tiltOffset);

        // 5. Smoothly Rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * smoothSpeed);
    }
} //Fuck this shit