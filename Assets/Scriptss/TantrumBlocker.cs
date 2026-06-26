using UnityEngine;
using UnityEngine.Events;

public class TantrumBlocker : MonoBehaviour
{
    public float requiredHoldTime = 5f;
    private float currentHoldTime = 0f;
    
    private int handsInZone = 0; // Tracks if one or both hands are blocking
    private bool isSuccess = false;

    public UnityEvent OnBlockSuccess; // Trigger this when the 5s is up

    void OnTriggerEnter(Collider foreign)
    {
        if (foreign.CompareTag("PlayerHand"))
        {
            handsInZone++;
        }
    }

    void OnTriggerExit(Collider foreign)
    {
        if (foreign.CompareTag("PlayerHand"))
        {
            handsInZone--;
            // Optional: Reset timer if they let go? 
            // currentHoldTime = 0; 
        }
    }

    void Update()
    {
        // If at least one hand is blocking, start the timer
        if (handsInZone > 0 && !isSuccess)
        {
            currentHoldTime += Time.deltaTime;

            if (currentHoldTime >= requiredHoldTime)
            {
                CompleteObjective();
            }
        }
        else if (handsInZone == 0)
        {
            // Reset timer if the player stops blocking entirely
            currentHoldTime = Mathf.Max(0, currentHoldTime - Time.deltaTime);
        }
    }

    void CompleteObjective()
    {
        isSuccess = true;
        Debug.Log("Objective Successful: Child is safe.");
        OnBlockSuccess.Invoke();
        
        // Switch animation to "Calm" or move to next stage here
    }
}