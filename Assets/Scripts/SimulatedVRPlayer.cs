using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimulatedVRPlayer : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1.0f; 
    public KeyCode crouchKey = KeyCode.C;
    
    // How long it takes to crouch (smoothness)
    public float crouchTransitionSpeed = 10f;

    [Header("Camera Settings")]
    // Where are your eyes when standing? (e.g., 1.6 meters high)
    public float standingEyeLevel = 1.6f;
    // Where are your eyes when crouching? (e.g., 0.8 meters high)
    public float crouchingEyeLevel = 0.8f;

    private CharacterController controller;
    private Transform cameraTransform;
    private float verticalRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
        
        // Find the camera inside the player
        cameraTransform = GetComponentInChildren<Camera>().transform;
        
        // Lock cursor for FPS feel
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleCrouch();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Use 'transform.right' and 'transform.forward' to move relative to where we are looking
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Rotate Camera Up/Down
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        // Rotate Body Left/Right
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        // define targets based on whether we are holding the key
        float targetHeight = standingHeight;
        float targetEyeLevel = standingEyeLevel;

        if (Input.GetKey(crouchKey))
        {
            targetHeight = crouchingHeight;
            targetEyeLevel = crouchingEyeLevel;
        }

        // 1. Smoothly change the Collider Height (The physical box)
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        // 2. IMPORTANT: Adjust the center so we shrink from the top down (feet stay on floor)
        // By default, Unity shrinks from the center (lifting feet). We fix this by moving the center down.
        controller.center = Vector3.down * (standingHeight - controller.height) / 2.0f;

        // 3. Smoothly move the Camera (The Eyes)
        Vector3 currentCamPos = cameraTransform.localPosition;
        float newY = Mathf.Lerp(currentCamPos.y, targetEyeLevel, crouchTransitionSpeed * Time.deltaTime);
        
        cameraTransform.localPosition = new Vector3(currentCamPos.x, newY, currentCamPos.z);
    }

    public bool IsCrouching()
    {
        // We are crouching if our height is closer to the crouch target than the standing target
        return controller.height < (standingHeight + crouchingHeight) / 2;
    }
}