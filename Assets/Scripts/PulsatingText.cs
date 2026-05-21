using UnityEngine;
using TMPro; // Required for TextMesh Pro

public class PulsatingText : MonoBehaviour
{
    // Fixed: Removed spaces from the enum identifiers
    public enum PulseMode { ScaleOnly, AlphaOnly, Both }

    [Header("Mode Settings")]
    [Tooltip("Choose whether the text changes size, fades in/out, or does both.")]
    public PulseMode mode = PulseMode.ScaleOnly;
    
    [Tooltip("How fast the text pulses.")]
    public float pulseSpeed = 3.0f;

    [Header("Scale Boundaries")]
    [Tooltip("The smallest size multiplier (e.g., 0.9 = 90% of original size).")]
    public float minScaleMultiplier = 0.9f;
    [Tooltip("The largest size multiplier (e.g., 1.1 = 110% of original size).")]
    public float maxScaleMultiplier = 1.1f;

    [Header("Alpha Boundaries")]
    [Range(0f, 1f)] public float minAlpha = 0.3f;
    [Range(0f, 1f)] public float maxAlpha = 1.0f;

    private TMP_Text textComponent;
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        // Automatically grab the TextMeshPro component on this GameObject
        textComponent = GetComponent<TMP_Text>();
        
        if (textComponent == null)
        {
            Debug.LogError($"PulsatingText script requires a TextMeshPro component on {gameObject.name}!");
            enabled = false;
            return;
        }

        // Store original baseline values
        originalScale = transform.localScale;
        originalColor = textComponent.color;
    }

    void Update()
    {
        // Math representation of a clean continuous wave looping smoothly between 0 and 1
        float rawSin = Mathf.Sin(Time.time * pulseSpeed);
        float normalizedWave = (rawSin + 1.0f) / 2.0f; 

        // Handle Size Scale Changes
        if (mode == PulseMode.ScaleOnly || mode == PulseMode.Both)
        {
            float currentScaleMultiplier = Mathf.Lerp(minScaleMultiplier, maxScaleMultiplier, normalizedWave);
            transform.localScale = originalScale * currentScaleMultiplier;
        }
        else
        {
            // Reset to original scale if mode is changed at runtime
            transform.localScale = originalScale;
        }

        // Handle Transparency/Alpha Changes
        if (mode == PulseMode.AlphaOnly || mode == PulseMode.Both)
        {
            float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, normalizedWave);
            Color newColor = textComponent.color;
            newColor.a = currentAlpha;
            textComponent.color = newColor;
        }
        else
        {
            // Reset to original color if mode is changed at runtime
            textComponent.color = originalColor;
        }
    }

    void OnDisable()
    {
        // Safety check to restore defaults if the script or object gets turned off
        if (textComponent != null)
        {
            transform.localScale = originalScale;
            textComponent.color = originalColor;
        }
    }
}