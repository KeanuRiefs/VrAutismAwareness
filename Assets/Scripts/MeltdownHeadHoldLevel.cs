using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MeltdownHeadHoldLevel : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private XRBaseInteractable childHeadInteractable;
    [SerializeField, Min(0.1f)] private float requiredHoldSeconds = 5f;
    [SerializeField] private bool countHoverAsComfort = true;
    [SerializeField] private bool countGrabAsComfort = true;

    [Header("UI (Optional)")]
    [SerializeField] private Slider holdProgressSlider;
    [SerializeField] private TMP_Text holdProgressText;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private float holdTimer;
    private bool isCompleted;
    private int activeInteractors;

    private void Reset()
    {
        childHeadInteractable = GetComponentInChildren<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (childHeadInteractable == null) return;

        if (countHoverAsComfort)
        {
            childHeadInteractable.hoverEntered.AddListener(OnHoverEntered);
            childHeadInteractable.hoverExited.AddListener(OnHoverExited);
        }

        if (countGrabAsComfort)
        {
            childHeadInteractable.selectEntered.AddListener(OnSelectEntered);
            childHeadInteractable.selectExited.AddListener(OnSelectExited);
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (childHeadInteractable == null) return;

        if (countHoverAsComfort)
        {
            childHeadInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            childHeadInteractable.hoverExited.RemoveListener(OnHoverExited);
        }

        if (countGrabAsComfort)
        {
            childHeadInteractable.selectEntered.RemoveListener(OnSelectEntered);
            childHeadInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void Update()
    {
        if (isCompleted) return;

        if (activeInteractors > 0)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldSeconds)
            {
                CompleteLevel();
            }
        }

        UpdateUI();
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        activeInteractors++;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        activeInteractors = Mathf.Max(0, activeInteractors - 1);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        activeInteractors++;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        activeInteractors = Mathf.Max(0, activeInteractors - 1);
    }

    private void CompleteLevel()
    {
        isCompleted = true;
        holdTimer = requiredHoldSeconds;
        UpdateUI();
        onLevelCompleted?.Invoke();
        Debug.Log("L1 Complete: Child was comforted for 5 seconds.");
    }

    private void UpdateUI()
    {
        float progress = Mathf.Clamp01(holdTimer / requiredHoldSeconds);

        if (holdProgressSlider != null)
        {
            holdProgressSlider.value = progress;
        }

        if (holdProgressText != null)
        {
            int secondsLeft = Mathf.Max(0, Mathf.CeilToInt(requiredHoldSeconds - holdTimer));
            holdProgressText.text = isCompleted
                ? "Level Complete"
                : $"Comforting child... {secondsLeft}s";
        }
    }
}
