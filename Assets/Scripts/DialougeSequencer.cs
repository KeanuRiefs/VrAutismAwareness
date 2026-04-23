using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueSequencer : MonoBehaviour
{
    private System.Action onDialogueEndedAction;

    [Header("Dialogue Content")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private GameObject dialogueBackground;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [Tooltip("Prevents accidental double-skipping when physically pushing the button.")]
    [SerializeField] private float interactionCooldown = 0.5f;

    [Header("XR Input (Optional)")]
    [SerializeField] private InputActionProperty continueAction;

    [Header("Animation References")]
    [SerializeField] private Animator childAnimator; 

    [Header("L2 PECS Cards (Optional)")]
    [SerializeField] private GameObject pecsCardContainer;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float pecsDistance = 0.6f;
    [SerializeField] private float pecsHeightOffset = -0.1f;
    [SerializeField] private float pecsSmoothSpeed = 8f;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueEnded;

    private TMP_Text textMesh;
    private VRFaceUI pecsFollow;
    private int currentIndex = 0;
    private bool isTyping = false;
    private float lastInteractionTime; // Tracks cooldown

    public void RegisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction += callback;
    public void UnregisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction -= callback;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.text = "";
        SetupPecsCardContainer();
    }

    private void SetupPecsCardContainer()
    {
        if (pecsCardContainer == null)
        {
            GameObject foundContainer = GameObject.Find("PecsCardContainer");
            if (foundContainer != null) pecsCardContainer = foundContainer;
        }

        if (playerCameraTransform == null && Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }

        if (pecsCardContainer == null) return;

        pecsFollow = pecsCardContainer.GetComponent<VRFaceUI>();
        if (pecsFollow == null)
        {
            pecsFollow = pecsCardContainer.AddComponent<VRFaceUI>();
        }

        pecsFollow.cameraTransform = playerCameraTransform;
        pecsFollow.distance = pecsDistance;
        pecsFollow.heightOffset = pecsHeightOffset;
        pecsFollow.smoothSpeed = pecsSmoothSpeed;
        pecsFollow.enabled = false;
        pecsCardContainer.SetActive(false);
    }

    private void OnEnable()
    {
        if (continueAction.action != null)
        {
            continueAction.action.Enable();
            continueAction.action.performed += OnContinuePressed;
        }
    }

    private void OnDisable()
    {
        if (continueAction.action != null)
        {
            continueAction.action.performed -= OnContinuePressed;
            continueAction.action.Disable();
        }
    }

    private void Start() => DisplayNextLine();

    // Triggered by the controller button
    private void OnContinuePressed(InputAction.CallbackContext context)
    {
        TryAdvanceDialogue();
    }

    // --- NEW METHOD FOR PHYSICAL UI BUTTON ---
    // Link your UI Button's OnClick() event to this method in the inspector
    public void OnPhysicalButtonPush()
    {
        TryAdvanceDialogue();
    }

    private void TryAdvanceDialogue()
    {
        // Check 1: Is it still typing?
        if (isTyping) return;

        // Check 2: Has enough time passed since the last push? (Anti-jitter)
        if (Time.time - lastInteractionTime < interactionCooldown) return;

        lastInteractionTime = Time.time;
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentIndex < dialogueLines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeText(dialogueLines[currentIndex]));
            currentIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        textMesh.text = "";
        if (dialogueContainer != null) dialogueContainer.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        if (pecsCardContainer != null)
        {
            if (pecsFollow == null) SetupPecsCardContainer();
            if (playerCameraTransform == null && Camera.main != null) playerCameraTransform = Camera.main.transform;

            pecsCardContainer.SetActive(true);

            if (pecsFollow != null)
            {
                pecsFollow.cameraTransform = playerCameraTransform != null ? playerCameraTransform : pecsFollow.cameraTransform;
                pecsFollow.enabled = true;
            }
        }

        if (childAnimator != null)
        {
            childAnimator.SetTrigger("StartTantrum");
        }

        onDialogueEndedAction?.Invoke();
        onDialogueEnded?.Invoke();
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        textMesh.text = "";
        foreach (char letter in line)
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}