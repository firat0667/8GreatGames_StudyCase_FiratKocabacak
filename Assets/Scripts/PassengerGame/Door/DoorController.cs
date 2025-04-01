using DG.Tweening;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Utility;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IMatchable, ISlotItem
{
    public GameKey SlotIndex { get; set; }
    public ItemColor ItemColor { get; set; } = ItemColor.Red;
    public bool IsMovable => false;
    public bool IsMarkedForMatch { get; set; }
    public List<GameKey> OccupiedGridKeys { get; private set; } = new();
    public GameObject Root => gameObject;

    private readonly List<PassengerController> _passengers = new();
    private PassengerMoveSettingsSO moveSettings;

    public void Initialize(GameKey key)
    {
        SlotIndex = key;
        OccupiedGridKeys.Clear();
        OccupiedGridKeys.Add(key);
        moveSettings = DoorManager.Instance.MoveSettigs;
    }

    public void AddPassenger(PassengerController passenger)
    {
        _passengers.Add(passenger);
    }

    public void RemovePassenger(PassengerController passenger)
    {
        _passengers.Remove(passenger);
    }

    public List<PassengerController> GetPassengers() => _passengers;

    public void UpdateColorByFirstPassenger()
    {
        if (_passengers.Count == 0) return;

        ItemColor = _passengers[0].ItemColor;

        if (gameObject.TryGetComponentInChildren<Renderer>(out var renderer))
            renderer.material.color = ItemColorUtility.GetColor(ItemColor);
    }

    public void TryMatchAndAssignPassengerToBus(BusController bus)
    {
        if (_passengers.Count == 0 || !bus.HasSwiped) return;

        PassengerController passenger = _passengers[0];

        if (!TryFindSeat(bus, passenger, out int seg, out int seat))
            return;

        Transform seatTransform = bus.SegmentSeats[seg][seat];

        passenger.JumpToSeat(seatTransform, moveSettings, () =>
        {
            bus.SeatOccupancy[seg][seat] = true;
            VFXManager.Instance.PlayMergeParticle(seatTransform.position);

            ShakeSegment(bus.Segments[seg].transform, () =>
            {
                bus.AreAllSeatsFull();
            });

            RemovePassenger(passenger);
            UpdateColorByFirstPassenger();
            TryMatchAndAssignPassengerToBus(bus);
        });

        AnimateOtherPassengers();
    }

    private bool TryFindSeat(BusController bus, PassengerController passenger, out int seg, out int seat)
    {
        return bus.TryGetAvailableSeat(passenger.ItemColor, out seg, out seat);
    }

    private void AnimateOtherPassengers()
    {
        for (int i = 1; i < _passengers.Count; i++)
        {
            var target = _passengers[i - 1].transform.position;
            _passengers[i].transform.DOMove(target, moveSettings.moveDelayBetweenPassengers).SetEase(Ease.OutQuad);
            _passengers[i].transform.DOLookAt(target, 0.1f);
        }
    }

    private void ShakeSegment(Transform segmentTransform, System.Action onComplete)
    {
        segmentTransform.DOShakeScale(
            duration: 0.3f,
            strength: new Vector3(0.25f, 0.25f, 0.25f),
            vibrato: 10,
            randomness: 90
        ).OnComplete(() =>
        {
            segmentTransform.DOScale(Vector3.one, 0.3f);
            onComplete?.Invoke();
        });
    }

    public void MoveTo(GameKey key) { }
    public bool MatchesWith(IMatchable other) => false;
    public void OnMatchedAsTarget() { }
    public void OnMatchedAsMover(GameKey targetSlot) { }
}
