using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using UnityEngine.SceneManagement; // Required for Scene transitions

public class SensoryChild : MonoBehaviour
{
    [Header("Socket References")]
    [SerializeField] private XRSocketInteractor headSocket;
    [SerializeField] private XRSocketInteractor eyeSocket;

    [Header("Required Items")]
    [SerializeField] private XRBaseInteractable requiredHeadphone;
    [SerializeField] private XRBaseInteractable requiredGlasses;

    [Header("UI (Optional)")]
    [SerializeField] private TMP_Text statusText;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onLevelCompleted;

    [Header("Transition Settings")]
    [SerializeField] private string outroSceneName = "OutroScene"; // Type your exact Outro scene asset name here
    [SerializeField] private float transitionDelay = 4.0f; // Time in seconds to let your star VFX play before switching scenes

    private bool hasHeadphones;
    private bool hasGlasses;
    private bool completed;

    private void OnEnable()
    {
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

        RefreshUI();
    }

    private void OnDisable()
    {
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

    private void OnHeadphonesAttached(SelectEnterEventArgs args)
    {
        hasHeadphones = IsRequiredItem(args.interactableObject, requiredHeadphone);
        CheckStatus();
    }

    private void OnHeadphonesRemoved(SelectExitEventArgs args)
    {
        if (IsRequiredItem(args.interactableObject, requiredHeadphone))
        {
            hasHeadphones = false;
            CheckStatus();
        }
    }

    private void OnGlassesAttached(SelectEnterEventArgs args)
    {
        hasGlasses = IsRequiredItem(args.interactableObject, requiredGlasses);
        CheckStatus();
    }

    private void OnGlassesRemoved(SelectExitEventArgs args)
    {
        if (IsRequiredItem(args.interactableObject, requiredGlasses))
        {
            hasGlasses = false;
            CheckStatus();
        }
    }

    private static bool IsRequiredItem(IXRSelectInteractable selectedItem, XRBaseInteractable requiredItem)
    {
        if (requiredItem == null || selectedItem == null)
        {
            return false;
        }

        return selectedItem.transform == requiredItem.transform;
    }

    private void CheckStatus()
    {
        RefreshUI();

        if (!completed && hasHeadphones && hasGlasses)
        {
            completed = true;
            RefreshUI();
            
            // 1. Fire your existing UnityEvent (Plays stars particles, etc.)
            onLevelCompleted?.Invoke();
            Debug.Log("L3 Complete: Headphone and Glasses 1 attached.");

            // 2. Start the delayed transition routine to load the Outro scene
            StartCoroutine(LoadOutroRoutine());
        }
    }

    // Coroutine to handle the level completion transition delay
    private System.Collections.IEnumerator LoadOutroRoutine()
    {
        // Wait here while your celebration particles fire off
        yield return new WaitForSeconds(transitionDelay);

        Debug.Log($"Transitioning to Outro scene: {outroSceneName}");
        
        // Load the Outro video scene
        SceneManager.LoadScene(outroSceneName);
    }

    private void RefreshUI()
    {
        if (statusText == null)
        {
            return;
        }

        if (completed)
        {
            statusText.text = "Level Complete";
            return;
        }

        if (!hasHeadphones && !hasGlasses)
        {
            statusText.text = "Snap Headphone and Glasses 1 onto the child";
        }
        else if (!hasHeadphones)
        {
            statusText.text = "Headphone missing";
        }
        else if (!hasGlasses)
        {
            statusText.text = "Glasses 1 missing";
        }
    }
}