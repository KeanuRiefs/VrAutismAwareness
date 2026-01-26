using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueSequencer : MonoBehaviour 
{
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string[] dialogueLines; // Put all your sentences here!
    
    [Header("Settings")]
    public float typingSpeed = 0.05f;
    
    private TMP_Text textMesh;
    private int currentIndex = 0;
    private bool isTyping = false;

    void Awake() 
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.text = ""; 
    }

    void Start() 
    {
        // Start the first line of dialogue
        DisplayNextLine();
    }

    void Update()
    {
        // When the player presses Space or Clicks, show the next line
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!isTyping) 
            {
                DisplayNextLine();
            }
        }
    }

    public void DisplayNextLine()
    {
        if (currentIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeText(dialogueLines[currentIndex]));
            currentIndex++;
        }
        else
        {
            // Optional: What happens when the dialogue is finished?
            textMesh.text = ""; 
            Debug.Log("End of dialogue.");
        }
    }

    IEnumerator TypeText(string line) 
    {
        isTyping = true;
        textMesh.text = ""; 

        foreach (char letter in line.ToCharArray()) 
        {
            textMesh.text += letter; 
            yield return new WaitForSeconds(typingSpeed); 
        }

        isTyping = false;
    }
}