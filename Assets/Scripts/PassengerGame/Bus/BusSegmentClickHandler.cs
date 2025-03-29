using UnityEngine;

public class BusSegmentClickHandler : MonoBehaviour
{
    private BusController _busController;
    private int _segmentIndex;
    private Vector2 _start;
    private bool _hasSwiped = false;
    public void Initialize(BusController controller, int index)
    {
        _busController = controller;
        _segmentIndex = index;
        Debug.Log($"[ClickHandler] Segment {index} bağlandı -> Bus: {controller.name}");
    }

    private void OnMouseDown()
    {
        _start = Input.mousePosition;
        _hasSwiped = false;
    }


    private void OnMouseDrag()
    {
        if (_hasSwiped) return;

        Vector2 delta = (Vector2)Input.mousePosition - _start;
        if (delta.magnitude < 20f) return; // minimum swipe eşiği

        Direction dir = GetDirectionFromDelta(delta);
        Debug.Log($"[ClickHandler] Swipe detected! Segment: {_segmentIndex}, Dir: {dir}");

        _busController.OnSegmentClicked(dir);
        _hasSwiped = true;
    }

    private Direction GetDirectionFromDelta(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? Direction.Right : Direction.Left;
        else
            return delta.y > 0 ? Direction.Up : Direction.Down;
    }
}
