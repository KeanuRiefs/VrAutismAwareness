using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BearHandover : MonoBehaviour
{
    public Animator bearAnimator; 
    [Header("VR Interaction")]
    [SerializeField] private GameObject cardInHand; 
    [SerializeField] private XRGrabInteractable cardInteractable; 

    [Header("Events")]
    [Tooltip("Fires the exact moment the player grabs the card from the bear.")]
    public UnityEvent onCardGrabbed;

    void Start()
    {
        if (bearAnimator == null) bearAnimator = GetComponent<Animator>();
        
        if (cardInHand != null) cardInHand.SetActive(false);
    }

    public void StartHandover()
    {
        if (cardInHand != null) cardInHand.SetActive(true);
        bearAnimator.SetTrigger("StartHandover"); 
    }

    public void PauseForPlayer() 
    {
        bearAnimator.speed = 0f; 
        
        if (cardInteractable != null)
        {
            cardInteractable.selectEntered.AddListener(OnCardTaken);
        }
    }

    private void OnCardTaken(SelectEnterEventArgs args)
    {
        ResumeAnimation();
        
        cardInteractable.selectEntered.RemoveListener(OnCardTaken);

        // --- NEW: Triggers your UI updates in the Inspector! ---
        onCardGrabbed?.Invoke();

        CommunicationCard cardScript = cardInHand.GetComponent<CommunicationCard>();
        if (cardScript != null) cardScript.PresentToChild();
    }

    public void ResumeAnimation() 
    {
        bearAnimator.speed = 1f;
    }
}