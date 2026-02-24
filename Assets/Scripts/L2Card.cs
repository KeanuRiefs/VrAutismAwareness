using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

public class L2Card : MonoBehaviour
{
    [SerializeField] private bool isCorrectCard;
    [SerializeField] private XRGrabInteractable grabInteractable;

    private IXRSelectInteractor currentInteractor;
    private L2CommunicationManager manager;

    public bool IsCorrectCard => isCorrectCard;

    private void Reset()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void Initialize(L2CommunicationManager flowManager)
    {
        manager = flowManager;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject == currentInteractor)
        {
            currentInteractor = null;
        }
    }

    public bool IsCurrentlyHeldBy(XRBaseInteractor interactor)
    {
        return interactor != null && currentInteractor == interactor;
    }

    public void PresentToChild()
    {
        manager?.TryPresentCard(this);
    }
}
