using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class L2CommunicationManager : MonoBehaviour
{
    [Header("Dialogue + Cards")]
    [SerializeField] private DialogueSequencer dialogueSequencer;
    [SerializeField] private GameObject cardStackRoot; // This holds your PECS cards
    [SerializeField] private List<L2Card> cards = new List<L2Card>();
    [SerializeField] private XRBaseInteractor rightHandInteractor;

    [Header("Child + Reward")]
    [SerializeField] private Animator childAnimator;
    [SerializeField] private string nodTrigger = "Nod";
    [SerializeField] private string rejectTrigger = "Reject";
    [SerializeField] private GameObject trainToy;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueEnded;
    [SerializeField] private UnityEvent onCorrectCard;
    [SerializeField] private UnityEvent onWrongCard;
    [SerializeField] private UnityEvent onToyUnlocked;
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool dialogueFinished;
    private bool correctCardAccepted;
    private bool toyGiven;

    private void Awake()
    {
        if (dialogueSequencer == null) dialogueSequencer = FindAnyObjectByType<DialogueSequencer>();
    }

    private void Start()
    {
        // Hide cards and toy at the start of the Level
        if (cardStackRoot != null) cardStackRoot.SetActive(false);
        if (trainToy != null) trainToy.SetActive(false);

        foreach (var card in cards)
        {
            if (card != null) card.Initialize(this);
        }
    }

    private void OnEnable()
    {
        if (dialogueSequencer != null) dialogueSequencer.RegisterOnDialogueEnded(HandleDialogueEnded);
    }

    private void OnDisable()
    {
        if (dialogueSequencer != null) dialogueSequencer.UnregisterOnDialogueEnded(HandleDialogueEnded);
    }

    private void HandleDialogueEnded()
    {
        dialogueFinished = true;
        
        // This is where the cards finally appear for the player to grab
        if (cardStackRoot != null) cardStackRoot.SetActive(true);
        
        onDialogueEnded?.Invoke();
        Debug.Log("L2: Dialogue ended, card stack is now visible.");
    }

    // Called by the individual Card scripts when they hit the child's "trigger" area
    public void TryPresentCard(L2Card card)
    {
        if (!dialogueFinished || correctCardAccepted || card == null) return;

        // Ensure the player is using the Right Hand for this specific interaction
        bool rightHandOnly = rightHandInteractor == null || card.IsCurrentlyHeldBy(rightHandInteractor);
        if (!rightHandOnly) return;

        if (card.IsCorrectCard)
        {
            correctCardAccepted = true;
            if (childAnimator != null) childAnimator.SetTrigger(nodTrigger);
            if (trainToy != null) trainToy.SetActive(true);
            
            onCorrectCard?.Invoke();
            onToyUnlocked?.Invoke();
        }
        else
        {
            if (childAnimator != null) childAnimator.SetTrigger(rejectTrigger);
            onWrongCard?.Invoke();
        }
    }

    public void TryGiveToyToChild(L2TrainToy toy)
    {
        if (toyGiven || !correctCardAccepted || toy == null) return;

        bool rightHandOnly = rightHandInteractor == null || toy.IsCurrentlyHeldBy(rightHandInteractor);
        if (!rightHandOnly) return;

        toyGiven = true;
        onLevelCompleted?.Invoke();
        Debug.Log("L2 Complete: Training Level finished!");
    }
}