using UnityEngine;

public class MouseInteractable : MonoBehaviour
{
    private Rigidbody rb;
    private float forceMultiplier = 5f; // 🔥 Tıklanınca uygulanacak kuvvet

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        rb.AddForce(Vector3.up * forceMultiplier, ForceMode.Impulse);
    }
}
