using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ButtonScript : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab to instantiate when the button is clicked.")]
    [SerializeField] private GameObject prefabToSpawn;

    [Tooltip("Where to spawn the prefab. If null, uses this object’s position.")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Optional parent for the spawned object (use a Canvas/RectTransform for UI prefabs).")]
    [SerializeField] private Transform spawnParent;

    [Tooltip("If true and a spawnPoint is set, uses its rotation. Otherwise, identity.")]
    [SerializeField] private bool useSpawnPointRotation = true;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(Spawn);
        }
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(Spawn);
        }
    }

    // You can also call this from the Button's OnClick in the Inspector
    // if you prefer not to auto-wire via GetComponent<Button>().
    public void Spawn()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"{nameof(ButtonScript)} on {name}: No prefab assigned to spawn.");
            return;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotation = (useSpawnPointRotation && spawnPoint != null) ? spawnPoint.rotation : Quaternion.identity;

        if (spawnParent != null)
        {
            Instantiate(prefabToSpawn, position, rotation, spawnParent);
        }
        else
        {
            Instantiate(prefabToSpawn, position, rotation);
        }
    }
}

