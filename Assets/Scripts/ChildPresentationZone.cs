using UnityEngine;

public class ChildPresentationZone : MonoBehaviour
{
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
        }
    }
}
