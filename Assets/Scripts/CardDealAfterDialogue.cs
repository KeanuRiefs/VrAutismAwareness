using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealAfterDialogue : MonoBehaviour
{
    [Header("Cards attached to the bear before dialogue ends")]
    [SerializeField] private List<GameObject> cards = new List<GameObject>();

    [Header("Where cards should appear for the player")]
    [SerializeField] private Transform playerCardRoot;
    [SerializeField] private List<Transform> cardSpawnPoints = new List<Transform>();

    [Header("Timing")]
    [SerializeField, Min(0f)] private float initialDelay = 0.15f;
    [SerializeField, Min(0f)] private float delayBetweenCards = 0.1f;

    [Header("Optional")]
    [SerializeField] private bool detachFromBear = true;

    private bool hasDealt;

    private void Start()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
            {
                cards[i].SetActive(false);
            }
        }
    }

    public void DealCardsToPlayer()
    {
        if (hasDealt) return;
        hasDealt = true;
        StartCoroutine(DealRoutine());
    }

    private IEnumerator DealRoutine()
    {
        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            if (card == null) continue;

            if (detachFromBear)
            {
                card.transform.SetParent(null, true);
            }

            Transform spawn = i < cardSpawnPoints.Count ? cardSpawnPoints[i] : null;
            if (spawn != null)
            {
                card.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            }
            else if (playerCardRoot != null)
            {
                card.transform.SetParent(playerCardRoot, true);
            }

            card.SetActive(true);

            if (delayBetweenCards > 0f)
            {
                yield return new WaitForSeconds(delayBetweenCards);
            }
        }

        Debug.Log("L2: All cards dealt to player after dialogue.");
    }
}
