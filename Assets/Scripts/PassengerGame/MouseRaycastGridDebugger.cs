
using UnityEngine;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;

public class MouseRaycastGridDebugger : MonoBehaviour
{
    private Camera _mainCamera;
    private bool isDragging = false;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            PerformRaycast();
        }
    }

    void PerformRaycast()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            Vector3 hitPoint = hitInfo.point;

            Debug.Log($"Hit at position: {hitPoint}");

            CheckGridSlotsUnderPoint(hitPoint);
        }
    }

    void CheckGridSlotsUnderPoint(Vector3 hitPoint)
    {
        foreach (var slot in GridManager.Instance.LowerGrid.GetAllSlots())
        {
            Vector3 slotPos = slot.Value.Position;

            float distance = Vector3.Distance(new Vector3(slotPos.x, hitPoint.y, slotPos.z), hitPoint);

            if (distance < 0.5f)
            {
                Debug.Log($"Slot Key: {slot.Key.ValueAsString} | Occupied: {slot.Value.IsOccupied}");
            }
        }

        foreach (var slot in GridManager.Instance.UpperGrid.GetAllSlots())
        {
            Vector3 slotPos = slot.Value.Position;

            float distance = Vector3.Distance(new Vector3(slotPos.x, hitPoint.y, slotPos.z), hitPoint);

            if (distance < 0.5f)
            {
                Debug.Log($"Slot Key: {slot.Key.ValueAsString} | Occupied: {slot.Value.IsOccupied}");
            }
        }
    }
}
