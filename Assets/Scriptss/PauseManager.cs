using UnityEngine;

public class VRPauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel; // The Panel containing Resume/Quit buttons
    public GameObject pauseButton;    // The floating button used to pause

    // 1. Call this from your floating "Pause" button
    public void PauseGame()
    {
        Time.timeScale = 0f;            // Stop time
        pauseMenuPanel.SetActive(true);  // Show the menu
        pauseButton.SetActive(false);    // Hide the pause button
    }

    // 2. Call this from the "Resume" button inside your menu
    public void ResumeGame()
    {
        Time.timeScale = 1f;            // Start time
        pauseMenuPanel.SetActive(false); // Hide the menu
        pauseButton.SetActive(true);     // Show the pause button again
    }
}