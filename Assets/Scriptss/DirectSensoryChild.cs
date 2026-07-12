using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DirectSensoryChild : MonoBehaviour
{
    [Header("Socket References")]
    [SerializeField] private XRSocketInteractor headSocket;
    [SerializeField] private XRSocketInteractor eyeSocket;

    [Header("Required Items")]
    [SerializeField] private XRBaseInteractable requiredHeadphone;
    [SerializeField] private XRBaseInteractable requiredGlasses;

    [Header("Animation")]
    [SerializeField] private Animator childAnimator;

    // --- SEQUENTIAL TEXT INSTRUCTIONS ---
    [Header("Sequential Instructions")]
    [Tooltip("The text GameObject saying 'Attach headphone to Aiman'")]
    [SerializeField] private GameObject attachHeadphoneText;
    
    [Tooltip("The text GameObject saying 'Attach sunglasses to Aiman'")]
    [SerializeField] private GameObject attachSunglassesText;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool hasHeadphones;
    private bool hasGlasses;
    private bool completed;

    private void Start()
    {
        // Instantly start the process on load
        
        // 1. Hide the second instruction just in case it was left on in the editor
        if (attachSunglassesText != null) attachSunglassesText.SetActive(false);

        // 2. Trigger the first animation
        if (childAnimator != null && !completed)
        {
            childAnimator.SetTrigger("StartHeadphone");
        }

        // 3. Show the first instruction
        if (attachHeadphoneText != null && !hasHeadphones) 
        {
            attachHeadphoneText.SetActive(true);
        }
    }

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
        
        if (hasHeadphones)
        {
            // HEADPHONE ATTACHED: Hide headphone text, show sunglasses text
            if (attachHeadphoneText != null) attachHeadphoneText.SetActive(false);
            if (attachSunglassesText != null && !hasGlasses) attachSunglassesText.SetActive(true);

            if (!hasGlasses && childAnimator != null && !completed)
            {
                childAnimator.SetTrigger("StartSunglasses");
            }
        }

        CheckStatus();
    }

    private void OnHeadphonesRemoved(SelectExitEventArgs args)
    {
        // If player takes them off, revert the texts
        if (IsRequiredItem(args.interactableObject, requiredHeadphone))
        {
            hasHeadphones = false;
            if (attachHeadphoneText != null) attachHeadphoneText.SetActive(true);
            if (attachSunglassesText != null) attachSunglassesText.SetActive(false);
        }
    }

    private void OnGlassesAttached(SelectEnterEventArgs args)
    {
        hasGlasses = IsRequiredItem(args.interactableObject, requiredGlasses);
        
        if (hasGlasses)
        {
            // SUNGLASSES ATTACHED: Hide the sunglasses text
            if (attachSunglassesText != null) attachSunglassesText.SetActive(false);
        }

        CheckStatus();
    }

    private void OnGlassesRemoved(SelectExitEventArgs args)
    {
        // If player takes them off, revert the text
        if (IsRequiredItem(args.interactableObject, requiredGlasses) && hasHeadphones)
        {
            hasGlasses = false;
            if (attachSunglassesText != null) attachSunglassesText.SetActive(true);
        }
    }

    // Uses the reliable GameObject comparison we set up previously
    private static bool IsRequiredItem(IXRSelectInteractable selectedItem, XRBaseInteractable requiredItem)
    {
        if (requiredItem == null || selectedItem == null) return false;
        return selectedItem.transform.gameObject == requiredItem.gameObject;
    }

    private void CheckStatus()
    {
        if (!completed && hasHeadphones && hasGlasses)
        {
            completed = true;
            
            // Just to be absolutely safe, force both texts off when level is done
            if (attachHeadphoneText != null) attachHeadphoneText.SetActive(false);
            if (attachSunglassesText != null) attachSunglassesText.SetActive(false);

            if (childAnimator != null)
            {
                childAnimator.SetTrigger("StartDrink");
            }

            onLevelCompleted?.Invoke();
            Debug.Log("L3 Complete: Headphone and Glasses attached.");
        }
    }
}