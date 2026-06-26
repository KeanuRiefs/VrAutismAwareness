using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Type the exact name of your Outro scene asset.")]
    public string outroSceneName = "OutroScene";

    // Method 1: Triggered by physical VR movement (Player walking into a zone)
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the VR Player Rig
        // (Make sure your XR Origin/Rig has the "Player" tag assigned)
        if (other.CompareTag("Player"))
        {
            LoadOutro();
        }
    }

    // Method 2: Public method you can call from other objective scripts
    public void LoadOutro()
    {
        Debug.Log("Transitioning to Outro scene...");
        SceneManager.LoadScene(outroSceneName);
    }
}