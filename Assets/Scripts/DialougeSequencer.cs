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

    public void RegisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction += callback;
    public void UnregisterOnDialogueEnded(System.Action callback) => onDialogueEndedAction -= callback;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh != null) textMesh.text = "";
        SetupPecsCardContainer();
    }

    private void OnEnable()
    {
        if (continueAction.action != null)
        {
            continueAction.action.Enable();
            continueAction.action.performed += _ => ContinueDialogue();
        }
    }

    private void OnDisable()
    {
        if (continueAction.action != null)
        {
            continueAction.action.performed -= _ => ContinueDialogue();
            continueAction.action.Disable();
        }
    }

    private void Start() => DisplayNextLine();

    // --- THE ONLY METHOD YOU NEED FOR YOUR BUTTON ---
    public void ContinueDialogue()
    {
        if (isTyping) return; // Don't skip while typing (unless you want to add skip logic)
        DisplayNextLine();
    }

    private void DisplayNextLine()
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
        if (textMesh != null) textMesh.text = "";
        if (dialogueContainer != null) dialogueContainer.SetActive(false);
        if (dialogueBackground != null) dialogueBackground.SetActive(false);

        if (pecsCardContainer != null)
        {
            if (pecsFollow == null) SetupPecsCardContainer();
            pecsCardContainer.SetActive(true);
            if (pecsFollow != null) pecsFollow.enabled = true;
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

    private void SetupPecsCardContainer()
    {
        if (pecsCardContainer == null)
            pecsCardContainer = GameObject.Find("PecsCardContainer");

        if (playerCameraTransform == null && Camera.main != null)
            playerCameraTransform = Camera.main.transform;

        if (pecsCardContainer == null) return;

        pecsFollow = pecsCardContainer.GetComponent<VRFaceUI>() ?? pecsCardContainer.AddComponent<VRFaceUI>();
        pecsFollow.cameraTransform = playerCameraTransform;
        pecsFollow.distance = pecsDistance;
        pecsFollow.heightOffset = pecsHeightOffset;
        pecsFollow.smoothSpeed = pecsSmoothSpeed;
        pecsFollow.enabled = false;
        pecsCardContainer.SetActive(false);
    }
}