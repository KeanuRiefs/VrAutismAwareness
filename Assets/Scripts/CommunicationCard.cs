using UnityEngine;

public class CommunicationCard : MonoBehaviour
{
    [SerializeField] private CommunicationLevelManager levelManager;
    [SerializeField] private bool isCorrectCard;

    private void Start()
    {
        if (levelManager != null)
        {
            levelManager.RegisterCard();
        }
    }

    public void PresentToChild()
    {
        if (levelManager == null) return;
        levelManager.OnCardPresented(isCorrectCard);
    }
}
