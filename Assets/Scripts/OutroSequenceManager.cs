using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OutroSequenceManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;

    [Header("Transition Settings")]
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        if (videoPlayer != null)
        {
            // Listen for the outro video to finish playing
            videoPlayer.loopPointReached += OnOutroFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer reference is missing on OutroSequenceManager!");
        }
    }

    void OnOutroFinished(VideoPlayer vp)
    {
        Debug.Log("Outro video finished. Loading Main Menu instantly.");
        
        // Load the main menu scene immediately
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        // Always unregister event listeners when the scene changes
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnOutroFinished;
        }
    }
}