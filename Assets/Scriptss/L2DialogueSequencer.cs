using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class L2DialogueSequencer : MonoBehaviour
{
    private System.Action onDialogueEndedAction;

    [Header("Dialogue Content")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private GameObject dialogueBackground;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource dialogueAudioSource;
    [SerializeField] private AudioClip[] voiceLines;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("XR Input (Optional)")]
    [SerializeField] private InputActionProperty continueAction;

    [Header("Animation References")]
    [Tooltip("Element 0 matches Dialogue Line 0, Element 1 matches Line 1, etc.")]
    [SerializeField] private LineAnimationGroup[] lineAnimations;

    // --- NEW: A safe way to trigger scripts per line! ---
    [Header("Special Script Triggers")]
    [Tooltip("Fire events on specific lines. Element 0 = Line 1, Element 1 = Line 2, etc.")]
    [SerializeField] private UnityEvent[] lineEvents;

    [Header("L2 PECS Cards")]
    [SerializeField] private GameObject pecsCardContainer;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueEnded;

    private TMP_Text textMesh;
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
            
            PlayCurrentVoiceLine(currentIndex);
            TriggerAnimationsForIndex(currentIndex);

            // --- NEW: Trigger the Unity Event for this exact line ---
            if (lineEvents != null && currentIndex < lineEvents.Length && lineEvents[currentIndex] != null)
            {
                lineEvents[currentIndex]?.Invoke();
            }

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
            pecsCardContainer.SetActive(true);
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

        if (pecsCardContainer != null)
        {
            pecsCardContainer.SetActive(false);
        }
    }
}