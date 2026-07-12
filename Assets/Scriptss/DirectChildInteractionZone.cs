using UnityEngine;

public class DirectChildInteractionZone : MonoBehaviour
{
    [SerializeField] private DirectCommunicationManager manager;

    private void Awake()
    {
        if (manager == null)
        {
            manager = FindAnyObjectByType<DirectCommunicationManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only look for the PECS card
        L2Card card = other.GetComponentInParent<L2Card>();
        if (card != null)
        {
            manager?.TryPresentCard(card);
        }
        
        // Toy logic has been completely removed!
    }
}