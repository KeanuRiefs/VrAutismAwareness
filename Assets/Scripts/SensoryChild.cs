using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SensoryChild : MonoBehaviour
{
    [Header("Socket References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor headSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor eyeSocket;

    // States for your VR Autism Awareness Project
    private bool hasHeadphones = false;
    private bool hasGlasses = false;

    void OnEnable()
    {
        // Subscribe to socket events when the script is enabled
        if (headSocket != null)
        {
            headSocket.selectEntered.AddListener(OnHeadphonesAttached);
            headSocket.selectExited.AddListener(OnHeadphonesRemoved);
        }

        if (eyeSocket != null)
        {
            eyeSocket.selectEntered.AddListener(OnGlassesAttached);
            eyeSocket.selectExited.AddListener(OnGlassesRemoved);
        }
    }

    void OnDisable()
    {
        // Clean up listeners to prevent memory leaks or errors
        if (headSocket != null)
        {
            headSocket.selectEntered.RemoveListener(OnHeadphonesAttached);
            headSocket.selectExited.RemoveListener(OnHeadphonesRemoved);
        }

        if (eyeSocket != null)
        {
            eyeSocket.selectEntered.RemoveListener(OnGlassesAttached);
            eyeSocket.selectExited.RemoveListener(OnGlassesRemoved);
        }
    }

    // --- Interaction Handlers ---

    private void OnHeadphonesAttached(SelectEnterEventArgs args)
    {
        hasHeadphones = true;
        Debug.Log("Audio stress reduced: Headphones equipped.");
        CheckStatus();
    }

    private void OnHeadphonesRemoved(SelectExitEventArgs args)
    {
        hasHeadphones = false;
        CheckStatus();
    }

    private void OnGlassesAttached(SelectEnterEventArgs args)
    {
        hasGlasses = true;
        Debug.Log("Visual stress reduced: Sunglasses equipped.");
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
            // This is where you trigger your "Level Complete" logic
            Debug.Log("LEVEL SUCCESS: The child is now regulated and comfortable.");
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        // Add your final year project transition here (e.g., Load next scene, play audio)
    }
}