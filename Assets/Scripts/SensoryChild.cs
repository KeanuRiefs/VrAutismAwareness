using UnityEngine;

public class SensoryChild : MonoBehaviour
{
    public Renderer childRenderer; // Drag the Capsule's MeshRenderer here
    
    // States
    private bool hasHeadphones = false;
    private bool hasGlasses = false;

    void Start()
    {
        // Start Stressed (Red Color)
        childRenderer.material.color = Color.red;
    }

    public void ReceiveItem(string itemType)
    {
        if (itemType == "Headphones")
        {
            hasHeadphones = true;
            Debug.Log("Child put on Headphones. Audio stress reduced.");
        }
        else if (itemType == "Glasses")
        {
            hasGlasses = true;
            Debug.Log("Child put on Sunglasses. Visual stress reduced.");
        }

        CheckStatus();
    }

    void CheckStatus()
    {
        // If he has BOTH items, he is fully calm (Green)
        if (hasHeadphones && hasGlasses)
        {
            childRenderer.material.color = Color.green;
            Debug.Log("LEVEL COMPLETE: Child is fully regulated!");
        }
        // If he has ONE item, he is doing better (Yellow)
        else if (hasHeadphones || hasGlasses)
        {
            childRenderer.material.color = Color.yellow;
        }
    }
}