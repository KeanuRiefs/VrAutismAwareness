using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour 
{
    public float delayBetweenChars = 0.05f; 
    private TMP_Text textMesh;
    private string originalText;

    void Awake() 
    {
        textMesh = GetComponent<TMP_Text>();
        
        // 1. Save what you typed in the Inspector
        originalText = textMesh.text; 
        
        // 2. Clear the text so it's empty at the start
        textMesh.text = "";           
    }

    void Start() 
    {
        // 3. Start the typing effect immediately
        if (!string.IsNullOrEmpty(originalText))
        {
            StartCoroutine(TypeText());
        }
    }

    IEnumerator TypeText() 
    {
        // Optional: Wait a split second before starting
        yield return new WaitForSeconds(0.5f);

        foreach (char letter in originalText.ToCharArray()) 
        {
            textMesh.text += letter; 
            yield return new WaitForSeconds(delayBetweenChars); 
        }
    }
}