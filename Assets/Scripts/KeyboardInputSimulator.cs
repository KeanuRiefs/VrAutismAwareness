using UnityEngine;

public class KeyboardInputSimulator : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform leftHand;   
    public Transform rightHand;
    public Vector3 blockOffset = new Vector3(0, 0.5f, 0.5f);
    public Vector3 restOffset = new Vector3(0, -0.5f, 0.5f);
    public float handMoveSpeed = 10f;

    [Header("Interaction Settings")]
    public Transform holdPosition; // An empty object in front of your camera (where you hold items)
    private GameObject heldItem = null;

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float mouseSensitivity = 100.0f; // New variable for mouse speed

    // Private variables to track rotation
    private float xRotation = 0f;

    void Start()
    {
        // Optional: Lock cursor to center of screen so it doesn't click outside
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        // --- 1. MOUSE LOOK (New!) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate Player Body Left/Right (Y axis)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate Camera Up/Down (X axis)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Stop from breaking neck
        
        // Apply the rotation to the Camera (assuming this script is ON the camera)
        // If script is on a Parent object, you might need to rotate a child camera instead.
        // For now, let's assume this script is on the Main Camera object.
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Also rotate the parent body if this script is on the Player Body
        // If this script is ON the Camera, the line 'transform.Rotate' above handles Left/Right.


        // --- 2. MOVEMENT (WASD) ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * walkSpeed * Time.deltaTime;


        // --- 3. HAND BLOCKING ---
        if (Input.GetKey(KeyCode.E))
        {
            MoveHands(blockOffset);
        }
        else
        {
            MoveHands(restOffset);
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }
    }

    void MoveHands(Vector3 targetOffset)
    {
        // Simple local position movement relative to camera
        Vector3 targetLeft = new Vector3(-0.3f, targetOffset.y, targetOffset.z);
        Vector3 targetRight = new Vector3(0.3f, targetOffset.y, targetOffset.z);

        leftHand.localPosition = Vector3.Lerp(leftHand.localPosition, targetLeft, Time.deltaTime * handMoveSpeed);
        rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, targetRight, Time.deltaTime * handMoveSpeed);
    }

    void HandleInteraction()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3.0f))
        {
            // CASE 1: Picking up an item (if hands are empty)
            if (heldItem == null)
            {
                PickupItem item = hit.transform.GetComponent<PickupItem>();
                if (item != null)
                {
                    GrabObject(hit.transform.gameObject);
                }
            }
            // CASE 2: Giving the item to the Child
            else 
            {
                SensoryChild child = hit.transform.GetComponent<SensoryChild>();
                if (child != null)
                {
                    GiveObject(child);
                }
            }
        }
    }

    void GrabObject(GameObject item)
    {
        heldItem = item;
        item.GetComponent<Rigidbody>().isKinematic = true; // Stop gravity
        item.transform.parent = holdPosition; // Attach to player
        item.transform.localPosition = Vector3.zero; // Snap to hand
        item.transform.localRotation = Quaternion.identity;
    }

    void GiveObject(SensoryChild child)
    {
        // Tell the child what they received
        string type = heldItem.GetComponent<PickupItem>().itemType;
        child.ReceiveItem(type);

        // Destroy the item (simulating putting it on)
        Destroy(heldItem); 
        heldItem = null; // Hands are empty again
    }
}