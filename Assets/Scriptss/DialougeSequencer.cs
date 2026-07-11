using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// Renamed to guarantee it doesn't conflict with any other script in your project
[System.Serializable]
public class DialogueAnimTrigger
{
    [Tooltip("The Animator component of the character you want to animate.")]
    public Animator animator;
    [Tooltip("The exact name of the Trigger parameter in their Animator Controller.")]
    public string triggerName;
}

[System.Serializable]
public class LineAnimationGroup
{
    [Tooltip("Add multiple character animations to trigger on this specific dialogue line index.")]
    public DialogueAnimTrigger[] animationsToPlay;
}

// CHANGED TO MATCH YOUR UNITY FILENAME EXACTLY (DialougeSequencer)
public class DialougeSequencer : MonoBehaviour
{
    private System.Action onDialogueEndedAction;

    [Header("Dialogue Content")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private GameObject dialogueBackground;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("Audio Settings")]
    [Tooltip("Drag your AudioSource component here (can be on this object or the character).")]
    [SerializeField] private AudioSource dialogueAudioSource;
    [Tooltip("Add your voice lines here in the exact same order as your dialogue lines.")]
    [SerializeField] private AudioClip[] voiceLines;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("XR Input (Optional)")]
    [SerializeField] private InputActionProperty continueAction;

    [Header("Animation References")]
    [SerializeField] private Animator childAnimator; 
    
    // --- The parallel array for line-by-line animations ---
    [Tooltip("Element 0 matches Dialogue Line 0, Element 1 matches Line 1, etc. Leave empty if a line has no animations.")]
    [SerializeField] private LineAnimationGroup[] lineAnimations;

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
        
        if (dialogueAudioSource == null) dialogueAudioSource = GetComponent<AudioSource>();

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

    public void ContinueDialogue()
    {
        if (isTyping) return; 
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (currentIndex < dialogueLines.Length)
        {
            StopAllCoroutines();
            
            // 1. Play matching voice line
            PlayCurrentVoiceLine(currentIndex);

            // 2. Trigger any animations assigned to this specific line
            TriggerAnimationsForIndex(currentIndex);

            // 3. Type out the text
            StartCoroutine(TypeText(dialogueLines[currentIndex]));
            currentIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private void PlayCurrentVoiceLine(int index)
    {
        if (dialogueAudioSource == null) return;

        dialogueAudioSource.Stop();

        if (voiceLines != null && index < voiceLines.Length && voiceLines[index] != null)
        {
            dialogueAudioSource.clip = voiceLines[index];
            dialogueAudioSource.Play();
        }
    }

    private void TriggerAnimationsForIndex(int index)
    {
        if (lineAnimations == null || index >= lineAnimations.Length || lineAnimations[index] == null) return;

        var currentGroup = lineAnimations[index].animationsToPlay;
        if (currentGroup == null || currentGroup.Length == 0) return;

        foreach (var action in currentGroup)
        {
            if (action.animator != null && !string.IsNullOrEmpty(action.triggerName))
            {
                action.animator.SetTrigger(action.triggerName);
            }
        }
    }

    private void EndDialogue()
    {
        if (dialogueAudioSource != null) dialogueAudioSource.Stop();

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