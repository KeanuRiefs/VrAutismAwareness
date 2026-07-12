using UnityEngine;
using UnityEngine.Events;

public class DirectCommunicationManager : MonoBehaviour
{
    [Header("Flow References")]
    [SerializeField] private GameObject cardsContainer;
    
    [Header("Animation")]
    [SerializeField] private Animator childAnimator;

    [Header("Events")]
    [SerializeField] private UnityEvent onLevelStarted; 
    [SerializeField] private UnityEvent onWrongCardPresented;
    [SerializeField] private UnityEvent onCorrectCardPresented;
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool correctCardDone;
    private bool levelCompleted;

    private void Start()
    {
        // Immediately start the sequence
        if (cardsContainer != null) cardsContainer.SetActive(true);

        onLevelStarted?.Invoke();
        Debug.Log("Level started immediately. Waiting for correct card.");
    }

    public void TryPresentCard(L2Card card)
    {
        if (correctCardDone || card == null) return;

        if (!card.IsCorrectCard)
        {
            if (childAnimator != null) childAnimator.SetTrigger("ShakeHead");
            onWrongCardPresented?.Invoke();
            Debug.Log("Wrong card presented.");
            return;
        }

        correctCardDone = true;
        
        // 1. Trigger the nod animation
        if (childAnimator != null) childAnimator.SetTrigger("NodHead");
        
        // 2. Fire the correct card event
        onCorrectCardPresented?.Invoke();

        // 3. Hide the UI cards instantly (Fixes the Coroutine crash!)
        if (cardsContainer != null) cardsContainer.SetActive(false);

        // 4. Complete the level (This turns off the GameObject via MasterManager)
        levelCompleted = true;
        onLevelCompleted?.Invoke();
        
        Debug.Log("Correct card presented. Level complete!");
    }
}