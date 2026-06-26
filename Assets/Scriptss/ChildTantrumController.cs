using UnityEngine;

public class ChildTantrumController : MonoBehaviour
{
    [Header("Components")]
    public Animator childAnimator;
    
    // Optional: Add this back only if you want a crying sound to play during the tantrum
    // public AudioSource tantrumCryingAudio; 

    // Your DialogueSequencer (or a UnityEvent) will call this method directly
    public void TriggerTantrum()
    {
        if (childAnimator != null)
        {
            childAnimator.SetTrigger("StartTantrum");
            Debug.Log("Tantrum Animation Triggered!");
        }

        // if (tantrumCryingAudio != null) tantrumCryingAudio.Play();
    }

    /// <blockquotes>
    /// Call this method when the level is completed to calm the child down.
    /// </blockquotes>
    public void TriggerCalmDown()
    {
        if (childAnimator != null)
        {
            childAnimator.SetTrigger("CalmDown");
            Debug.Log("Calm Down Animation Triggered!");
        }
        
        // If you had audio playing, you could stop it here:
        // if (tantrumCryingAudio != null && tantrumCryingAudio.isPlaying) tantrumCryingAudio.Stop();
    }
}