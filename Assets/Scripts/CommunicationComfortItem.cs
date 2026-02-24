using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CommunicationComfortItem : MonoBehaviour
{
    [SerializeField] private CommunicationLevelManager levelManager;
    [SerializeField] private XRGrabInteractable grabInteractable;

    private void Reset()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnPickedUp);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnPickedUp);
        }
    }

    private void OnPickedUp(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        levelManager?.OnComfortObjectPickedUp();
    }

    public void GiveToChild()
    {
        levelManager?.OnComfortObjectGivenToChild();
    }
}
