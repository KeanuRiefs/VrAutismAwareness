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
    private Rigidbody rb;

    public bool IsCorrectCard => isCorrectCard;

    private void Reset()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
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

        LockCardToContainer();
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

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject == currentInteractor)
        {
            currentInteractor = null;
        }
    }

    private void LockCardToContainer()
    {
        if (rb == null) return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;
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
