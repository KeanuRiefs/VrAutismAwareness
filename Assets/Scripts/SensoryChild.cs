using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Required for XR events

public class SensoryChild : MonoBehaviour
{
    public Renderer childRenderer;
    
    // Reference your Socket Interactors here
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor headSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor eyeSocket;

    private bool hasHeadphones = false;
    private bool hasGlasses = false;

    void Start()
    {
        childRenderer.material.color = Color.red;

        // Subscribe to socket events
        headSocket.selectEntered.AddListener(OnHeadphonesAttached);
        eyeSocket.selectEntered.AddListener(OnGlassesAttached);
        
        // Optional: Subscribe to removal if you want them to get stressed again
        headSocket.selectExited.AddListener(OnHeadphonesRemoved);
        eyeSocket.selectExited.AddListener(OnGlassesRemoved);
    }

    // --- Event Handlers ---

    private void OnHeadphonesAttached(SelectEnterEventArgs args)
    {
        hasHeadphones = true;
        CheckStatus();
    }

    private void OnGlassesAttached(SelectEnterEventArgs args)
    {
        hasGlasses = true;
        CheckStatus();
    }

    private void OnHeadphonesRemoved(SelectExitEventArgs args)
    {
        hasHeadphones = false;
        CheckStatus();
    }

    private void OnGlassesRemoved(SelectExitEventArgs args)
    {
        hasGlasses = false;
        CheckStatus();
    }

    void CheckStatus()
    {
        if (hasHeadphones && hasGlasses)
        {
            childRenderer.material.color = Color.green;
            Debug.Log("LEVEL COMPLETE: Child is fully regulated!");
        }
        else if (hasHeadphones || hasGlasses)
        {
            childRenderer.material.color = Color.yellow;
        }
        else
        {
            childRenderer.material.color = Color.red;
        }
    }
}