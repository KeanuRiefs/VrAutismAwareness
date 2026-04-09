using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class ManualTutorialSequencer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    
    [Header("Sequence Settings")]
    public VideoClip[] tutorialClips;
    [Tooltip("Set duration in seconds for each clip above. Must match the number of clips.")]
    public float[] manualDurations; 

    private int currentClipIndex = 0;

    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        
        // Safety check: Make sure we have durations for our clips
        if (tutorialClips.Length != manualDurations.Length)
        {
            Debug.LogError("Mismatch! You need the same number of Durations as Clips.");
            return;
        }

        StartCoroutine(PlayTutorialSequence());
    }

    IEnumerator PlayTutorialSequence()
    {
        while (currentClipIndex < tutorialClips.Length)
        {
            // Set and Play the clip
            videoPlayer.clip = tutorialClips[currentClipIndex];
            videoPlayer.Play();

            Debug.Log($"Playing {videoPlayer.clip.name} for {manualDurations[currentClipIndex]} seconds.");

            // Wait for your specific manual duration
            yield return new WaitForSeconds(manualDurations[currentClipIndex]);

            // Move to next clip
            currentClipIndex++;
        }

        HandleTutorialComplete();
    }

    void HandleTutorialComplete()
    {
        videoPlayer.Stop();
        Debug.Log("Manual Tutorial Sequence Finished!");
        // Add logic here to load Level 1
    }
}