using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Essential for loading scenes

public class ManualTutorialSequencer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    
    [Header("Sequence Settings")]
    public VideoClip[] tutorialClips;

    [Header("UI References")]
    [Tooltip("Drag your Continue button here.")]
    public GameObject continueButton;

    // --- ADDED: Scene Transition Settings ---
    [Header("Scene Transition")]
    [Tooltip("Type the EXACT name of your target scene here (case-sensitive).")]
    public string nextSceneName = "IntroScene"; 

    private int currentClipIndex = 0;

    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        
        if (tutorialClips.Length == 0)
        {
            Debug.LogError("No tutorial clips assigned!");
            return;
        }

        PlayCurrentClip();
    }

    public void NextClip()
    {
        currentClipIndex++;

        // If there are still clips left, play the next one
        if (currentClipIndex < tutorialClips.Length)
        {
            PlayCurrentClip();
        }
        else
        {
            // If the index goes past the number of clips, we are done!
            HandleTutorialComplete();
        }
    }

    private void PlayCurrentClip()
    {
        videoPlayer.clip = tutorialClips[currentClipIndex];
        videoPlayer.isLooping = true; 
        videoPlayer.Play();

        Debug.Log($"Playing Tutorial Video {currentClipIndex + 1} of {tutorialClips.Length}");
    }

    void HandleTutorialComplete()
    {
        videoPlayer.Stop();
        
        if (continueButton != null) continueButton.SetActive(false);
        
        Debug.Log($"Tutorial finished! Loading scene: {nextSceneName}");
        
        // --- UPDATED: This line now triggers the scene transition ---
        SceneManager.LoadScene(nextSceneName);
    }
}