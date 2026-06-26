using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IntroSequenceManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public GameObject bookGameObject;
    public Animator bookAnimator;
    public XRGrabInteractable grabInteractable;
    
    [Tooltip("Drag the Text Canvas GameObject here.")]
    public GameObject instructionsCanvas; 

    // --- ADDED: Reference for your Skip Button ---
    [Tooltip("Drag your Skip Button GameObject here.")]
    public GameObject skipButton;

    [Header("Transition Settings")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Time to wait in seconds after grabbing the book before the scene changes.")]
    public float transitionDelay = 5.0f; 

    void Start()
    {
        if (bookGameObject != null) bookGameObject.SetActive(false);
        if (instructionsCanvas != null) instructionsCanvas.SetActive(false);

        // Make sure the skip button is visible at the start
        if (skipButton != null) skipButton.SetActive(true);

        videoPlayer.loopPointReached += OnVideoFinished;

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnBookGrabbed);
        }
    }

    // --- ADDED: This is the method your UI Button will call ---
    public void SkipVideo()
    {
        Debug.Log("Intro skipped by player!");
        
        // Stop the video playback immediately
        videoPlayer.Stop();
        
        // Manually trigger the rest of the sequence
        OnVideoFinished(videoPlayer);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);

        // --- ADDED: Hide the skip button so it doesn't linger ---
        if (skipButton != null) skipButton.SetActive(false);

        if (bookGameObject != null)
        {
            bookGameObject.SetActive(true);
        }

        if (bookAnimator != null)
        {
            bookAnimator.SetTrigger("StartPlaying");
        }

        if (instructionsCanvas != null)
        {
            instructionsCanvas.SetActive(true);
        }
    }

    void OnBookGrabbed(SelectEnterEventArgs args)
    {
        if (bookAnimator != null) bookAnimator.enabled = false;
        if (instructionsCanvas != null) instructionsCanvas.SetActive(false);

        Debug.Log($"Book grabbed! Waiting {transitionDelay} seconds before loading menu...");
        StartCoroutine(TransitionToMenuSequence());
    }

    System.Collections.IEnumerator TransitionToMenuSequence()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
        if (grabInteractable != null) grabInteractable.selectEntered.RemoveListener(OnBookGrabbed);
    }
}