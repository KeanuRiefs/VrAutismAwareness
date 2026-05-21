using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Required for Unity 6 XRIT

public class IntroSequenceManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public GameObject bookGameObject;
    public Animator bookAnimator;
    public XRGrabInteractable grabInteractable;

    [Header("Transition Settings")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Time to wait in seconds after grabbing the book before the scene changes.")]
    public float transitionDelay = 5.0f; 

    void Start()
    {
        // Make sure the book is hidden at the start
        if (bookGameObject != null) bookGameObject.SetActive(false);

        // Listen for when the video ends
        videoPlayer.loopPointReached += OnVideoFinished;

        // Listen for when the player grabs the book
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnBookGrabbed);
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 1. Hide or turn off the video player view
        videoPlayer.gameObject.SetActive(false);

        // 2. Reveal the book in the white scene
        if (bookGameObject != null)
        {
            bookGameObject.SetActive(true);
        }

        // 3. Trigger the book's animation clip
        if (bookAnimator != null)
        {
            bookAnimator.SetTrigger("StartPlaying");
        }
    }

    void OnBookGrabbed(SelectEnterEventArgs args)
    {
        // Stop the animation so it doesn't fight the player's physical tracking hands
        if (bookAnimator != null) bookAnimator.enabled = false;

        Debug.Log($"Book grabbed! Waiting {transitionDelay} seconds before loading menu...");
        
        // Trigger the delayed transition sequence
        StartCoroutine(TransitionToMenuSequence());
    }

    System.Collections.IEnumerator TransitionToMenuSequence()
    {
        // --- 5 SECOND DELAY HAPPENS HERE ---
        // This pauses the code execution inside this block for 5 seconds
        yield return new WaitForSeconds(transitionDelay);
        
        // Load your main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        // Clean up event listeners when leaving the scene
        if (videoPlayer != null) videoPlayer.loopPointReached -= OnVideoFinished;
        if (grabInteractable != null) grabInteractable.selectEntered.RemoveListener(OnBookGrabbed);
    }
}