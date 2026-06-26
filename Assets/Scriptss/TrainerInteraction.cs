using UnityEngine;

public class TrainerInteraction : MonoBehaviour
{
    public SimulatedVRPlayer playerController;
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Card Visuals")]
    public GameObject heldCard; // The card in your hand
    
    // Optional: A card that appears in the child's hand later
    // public GameObject childsCard; 

    void Start()
    {
        // Ensure we start with the card in hand
        if (heldCard != null) heldCard.SetActive(true);
    }

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            ChildBehavior child = hit.collider.GetComponent<ChildBehavior>();

            // Check if we hit the child AND the child is currently crying
            if (child != null && child.isCrying)
            {
                // Check if we are crouching (Eye level connection)
                if (playerController.IsCrouching())
                {
                    Debug.Log("UI: Press 'E' to give PECS Card");
                    
                    if (Input.GetKeyDown(interactKey))
                    {
                        GiveCard(child);
                    }
                }
                else
                {
                    Debug.Log("UI: Too tall! Crouch to eye level.");
                }
            }
        }
    }

    void GiveCard(ChildBehavior child)
    {
        // 1. Tell the child to calm down
        child.ReceiveCard();

        // 2. Hide the card from our hand (we gave it away)
        if (heldCard != null)
        {
            heldCard.SetActive(false);
        }
    }
}