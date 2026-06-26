using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Needed to force VR hand release
using UnityEngine.XR.Interaction.Toolkit;

public class L2CommunicationManager : MonoBehaviour
{
    [Header("Flow References")]
    [SerializeField] private L2DialogueSequencer dialogueSequencer;
    [SerializeField] private GameObject cardsContainer;
    [SerializeField] private GameObject toyObject;
    
    [Header("Animation")]
    [SerializeField] private Animator childAnimator;

    // --- SLEIGHT OF HAND VARIABLES MOVED HERE ---
    [Header("Sleight of Hand")]
    [Tooltip("Drag the hidden, fake toy attached to the child's hand here.")]
    [SerializeField] private GameObject hiddenChildToy;

    [Header("Card Step")]
    [SerializeField] private float hidePresentedCardDelay = 3f;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueEnded;
    [SerializeField] private UnityEvent onWrongCardPresented;
    [SerializeField] private UnityEvent onCorrectCardPresented;
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool dialogueDone;
    private bool correctCardDone;
    private bool levelCompleted;

    private void Awake()
    {
        if (dialogueSequencer == null)
        {
            dialogueSequencer = FindAnyObjectByType<L2DialogueSequencer>();
        }

        // Safety check to ensure the child's toy is hidden when the game starts
        if (hiddenChildToy != null) hiddenChildToy.SetActive(false);
    }

    private void OnEnable()
    {
        if (dialogueSequencer != null)
            dialogueSequencer.RegisterOnDialogueEnded(HandleDialogueEnded);
    }

    private void OnDisable()
    {
        if (dialogueSequencer != null)
            dialogueSequencer.UnregisterOnDialogueEnded(HandleDialogueEnded);
    }

    private void Start()
    {
        if (toyObject != null) toyObject.SetActive(true);
    }

    private void HandleDialogueEnded()
    {
        dialogueDone = true;
        if (cardsContainer != null) cardsContainer.SetActive(true);

        onDialogueEnded?.Invoke();
        Debug.Log("L2: Dialogue ended. Waiting for correct card.");
    }

    public void TryPresentCard(L2Card card)
    {
        if (!dialogueDone || correctCardDone || card == null) return;

        if (!card.IsCorrectCard)
        {
            if (childAnimator != null) childAnimator.SetTrigger("ShakeHead");
            onWrongCardPresented?.Invoke();
            Debug.Log("L2: Wrong card presented.");
            return;
        }

        correctCardDone = true;
        if (childAnimator != null) childAnimator.SetTrigger("NodHead");
        onCorrectCardPresented?.Invoke();

        if (toyObject != null) toyObject.SetActive(true);

        StartCoroutine(HideAllCardsAfterDelay());
        Debug.Log("L2: Correct card presented. Toy can now be handed over.");
    }

    public void TryGiveToyToChild(L2TrainToy toy)
    {
        if (levelCompleted || !correctCardDone || toy == null) return;

        levelCompleted = true;
        
        // --- SLEIGHT OF HAND SWAP HAPPENS HERE ---
        
        // 1. Reveal the child's perfectly positioned fake toy
        if (hiddenChildToy != null)
        {
            hiddenChildToy.SetActive(true);
        }

        // 2. Safely force the VR player to let go of their physical toy
        var grab = toy.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected && grab.interactionManager != null)
        {
            grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
        }

        // 3. Poof! Hide the player's toy
        toy.gameObject.SetActive(false);
        
        // ------------------------------------------

        if (childAnimator != null) childAnimator.SetTrigger("AcceptToy");
        
        onLevelCompleted?.Invoke();
        Debug.Log("L2 Complete: Toy handed over to child and swapped!");
    }

    private IEnumerator HideAllCardsAfterDelay()
    {
        yield return new WaitForSeconds(hidePresentedCardDelay);
        if (cardsContainer != null) cardsContainer.SetActive(false); 
    }
}