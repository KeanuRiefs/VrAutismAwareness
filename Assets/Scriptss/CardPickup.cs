using UnityEngine;

public class CardPickup : MonoBehaviour
{
    public BearHandover bearScript; 
    public GameObject pecsUI;       
    public float interactionDistance = 3f; // How close you must be to take it

    void Update()
    {
        // Check if the player presses E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpCard();
        }
    }

    void TryPickUpCard()
    {
        // We use a Raycast to see if the player is looking at the card
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // If the object we are looking at is THIS card
            if (hit.collider.gameObject == this.gameObject)
            {
                // 1. Tell the bear to resume his animation
                bearScript.ResumeAnimation();
                
                // 2. Hide the 3D card
                gameObject.SetActive(false);
                
                // 3. Show the PECS communication board
                pecsUI.SetActive(true);
                
                Debug.Log("Card picked up with E key!");
            }
        }
    }
}