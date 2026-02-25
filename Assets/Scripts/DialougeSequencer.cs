using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueSequencer : MonoBehaviour
{
    // This Action allows other scripts (like L2CommunicationManager) to listen for the end
    private System.Action onDialogueEndedAction;

    [Header("Dialogue Content")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private GameObject dialogueBackground;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("XR Input")]
    [SerializeField] private InputActionProperty continueAction;

    [Header("Animation References")]
    [SerializeField] private BearHandover bearHandoverScript;

    [Header("L2 PECS Cards (Assign in Inspector)")]
    [SerializeField] private GameObject pecsCardContainer;
    [SerializeField] private VRFaceUI pecsCardFollow;
    [SerializeField] private Transform playerCameraTransform;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueEnded;

    private TMP_Text textMesh;
    private int currentIndex = 0;
    private bool isTyping = false;

    // --- NEW REGISTRATION METHODS FOR L2 MANAGER ---
    public void RegisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction += callback;
    public void UnregisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction -= callback;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.text = "";

        // Keep PECS hidden/follow-disabled until dialogue ends.
        if (pecsCardContainer != null) pecsCardContainer.SetActive(false);
        if (pecsCardFollow != null) pecsCardFollow.enabled = false;
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

    private void OnContinuePressed(InputAction.CallbackContext context)
    {
        if (!isTyping) DisplayNextLine();
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

        if (pecsCardContainer != null) pecsCardContainer.SetActive(true);

        if (pecsCardFollow != null)
        {
            if (playerCameraTransform != null)
            {
                pecsCardFollow.cameraTransform = playerCameraTransform;
            }

            pecsCardFollow.enabled = true;
        }

        // 1. Notify the Bear to start its animation
        if (bearHandoverScript != null) bearHandoverScript.StartHandover();

        // 2. Notify the L2CommunicationManager and other listeners
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
