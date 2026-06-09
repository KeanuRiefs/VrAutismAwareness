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

    // --- ADDED AUDIO REFERENCES ---
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
        
        // Safety check if you forgot to assign the audio source
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
            
            // --- ADDED: Play matching voice line ---
            PlayCurrentVoiceLine(currentIndex);

            StartCoroutine(TypeText(dialogueLines[currentIndex]));
            currentIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    // --- ADDED AUDIO LOGIC ---
    private void PlayCurrentVoiceLine(int index)
    {
        if (dialogueAudioSource == null) return;

        // Stop whatever voice line was playing previously
        dialogueAudioSource.Stop();

        // Check if there is a valid voice clip assigned for this text index
        if (voiceLines != null && index < voiceLines.Length && voiceLines[index] != null)
        {
            dialogueAudioSource.clip = voiceLines[index];
            dialogueAudioSource.Play();
        }
    }

    private void EndDialogue()
    {
        // --- ADDED: Stop voice line if dialogue is forced to end ---
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