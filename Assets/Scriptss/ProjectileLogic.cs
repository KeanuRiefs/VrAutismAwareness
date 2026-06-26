using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // CASE A: BLOCKED! (Hit the hands)
        if (collision.gameObject.CompareTag("PlayerShield"))
        {
            Debug.Log("BLOCKED! Great job.");
            // Play "Thud" sound here
            Destroy(this.gameObject); // Remove the toy
        }
        
        // CASE B: FAILED! (Hit the Head/Camera)
        else if (collision.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("OUCH! You got hit.");
            // Flash screen Red here
            Destroy(this.gameObject);
        }

        // CASE C: MISSED (Hit the floor/walls)
        else 
        {
            // Destroy after 2 seconds so the floor doesn't get cluttered
            Destroy(this.gameObject, 2.0f);
        }
    }
}