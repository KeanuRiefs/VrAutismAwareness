using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

public class SensoryChild : MonoBehaviour
{
    [Header("Socket References")]
    public XRSocketInteractor headSocket;
    public XRSocketInteractor eyeSocket;

    [Header("UI (Optional)")]
    [SerializeField] private TMP_Text statusText;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private bool hasHeadphones;
    private bool hasGlasses;
    private bool completed;

    void OnEnable()
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

    void OnDisable()
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
        hasHeadphones = true;
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
        CheckStatus();
    }

    private void OnGlassesRemoved(SelectExitEventArgs args)
    {
        hasGlasses = false;
        CheckStatus();
    }

    void CheckStatus()
    {
        RefreshUI();

        if (!completed && hasHeadphones && hasGlasses)
        {
            completed = true;
            RefreshUI();
            onLevelCompleted?.Invoke();
            Debug.Log("L3 Complete: Headphones and sunglasses applied.");
        }
    }

    private void RefreshUI()
    {
        if (statusText == null) return;

        if (completed)
        {
            statusText.text = "Level Complete";
            return;
        }

        if (!hasHeadphones && !hasGlasses)
        {
            statusText.text = "Put headphones and sunglasses on the child";
        }
        else if (!hasHeadphones)
        {
            statusText.text = "Headphones missing";
        }
        else if (!hasGlasses)
        {
            statusText.text = "Sunglasses missing";
        }
    }
}
