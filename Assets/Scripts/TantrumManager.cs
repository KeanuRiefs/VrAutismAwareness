using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TantrumManager : MonoBehaviour
{
    public Animator childAnimator;
    public float timeRequired = 10f;
    
    [Header("Drag Left/Right Controller Objects Here")]
    public GameObject leftControllerObject;
    public GameObject rightControllerObject;

    private XRBaseController leftController;
    private XRBaseController rightController;
    private float currentTimer = 0f;
    private int handsInZone = 0;
    private bool isCalm = false;

    void Start()
    {
        // Automatically find the controller component on the objects you dragged in
        if (leftControllerObject != null) leftController = leftControllerObject.GetComponent<XRBaseController>();
        if (rightControllerObject != null) rightController = rightControllerObject.GetComponent<XRBaseController>();
    }

    void Update()
{
    if (isCalm || leftController == null || rightController == null) return;

    bool isLeftGrabbing = leftController.currentControllerState.selectInteractionState.active;
    bool isRightGrabbing = rightController.currentControllerState.selectInteractionState.active;

    // This will tell us in the Console if the hands are actually inside the head
    if (handsInZone >= 2)
    {
        Debug.Log("Hands are in position! Waiting for Grab buttons...");
        
        if (isLeftGrabbing && isRightGrabbing)
        {
            currentTimer += Time.deltaTime;
            Debug.Log($"Calming... {currentTimer:F1}");

            if (currentTimer >= timeRequired)
            {
                CalmChild();
            }
        }
    }
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand")) handsInZone++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand")) handsInZone--;
    }

    void CalmChild()
    {
        isCalm = true;
        if (childAnimator != null) childAnimator.SetTrigger("isCalmed");
        Debug.Log("Objective Complete: Child is calm.");
    }
}