using UnityEngine;
using UnityEngine.UI;

public class VRVolumeControl : MonoBehaviour
{
    [Header("UI References")]
    public Image volumeIndicator; // Drag the 'Volume Button' image here
    
    [Header("Settings")]
    public float volumeStep = 0.1f; // How much volume changes per click
    private float currentVolume = 0.5f; // Starts at 50%

    void Start()
    {
        // Initialize volume based on current system/game volume
        currentVolume = AudioListener.volume;
        UpdateUI();
    }

    public void IncreaseVolume()
    {
        currentVolume = Mathf.Clamp01(currentVolume + volumeStep);
        ApplyVolume();
    }

    public void DecreaseVolume()
    {
        currentVolume = Mathf.Clamp01(currentVolume - volumeStep);
        ApplyVolume();
    }

    void ApplyVolume()
    {
        AudioListener.volume = currentVolume;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (volumeIndicator != null)
        {
            // This adjusts the 'Fill' of your PNG bar
            volumeIndicator.fillAmount = currentVolume;
        }
    }
}