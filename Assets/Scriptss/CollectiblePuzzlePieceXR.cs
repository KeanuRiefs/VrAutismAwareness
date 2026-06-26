using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class CollectiblePuzzlePieceXR : MonoBehaviour
{
    [Header("Particle prefabs")]
    public ParticleSystem idleEffectPrefab;
    public ParticleSystem trailEffectPrefab;
    public ParticleSystem collectEffectPrefab;

    [Header("Behavior")]
    public bool rotatePiece = true;
    public float idleRotationSpeed = 45f; // Set slightly faster for a nice collectible spin

    [Header("Collection Rules")]
    public bool collectOnGrab = true;
    public bool collectWhenPlacedInZone = true;
    public string collectionZoneTag = "CollectionZone";

    [Header("Events")]
    public UnityEvent onCollected;

    XRGrabInteractable grabInteractable;
    XRBaseInteractor currentInteractor;
    ParticleSystem idleInstance;
    ParticleSystem trailInstance;
    bool collected;

    // Cache variable to hold the 3D model child reference
    private Transform visualMeshChild;
    private Vector3 initialChildPosition;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void Start()
    {
        // Automatically look for the 3D visual mesh child inside this object
        MeshRenderer childRenderer = GetComponentInChildren<MeshRenderer>();
        if (childRenderer != null)
        {
            visualMeshChild = childRenderer.transform;
            initialChildPosition = visualMeshChild.localPosition;
        }

        if (idleEffectPrefab)
        {
            idleInstance = Instantiate(idleEffectPrefab, transform);
            idleInstance.transform.localPosition = Vector3.zero;
            idleInstance.transform.localRotation = Quaternion.identity;
            idleInstance.Play();
        }

        if (trailEffectPrefab)
        {
            trailInstance = Instantiate(trailEffectPrefab, transform);
            trailInstance.transform.localPosition = Vector3.zero;
            trailInstance.transform.localRotation = Quaternion.identity;
            trailInstance.Play();
        }
    }

    void Update()
    {
        if (!collected)
        {
            // 1. Smooth Script-Based Axis Rotation
            if (rotatePiece)
            {
                if (visualMeshChild != null)
                {
                    // Spins ONLY the 3D model around its own local center
                    visualMeshChild.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.Self);
                }
                else
                {
                    transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.Self);
                }
            }

            // 2. Gentle Floating Bobbing Effect
            if (visualMeshChild != null)
            {
                // Smoothly calculate a hover wave up and down over time
                float hoverY = Mathf.Sin(Time.time * 2.5f) * 0.04f;
                visualMeshChild.localPosition = initialChildPosition + new Vector3(0, hoverY, 0);
            }
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject as XRBaseInteractor;

        if (collectOnGrab && !collected)
            Collect();
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInteractor;
        if (interactor == currentInteractor)
            currentInteractor = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (collectWhenPlacedInZone && other.CompareTag(collectionZoneTag))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        if (currentInteractor != null && grabInteractable != null && grabInteractable.interactionManager != null)
        {
            var manager = grabInteractable.interactionManager;
            try
            {
                var ixrInteractor = (IXRSelectInteractor)currentInteractor;
                var ixrInteractable = (IXRSelectInteractable)grabInteractable;
                manager.SelectExit(ixrInteractor, ixrInteractable);
            }
            catch (System.InvalidCastException)
            {
                if (currentInteractor.interactionManager != null)
                {
                    currentInteractor.interactionManager.SelectExit(currentInteractor as IXRSelectInteractor, grabInteractable as IXRSelectInteractable);
                }
            }

            currentInteractor = null;
        }

        if (idleInstance)
        {
            idleInstance.transform.SetParent(null);
            idleInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(idleInstance.gameObject, 3f);
            idleInstance = null;
        }

        if (trailInstance)
        {
            trailInstance.transform.SetParent(null);
            trailInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(trailInstance.gameObject, 3f);
            trailInstance = null;
        }

        ParticleSystem collectInstance = null;
        if (collectEffectPrefab)
        {
            collectInstance = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            collectInstance.Play();
        }

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        onCollected?.Invoke();

        if (collectInstance != null)
            StartCoroutine(DestroyAfterEffect(collectInstance));

        Destroy(gameObject, 0.2f);
    }

    IEnumerator DestroyAfterEffect(ParticleSystem ps)
    {
        if (ps == null) yield break;
        var main = ps.main;
        float wait = main.duration;
#if UNITY_2019_3_OR_NEWER
        wait += main.startLifetime.constantMax;
#else
        wait += main.startLifetime.constant;
#endif
        if (wait <= 0f) wait = 2f;
        yield return new WaitForSeconds(wait + 0.1f);
        if (ps) Destroy(ps.gameObject);
    }
}