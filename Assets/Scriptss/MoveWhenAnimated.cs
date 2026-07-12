using UnityEngine;
using System.Collections.Generic; // Required for the Queue!

public class MoveWhenAnimated : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Drag the empty GameObjects marking his path here.")]
    public Transform[] waypoints;
    public float moveSpeed = 1.0f;

    [Header("Animation Settings")]
    public Animator characterAnimator;
    
    public string startWalkingTrigger; 
    public string stopWalkingTrigger;

    private Transform currentTarget;
    private bool isMoving = false;
    
    // --- NEW: A queue to hold multiple points in memory! ---
    private Queue<int> waypointQueue = new Queue<int>();

    private void Update()
    {
        if (!isMoving || currentTarget == null) return;

        // 1. Look directly at the current target
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0; 
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 2. Slide towards the current target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // 3. Check if he reached his current mark
        if (Vector3.Distance(transform.position, currentTarget.position) <= 0.05f)
        {
            // Are there more points waiting in the queue?
            if (waypointQueue.Count > 0)
            {
                // Instantly grab the next point and keep walking!
                currentTarget = waypoints[waypointQueue.Dequeue()];
            }
            else
            {
                // No more points! Stop here.
                isMoving = false; 
                transform.rotation = currentTarget.rotation; // Snap to final rotation

                if (characterAnimator != null && !string.IsNullOrEmpty(stopWalkingTrigger))
                {
                    if (!string.IsNullOrEmpty(startWalkingTrigger))
                        characterAnimator.ResetTrigger(startWalkingTrigger);
                        
                    characterAnimator.SetTrigger(stopWalkingTrigger);
                }
            }
        }
    }

    // Keep the old function so your previous single-point dialogue lines don't break!
    public void WalkToWaypoint(int index)
    {
        WalkSequence(index.ToString()); 
    }

    // --- NEW: The continuous sequence function! ---
    public void WalkSequence(string sequence)
    {
        waypointQueue.Clear();
        
        // Split the string by commas (e.g., "1,2" becomes "1" and "2")
        string[] parts = sequence.Split(',');
        
        foreach (string p in parts)
        {
            if (int.TryParse(p.Trim(), out int index))
            {
                if (index >= 0 && index < waypoints.Length)
                {
                    waypointQueue.Enqueue(index); // Add it to the list of places to go
                }
                else
                {
                    Debug.LogWarning("Waypoint index " + index + " doesn't exist!");
                }
            }
        }

        // Start walking to the very first point in the queue
        if (waypointQueue.Count > 0)
        {
            currentTarget = waypoints[waypointQueue.Dequeue()];
            isMoving = true; 

            if (characterAnimator != null && !string.IsNullOrEmpty(startWalkingTrigger))
            {
                if (!string.IsNullOrEmpty(stopWalkingTrigger))
                    characterAnimator.ResetTrigger(stopWalkingTrigger);
                    
                characterAnimator.SetTrigger(startWalkingTrigger);
            }
        }
    }
}