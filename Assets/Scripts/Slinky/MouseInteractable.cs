using GreatGames.CaseLib.Slinky;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInteractable : MonoBehaviour
{
    private Rigidbody rb;
    private float forceMultiplier = 55f;
    private SlinkyController _parentSlinky;
    private void Awake()
    {
        _parentSlinky = GetComponentInParent<SlinkyController>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        rb.AddForce(Vector3.up * forceMultiplier, ForceMode.Impulse);
        HandleClick();

    }

    private void HandleClick()
    {
        if (_parentSlinky != null)
        {
            _parentSlinky.OnSegmentClicked();
        }
    }
}
