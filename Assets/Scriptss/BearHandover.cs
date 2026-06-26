using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BearHandover : MonoBehaviour
{
    public Animator bearAnimator; 
    [Header("VR Interaction")]
    [SerializeField] private GameObject cardInHand; // The card object the bear is holding
    [SerializeField] private XRGrabInteractable cardInteractable; // The Grab component on that card

    void Start()
    {
        if (bearAnimator == null) bearAnimator = GetComponent<Animator>();
        
        // Ensure the card starts hidden and we aren't listening for grabs yet
        if (cardInHand != null) cardInHand.SetActive(false);
    }

    public void StartHandover()
    {
        // 1. Show the card in the bear's hand
        if (cardInHand != null) cardInHand.SetActive(true);

        // 2. Play the animation of the bear reaching out
        bearAnimator.SetTrigger("StartHandover"); 
    }

    // Called via Animation Event when the bear's arm is fully extended
    public void PauseForPlayer() 
    {
        bearAnimator.speed = 0f; 
        
        // Enable the ability to grab the card now that it's being offered
        if (cardInteractable != null)
        {
            cardInteractable.selectEntered.AddListener(OnCardTaken);
        }
    }

    private void OnCardTaken(SelectEnterEventArgs args)
    {
        // Once the player grabs the card, resume the animation (bear pulling hand back)
        ResumeAnimation();
        
        // Remove listener to prevent multiple triggers
        cardInteractable.selectEntered.RemoveListener(OnCardTaken);

        // Notify the level manager that the card is now being "presented"
        CommunicationCard cardScript = cardInHand.GetComponent<CommunicationCard>();
        if (cardScript != null) cardScript.PresentToChild();
    }

    public void ResumeAnimation() 
    {
        bearAnimator.speed = 1f;
    }
}