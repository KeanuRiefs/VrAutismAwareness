using UnityEngine;

public class Billboard : MonoBehaviour {
    void LateUpdate() {
        // Makes the text face the camera perfectly
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}