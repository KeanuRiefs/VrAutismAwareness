using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueSequencer : MonoBehaviour
{
    [Header("Dialogue Content")]
    [SerializeField] private GameObject dialogueContainer;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("XR Input")]
    [Tooltip("Assign Right Controller Trigger Action here")]
    [SerializeField] private InputActionProperty continueAction;

    [Header("Animation References")]
    [SerializeField] private BearHandover bearHandoverScript;

    private TMP_Text textMesh;
    private int currentIndex = 0;
    private bool isTyping = false;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.text = "";
    }

    private void OnEnable()
    {
        continueAction.action.Enable();
        continueAction.action.performed += OnContinuePressed;
    }

    private void OnDisable()
    {
        continueAction.action.performed -= OnContinuePressed;
        continueAction.action.Disable();
    }

    private void Start()
    {
        DisplayNextLine();
    }

    private void OnContinuePressed(InputAction.CallbackContext context)
    {
        if (!isTyping)
        {
            DisplayNextLine();
        }
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
        Debug.Log("End of dialogue.");

        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

        if (bearHandoverScript != null)
            bearHandoverScript.StartHandover();
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
