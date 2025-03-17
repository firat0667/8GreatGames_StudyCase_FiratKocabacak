using GreatGames.CaseLib.Slinky;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlinkySpringClicker : MonoBehaviour, IPointerClickHandler
{
    private SlinkyController _parentSlinky;

    private void Awake()
    {
        _parentSlinky = GetComponentInParent<SlinkyController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
