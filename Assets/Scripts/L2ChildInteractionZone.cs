using UnityEngine;

public class L2ChildInteractionZone : MonoBehaviour
{
    [SerializeField] private L2CommunicationManager manager;

    private void Awake()
    {
        if (manager == null) manager = FindAnyObjectByType<L2CommunicationManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        L2Card card = other.GetComponentInParent<L2Card>();
        if (card != null)
        {
            card.PresentToChild();
            return;
        }

        L2TrainToy toy = other.GetComponentInParent<L2TrainToy>();
        if (toy != null)
        {
            manager?.TryGiveToyToChild(toy);
        }
    }
}
