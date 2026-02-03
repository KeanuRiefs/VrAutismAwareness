using UnityEngine;

public class BearHandover : MonoBehaviour
{
    public Animator bearAnimator; 
    public GameObject pressEPrompt; // Drag your 'PressE' UI object here!

    void Start()
    {
        if (bearAnimator == null) bearAnimator = GetComponent<Animator>();
        
        // Ensure the prompt is hidden when the game starts
        if (pressEPrompt != null) pressEPrompt.SetActive(false); 
    }

    public void StartHandover()
    {
        bearAnimator.SetTrigger("StartHandover"); 
    }

    public void PauseForPlayer() 
    {
        bearAnimator.speed = 0f; 
        
        // SHOW the prompt when the animation pauses
        if (pressEPrompt != null) pressEPrompt.SetActive(true); 
    }

    public void ResumeAnimation() 
    {
        bearAnimator.speed = 1f;
        
        // HIDE the prompt once the player interacts
        if (pressEPrompt != null) pressEPrompt.SetActive(false); 
    }
}