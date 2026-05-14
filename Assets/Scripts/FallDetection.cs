using UnityEngine;

public class Falldetection : MonoBehaviour
{
    // It's slightly more efficient to ask for a Transform instead of a GameObject 
    // if you only need their positions!
    public Transform glassSpawnPos;
    public Transform headphonePos;

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "glasses 1")
        {
            ResetPosition(other.gameObject, glassSpawnPos);
        }
        else if (other.gameObject.name == "headphone")
        {
            ResetPosition(other.gameObject, headphonePos);
        }
    }

    // Creating a helper method keeps your code clean and prevents repeating yourself
    private void ResetPosition(GameObject fallingObject, Transform spawnPoint)
    {
        // 1. Teleport the object to the spawn point
        fallingObject.transform.position = spawnPoint.position;

        // 2. Kill the momentum! 
        // If we don't do this, the object will remember it was falling fast and shoot straight through the floor.
        Rigidbody rb = fallingObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;        // Stops movement
            rb.angularVelocity = Vector3.zero; // Stops spinning
        }
    }
}