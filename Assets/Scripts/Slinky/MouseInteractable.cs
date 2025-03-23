using GreatGames.CaseLib.Slinky;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInteractable : MonoBehaviour
{
    private Rigidbody rb;
    private float forceMultiplier = 55f;
    private SlinkyController _parentSlinky;
    private Camera _mainCamera;

    private void Awake()
    {
        _parentSlinky = GetComponentInParent<SlinkyController>();
        _mainCamera = Camera.main;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            ProcessRay(ray);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUI())
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            ProcessRay(ray);
        }
    }
    private void ProcessRay(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                rb.AddForce(Vector3.up * forceMultiplier, ForceMode.Impulse);
                HandleClick();
            }
        }
    }
    private void HandleClick()
    {
        if (_parentSlinky != null)
        {
            _parentSlinky.OnSegmentClicked();
        }
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
