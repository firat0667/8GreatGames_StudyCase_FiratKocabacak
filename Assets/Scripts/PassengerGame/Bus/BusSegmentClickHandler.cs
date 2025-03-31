using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using System.Linq;
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
    }

    private void OnMouseDown()
    {
        if (!_busController.IsAllOccupied)
        {
            _busController.HasSwiped = false;
            _start = Input.mousePosition;
            _hasSwiped = true;
            _busController.ApplySegmentHighlight();
        }
    }

    private void OnMouseUp()
    {
        if (!_busController.IsAllOccupied)
        {
            _hasSwiped = false;
            _busController.HasSwiped = true;
            _busController.OnMoveCompleteAfterSwipe();
            _busController.ResetSegmentHighlight();
        }
      

    }
    private void Update()
    {
        if (_hasSwiped && !_busController.IsMoving)
        {
            Vector2 delta = (Vector2)Input.mousePosition - _start;
            if (delta.magnitude >= 20f)
            {
                Direction dir = GetDirectionFromDelta(delta);
                _start = Input.mousePosition; 

                _busController.OnSegmentClicked(dir); 
            }
        }
    }

    private Direction GetDirectionFromDelta(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? Direction.Right : Direction.Left;
        else
            return delta.y > 0 ? Direction.Up : Direction.Down;
    }
}
