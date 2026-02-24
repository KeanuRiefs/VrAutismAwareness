using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class L2CommunicationManager : MonoBehaviour
{
    [Header("Dialogue + Cards")]
    [SerializeField] private DialogueSequencer dialogueSequencer;
    [SerializeField] private GameObject cardStackRoot;
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
        if (dialogueSequencer == null)
        {
            dialogueSequencer = FindAnyObjectByType<DialogueSequencer>();
        }
    }

    private void Start()
    {
        if (cardStackRoot != null) cardStackRoot.SetActive(false);
        if (trainToy != null) trainToy.SetActive(false);

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                cards[i].Initialize(this);
            }
        }
    }

    private void OnEnable()
    {
        if (dialogueSequencer != null)
        {
            dialogueSequencer.RegisterOnDialogueEnded(HandleDialogueEnded);
        }
    }

    private void OnDisable()
    {
        if (dialogueSequencer != null)
        {
            dialogueSequencer.UnregisterOnDialogueEnded(HandleDialogueEnded);
        }
    }

    private void HandleDialogueEnded()
    {
        dialogueFinished = true;
        if (cardStackRoot != null) cardStackRoot.SetActive(true);
        onDialogueEnded?.Invoke();
        Debug.Log("L2: Dialogue ended, card stack is now visible.");
    }

    public void TryPresentCard(L2Card card)
    {
        if (!dialogueFinished || correctCardAccepted || card == null) return;

        bool rightHandOnly = rightHandInteractor == null || card.IsCurrentlyHeldBy(rightHandInteractor);
        if (!rightHandOnly)
        {
            Debug.Log("L2: Card ignored. Present using right hand interactor.");
            return;
        }

        if (card.IsCorrectCard)
        {
            correctCardAccepted = true;
            if (childAnimator != null && !string.IsNullOrEmpty(nodTrigger)) childAnimator.SetTrigger(nodTrigger);
            if (trainToy != null) trainToy.SetActive(true);
            onCorrectCard?.Invoke();
            onToyUnlocked?.Invoke();
            Debug.Log("L2: Correct card accepted. Train toy unlocked.");
            return;
        }

        if (childAnimator != null && !string.IsNullOrEmpty(rejectTrigger)) childAnimator.SetTrigger(rejectTrigger);
        onWrongCard?.Invoke();
        Debug.Log("L2: Wrong card shown.");
    }

    public void TryGiveToyToChild(L2TrainToy toy)
    {
        if (toyGiven || !correctCardAccepted || toy == null) return;

        bool rightHandOnly = rightHandInteractor == null || toy.IsCurrentlyHeldBy(rightHandInteractor);
        if (!rightHandOnly)
        {
            Debug.Log("L2: Toy ignored. Give using right hand interactor.");
            return;
        }

        toyGiven = true;
        onLevelCompleted?.Invoke();
        Debug.Log("L2 Complete: Correct card + train toy delivered to child.");
    }
}
