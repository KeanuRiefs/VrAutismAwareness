using UnityEngine;

public class Level4MasterManager : MonoBehaviour
{
    [Header("Task Sequence")]
    [Tooltip("Drag L1_TaskRoot, L2_TaskRoot, and L3_TaskRoot here in order.")]
    [SerializeField] private GameObject[] taskRoots;

    [Header("Final Sequence")]
    [Tooltip("Drag the GameObject holding your final dialogue UI here.")]
    [SerializeField] private GameObject finalDialogueContainer;

    private int currentTaskIndex = 0;

    private void Start()
    {
        // 1. Hide the final dialogue to start
        if (finalDialogueContainer != null) finalDialogueContainer.SetActive(false);

        // 2. Turn off all tasks to ensure a clean slate
        foreach (var task in taskRoots)
        {
            if (task != null) task.SetActive(false);
        }

        // 3. Turn on ONLY the very first task
        if (taskRoots.Length > 0 && taskRoots[0] != null)
        {
            taskRoots[0].SetActive(true);
            Debug.Log("Master: Starting Task 1");
        }
    }

    // --- Call this from your existing OnLevelCompleted Unity Events! ---
    public void CompleteCurrentTask()
    {
        // 1. Turn off the task we just finished
        if (currentTaskIndex < taskRoots.Length && taskRoots[currentTaskIndex] != null)
        {
            taskRoots[currentTaskIndex].SetActive(false);
        }

        // 2. Move to the next task in the list
        currentTaskIndex++;

        // 3. Check if there are more tasks left
        if (currentTaskIndex < taskRoots.Length)
        {
            if (taskRoots[currentTaskIndex] != null)
            {
                taskRoots[currentTaskIndex].SetActive(true);
                Debug.Log("Master: Starting Task " + (currentTaskIndex + 1));
            }
        }
        else
        {
            // 4. No more tasks! Trigger the final dialogue.
            Debug.Log("Master: All tasks complete! Starting final dialogue.");
            StartFinalDialogue();
        }
    }

    private void StartFinalDialogue()
    {
        // Turn on the final dialogue UI. 
        // Assuming your DialougeSequencer runs on Start(), this will immediately play it!
        if (finalDialogueContainer != null) 
        {
            finalDialogueContainer.SetActive(true);
        }
    }
}