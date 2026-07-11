using UnityEngine;

public class MoveWhenAnimated : MonoBehaviour
{
    [Header("Where to?")]
    [Tooltip("Drag the empty GameObject marking his destination here.")]
    public Transform targetPosition;
    public float moveSpeed = 1.0f;

    [Header("Animation Settings")]
    public Animator characterAnimator;
    
    public string walkingStateName;
    public string stopWalkingTrigger;

    private bool hasArrived = false;

    private void Update()
    {
        if (characterAnimator == null || targetPosition == null) return;

        // Detects when your dialogue script triggers the walking animation
        if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(walkingStateName))
        {
            if (!hasArrived)
            {
                // 1. Look directly at the target while moving
                Vector3 direction = (targetPosition.position - transform.position).normalized;
                direction.y = 0; 
                
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                // 2. Slide towards the target
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

                // 3. Stop when he reaches the target
                if (Vector3.Distance(transform.position, targetPosition.position) <= 0.05f)
                {
                    hasArrived = true;
                    
                    // --- NEW: Snap to face whatever direction the target object is facing! ---
                    transform.rotation = targetPosition.rotation;

                    if (!string.IsNullOrEmpty(stopWalkingTrigger))
                    {
                        characterAnimator.SetTrigger(stopWalkingTrigger);
                    }
                }
            }
        }
        else
        {
            // Reset the flag if he is no longer walking, so he can walk again later!
            hasArrived = false; 
        }
    }
}