using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoSequencer : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    
    [Header("Settings")]
    public VideoClip[] tutorialClips; // Drop your clips here in order
    private int currentClipIndex = 0;

    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();

        // Subscribe to the event that triggers when a video ends
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start the first video
        PlayCurrentClip();
    }

    void PlayCurrentClip()
    {
        if (currentClipIndex < tutorialClips.Length)
        {
            videoPlayer.clip = tutorialClips[currentClipIndex];
            videoPlayer.Play();
            Debug.Log($"Playing Tutorial Clip: {currentClipIndex + 1} of {tutorialClips.Length}");
        }
        else
        {
            HandleTutorialComplete();
        }
    }

    void OnVideoFinished(VideoPlayer source)
    {
        currentClipIndex++;
        PlayCurrentClip();
    }

    void HandleTutorialComplete()
    {
        Debug.Log("All tutorial videos finished!");
        // Logic to unlock Level 1 or show a 'Continue' button goes here
    }

    private void OnDestroy()
    {
        // Clean up the event subscription
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}