using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MeltdownHeadHoldLevel : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private XRBaseInteractable childHeadInteractable;
    [SerializeField, Min(0.1f)] private float requiredHoldSeconds = 5f;

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

        childHeadInteractable.hoverEntered.AddListener(OnHoverEntered);
        childHeadInteractable.hoverExited.AddListener(OnHoverExited);
        childHeadInteractable.selectEntered.AddListener(OnSelectEntered);
        childHeadInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        if (childHeadInteractable == null) return;

        childHeadInteractable.hoverEntered.RemoveListener(OnHoverEntered);
        childHeadInteractable.hoverExited.RemoveListener(OnHoverExited);
        childHeadInteractable.selectEntered.RemoveListener(OnSelectEntered);
        childHeadInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void Update()
    {
        if (isCompleted || activeInteractors <= 0) return;

        holdTimer += Time.deltaTime;
        UpdateUI();

        if (holdTimer >= requiredHoldSeconds)
        {
            CompleteLevel();
        }
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
            int secondsLeft = Mathf.CeilToInt(requiredHoldSeconds - holdTimer);
            secondsLeft = Mathf.Max(0, secondsLeft);
            holdProgressText.text = isCompleted
                ? "Level Complete"
                : $"Comforting child... {secondsLeft}s";
        }
    }
}
