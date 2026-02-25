using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class L2CommunicationManager : MonoBehaviour
{
    [Header("Flow References")]
    [SerializeField] private DialogueSequencer dialogueSequencer;
    [SerializeField] private GameObject cardsContainer;
    [SerializeField] private GameObject toyObject;

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
            dialogueSequencer = FindAnyObjectByType<DialogueSequencer>();
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

    private void Start()
    {
        if (toyObject != null)
        {
            toyObject.SetActive(false);
        }
    }

    private void HandleDialogueEnded()
    {
        dialogueDone = true;

        if (cardsContainer != null)
        {
            cardsContainer.SetActive(true);
        }

        onDialogueEnded?.Invoke();
        Debug.Log("L2: Dialogue ended. Waiting for correct card.");
    }

    public void TryPresentCard(L2Card card)
    {
        if (!dialogueDone || correctCardDone || card == null)
        {
            return;
        }

        if (!card.IsCorrectCard)
        {
            onWrongCardPresented?.Invoke();
            Debug.Log("L2: Wrong card presented.");
            return;
        }

        correctCardDone = true;
        onCorrectCardPresented?.Invoke();

        if (toyObject != null)
        {
            toyObject.SetActive(true);
        }

        StartCoroutine(HideCardAfterDelay(card.gameObject));
        Debug.Log("L2: Correct card presented. Toy can now be handed over.");
    }

    public void TryGiveToyToChild(L2TrainToy toy)
    {
        if (levelCompleted || !correctCardDone || toy == null)
        {
            return;
        }

        levelCompleted = true;
        onLevelCompleted?.Invoke();
        Debug.Log("L2 Complete: Toy handed over to child.");
    }

    private IEnumerator HideCardAfterDelay(GameObject cardObject)
    {
        yield return new WaitForSeconds(hidePresentedCardDelay);

        if (cardObject != null)
        {
            cardObject.SetActive(false);
        }
    }
}
