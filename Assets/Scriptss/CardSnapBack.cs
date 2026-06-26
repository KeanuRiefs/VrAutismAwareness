using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class CardSnapBack : MonoBehaviour
{
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Coroutine returnCoroutine;

    [Header("Settings")]
    public float returnSpeed = 5f;

    void Start()
    {
        // Remember the exact spot and rotation inside the container
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Listen for the moment the player lets go of the card
        grabInteractable.selectExited.AddListener(OnRelease);
        
        // Optional: Stop returning if the player grabs it again mid-flight
        grabInteractable.selectEntered.AddListener(OnGrab); 
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Fixed the spelling of Hierarchy here!
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (returnCoroutine != null) 
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnToPosition());
    }

    private IEnumerator ReturnToPosition()
    {
        // Smoothly glide the card back to its original slot
        while (Vector3.Distance(transform.localPosition, originalLocalPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, Time.deltaTime * returnSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalLocalRotation, Time.deltaTime * returnSpeed);
            yield return null;
        }

        // Snap perfectly at the very end
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnRelease);
            grabInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }
}