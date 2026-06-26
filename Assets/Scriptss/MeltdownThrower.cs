using UnityEngine;

public class MeltdownThrower : MonoBehaviour
{
    public GameObject toyPrefab;      // Drag the Red Sphere Prefab here
    public Transform playerHead;      // Drag your "Main Camera" here
    public float throwInterval = 3.0f;// Time between throws
    public float throwForce = 5.0f;   // How fast it flies

    private float timer;

   void Update()
    {
        transform.LookAt(new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z));

        // NEW: Check Distance
        float distance = Vector3.Distance(transform.position, playerHead.position);

        // If Player is SAFE (far away), slow down throws
        if (distance > 5.5f) 
        {
            throwInterval = 5.0f; // Calming down
        }
        // If Player is TOO CLOSE (threatening), speed up throws
        else 
        {
            throwInterval = 1.5f; // Meltdown intensifying!
        }

        // Timer logic...
        timer += Time.deltaTime;
        if (timer >= throwInterval)
        {
            ThrowObject();
            timer = 0;
        }
    }

    void ThrowObject()
    {
        // Spawn the toy slightly in front of the child (so it doesn't hit the child)
        Vector3 spawnPos = transform.position + transform.forward * 0.5f + Vector3.up * 1.0f;
        GameObject clone = Instantiate(toyPrefab, spawnPos, Quaternion.identity);

        // Calculate direction towards player's head
        Vector3 direction = (playerHead.position - spawnPos).normalized;

        // Add Physics Force
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }
    }
}