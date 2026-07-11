using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRWatchUI : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag your pop-up UI GameObject here.")]
    public GameObject watchUI;

    [Header("Detection Settings")]
    [Tooltip("Drag the Main Camera here (or leave empty to auto-find).")]
    public Transform playerCamera;
    
    [Tooltip("Drag an Empty GameObject that is placed exactly on the watch screen, with its BLUE arrow (Z) pointing straight out of the glass.")]
    public Transform watchScreenReference;
    
    [Tooltip("Higher = have to look more directly at the watch face (0.7 to 0.9 is a good range).")]
    [Range(0.1f, 1f)]
    public float lookAngleThreshold = 0.75f;

    // --- NEW: The Wrist Twist Check ---
    [Tooltip("Ensures the wrist is twisted flat. Higher = watch must be perfectly horizontal. (0.3 to 0.6 is usually best).")]
    [Range(0.1f, 1f)]
    public float horizontalThreshold = 0.4f;

    [Header("Haptic Feedback")]
    [Tooltip("Drag your Left XR Controller (under your XR Origin) here to make it vibrate.")]
    public XRBaseController leftHandController;
    public float vibrateIntensity = 0.5f;
    public float vibrateDuration = 0.15f;

    private bool isUIActive = false;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (watchUI != null)
            watchUI.SetActive(false); // Start hidden
    }

    private void Update()
    {
        if (playerCamera == null || watchScreenReference == null) return;

        // 1. Is the watch facing the camera?
        Vector3 directionToFace = (playerCamera.position - watchScreenReference.position).normalized;
        float cameraDot = Vector3.Dot(watchScreenReference.forward, directionToFace);
        bool isFacingCamera = cameraDot >= lookAngleThreshold;

        // 2. Is the wrist twisted horizontally? (Blue arrow pointing upwards)
        float upDot = Vector3.Dot(watchScreenReference.forward, Vector3.up);
        bool isHorizontal = upDot >= horizontalThreshold;

        // --- NEW: Must meet BOTH conditions to trigger ---
        bool isLookingAtWatch = isFacingCamera && isHorizontal;

        if (isLookingAtWatch && !isUIActive)
        {
            // Just tilted towards face and twisted horizontally
            isUIActive = true;
            if (watchUI != null) watchUI.SetActive(true);

            // Vibrate only once when it pops up
            if (leftHandController != null)
            {
                leftHandController.SendHapticImpulse(vibrateIntensity, vibrateDuration);
            }
        }
        else if (!isLookingAtWatch && isUIActive)
        {
            // Tilted away or arm dropped
            isUIActive = false;
            if (watchUI != null) watchUI.SetActive(false);
        }
    }
}