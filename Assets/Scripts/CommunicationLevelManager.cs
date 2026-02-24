using UnityEngine;
using UnityEngine.Events;

public class CommunicationLevelManager : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private int expectedCardCount = 4;

    [Header("Comfort Object")]
    [SerializeField] private GameObject comfortObjectOnShelf;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onWrongCardPresented;
    [SerializeField] private UnityEvent onCorrectCardPresented;
    [SerializeField] private UnityEvent onComfortObjectPickedUp;
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool correctCardAccepted;
    private bool comfortObjectPickedUp;
    private bool isCompleted;
    private int registeredCards;

    private void Start()
    {
        if (comfortObjectOnShelf != null)
        {
            comfortObjectOnShelf.SetActive(false);
        }
    }

    public void RegisterCard()
    {
        registeredCards++;
        if (registeredCards > expectedCardCount)
        {
            Debug.LogWarning("More cards registered than expected for L2.");
        }
    }

    public void OnCardPresented(bool isCorrectCard)
    {
        if (isCompleted || correctCardAccepted) return;

        if (!isCorrectCard)
        {
            onWrongCardPresented?.Invoke();
            Debug.Log("Wrong PECS card shown to child.");
            return;
        }

        correctCardAccepted = true;
        onCorrectCardPresented?.Invoke();
        Debug.Log("Correct PECS card shown. Pick the shelf object.");

        if (comfortObjectOnShelf != null)
        {
            comfortObjectOnShelf.SetActive(true);
        }
    }

    public void OnComfortObjectPickedUp()
    {
        if (!correctCardAccepted || isCompleted) return;

        comfortObjectPickedUp = true;
        onComfortObjectPickedUp?.Invoke();
    }

    public void OnComfortObjectGivenToChild()
    {
        if (!correctCardAccepted || !comfortObjectPickedUp || isCompleted) return;

        isCompleted = true;
        onLevelCompleted?.Invoke();
        Debug.Log("L2 Complete: Correct card and comfort object delivered.");
    }
}
