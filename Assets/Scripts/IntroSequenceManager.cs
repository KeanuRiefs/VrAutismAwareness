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
    
    // ADDED: Reference for your UI Text Canvas
    [Tooltip("Drag the Text Canvas GameObject here.")]
    public GameObject instructionsCanvas; 

    [Header("Transition Settings")]
    public string mainMenuSceneName = "MainMenu";
    [Tooltip("Time to wait in seconds after grabbing the book before the scene changes.")]
    public float transitionDelay = 5.0f; 

    void Start()
    {
        // Make sure the book is hidden at the start
        if (bookGameObject != null) bookGameObject.SetActive(false);

        // ADDED: Make sure the text canvas is hidden at the start
        if (instructionsCanvas != null) instructionsCanvas.SetActive(false);

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

        // ADDED: 4. Reveal the "[Grab the book!]" text canvas now!
        if (instructionsCanvas != null)
        {
            instructionsCanvas.SetActive(true);
        }
    }

    void OnBookGrabbed(SelectEnterEventArgs args)
    {
        // Stop the animation so it doesn't fight the player's physical tracking hands
        if (bookAnimator != null) bookAnimator.enabled = false;

        // Optional: Hide the instructions canvas when grabbed so it cleans up the screen
        if (instructionsCanvas != null) instructionsCanvas.SetActive(false);

        Debug.Log($"Book grabbed! Waiting {transitionDelay} seconds before loading menu...");
        
        // Trigger the delayed transition sequence
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