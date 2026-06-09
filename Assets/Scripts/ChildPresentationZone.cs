using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Needed just in case we need to cleanly break the grip

public class ChildPresentationZone : MonoBehaviour
{
    [Header("Sleight of Hand Setup")]
    [Tooltip("Drag the hidden, fake toy that is already attached to the child's hand bone here.")]
    [SerializeField] private GameObject hiddenChildToy;

    private void OnTriggerEnter(Collider other)
    {
        CommunicationCard card = other.GetComponentInParent<CommunicationCard>();
        if (card != null)
        {
            card.PresentToChild();
            return;
        }

        CommunicationComfortItem comfortItem = other.GetComponentInParent<CommunicationComfortItem>();
        if (comfortItem != null)
        {
            comfortItem.GiveToChild();
            
            // Perform the visual swap
            PerformToySwap(comfortItem.gameObject);
        }
    }

    private void PerformToySwap(GameObject playerToy)
    {
        // 1. Reveal the perfectly positioned toy in the child's hand
        if (hiddenChildToy != null)
        {
            hiddenChildToy.SetActive(true);
        }
        else
        {
            Debug.LogWarning("You forgot to assign the Hidden Child Toy in the inspector!");
        }

        // 2. Safely tell the VR system to let go of the player's toy (prevents invisible grip bugs)
        XRGrabInteractable grab = playerToy.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected && grab.interactionManager != null)
        {
            grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
        }

        // 3. Poof! Hide the player's toy so it looks like it transferred instantly.
        playerToy.SetActive(false);
        
        Debug.Log("Sleight of hand swap complete!");
    }
}