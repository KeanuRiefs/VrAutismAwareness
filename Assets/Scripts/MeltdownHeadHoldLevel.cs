using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



public class MeltdownHeadHoldLevel : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private XRBaseInteractable childHeadInteractable;
    [SerializeField, Min(0.1f)] private float requiredHoldSeconds = 5f;


    [Header("UI (Optional)")]
    [SerializeField] private Slider holdProgressSlider;
    [SerializeField] private TMP_Text holdProgressText;

    [Header("Level Events")]
    [SerializeField] private UnityEvent onLevelCompleted;

    private float holdTimer;
    private bool isCompleted;


    private bool isBeingHeld;


    private void OnEnable()
    {
        if (childHeadInteractable == null) return;



        // We use Select (Grab) instead of Hover
        childHeadInteractable.selectEntered.AddListener(OnGrabStarted);
        childHeadInteractable.selectExited.AddListener(OnGrabEnded);

    }

    private void OnDisable()
    {
        if (childHeadInteractable == null) return;



        childHeadInteractable.selectEntered.RemoveListener(OnGrabStarted);
        childHeadInteractable.selectExited.RemoveListener(OnGrabEnded);

    }

    private void Update()
    {
        if (isCompleted) return;



        if (isBeingHeld)
        {
            holdTimer += Time.deltaTime;
            

            if (holdTimer >= requiredHoldSeconds)
            {
                CompleteLevel();
            }
        }


        else
        {
            // Optional: Reset timer if they let go before 5 seconds
            holdTimer = Mathf.Max(0, holdTimer - (Time.deltaTime * 2f)); 
        }


        UpdateUI();
    }



    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
        UnityEngine.Debug.Log("Grab started! Timer running.");
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        isBeingHeld = false;
        UnityEngine.Debug.Log("Grab released! Timer paused/resetting.");

    }

    private void CompleteLevel()
    {
        isCompleted = true;
        holdTimer = requiredHoldSeconds;


        onLevelCompleted?.Invoke();
        UnityEngine.Debug.Log("Level Complete: Child comforted successfully!");

    }

    private void UpdateUI()
    {
        float progress = Mathf.Clamp01(holdTimer / requiredHoldSeconds);


        if (holdProgressSlider != null) holdProgressSlider.value = progress;

        if (holdProgressText != null)
        {
            int secondsLeft = Mathf.CeilToInt(requiredHoldSeconds - holdTimer);
            holdProgressText.text = isCompleted ? "Success!" : $"Hold to comfort... {Mathf.Max(0, secondsLeft)}s";
        }
    }
}

