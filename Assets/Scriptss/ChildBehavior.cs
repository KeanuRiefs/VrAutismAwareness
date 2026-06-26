using UnityEngine;

public class ChildBehavior : MonoBehaviour
{
    public bool isCrying = true;
    private Renderer childRenderer;

    [Header("Visuals")]
    public GameObject objectInHand; // The one holding by child (Starts Hidden)
    public GameObject objectOnShelf; // The one on the shelf (Starts Visible)

    void Start()
    {
        childRenderer = GetComponent<Renderer>();
        
        // SETUP: Shelf has object, Hand is empty
        if (objectOnShelf != null) objectOnShelf.SetActive(true);
        if (objectInHand != null) objectInHand.SetActive(false);

        UpdateColor();
    }

    public void ReceiveCard()
    {
        if (!isCrying) return; 

        isCrying = false;
        
        // SWAP: Hide shelf object, Show hand object
        if (objectOnShelf != null) objectOnShelf.SetActive(false);
        if (objectInHand != null) objectInHand.SetActive(true);

        Debug.Log("Child: *Sniffle* ... Thank you.");
        UpdateColor();
    }

    void UpdateColor()
    {
        if (isCrying)
            childRenderer.material.color = Color.red;
        else
            childRenderer.material.color = Color.green;
    }
}