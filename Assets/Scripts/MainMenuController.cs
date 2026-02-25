using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // These methods will be linked to your VR UI Buttons
    
    public void LoadLevel1()
    {
        // Replace with your exact scene name, e.g., "Scene_L1_..."
        SceneManager.LoadScene("Scene_L1_Meltdown"); 
    }

    public void LoadLevel2()
    {
        // Replace with your exact scene name, e.g., "Scene_L2_Communication"
        SceneManager.LoadScene("Scene_L2_Communication");
    }

    public void LoadLevel3()
    {
        // Replace with your exact scene name
        SceneManager.LoadScene("Scene_L3_Sensory Overload");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Scene_Menu");
    }
}