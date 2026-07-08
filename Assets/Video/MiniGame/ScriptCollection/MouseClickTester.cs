using UnityEngine;
using UnityEngine.Events;

public class MouseClickTester : MonoBehaviour
{
    // Ini akan buat ruang kosong di Inspector macam butang UI biasa
    public UnityEvent onMouseClick;

    // Fungsi automatik Unity bila objek ber-collider diklik guna mouse kiri
    void OnMouseDown()
    {
        Debug.Log("Mouse klik dikesan pada " + gameObject.name);
        onMouseClick.Invoke();
    }
}