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
    public void UpdateColorByFirstPassenger()
    {
        if (_passengers.Count == 0) return;

        var firstColor = _passengers[0].ItemColor;
        ItemColor = firstColor;

        if (gameObject.TryGetComponentInChildren<Renderer>(out var renderer))
        {
            renderer.material.color = ItemColorUtility.GetColor(firstColor);
        }
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

    public void MoveTo(GameKey key){ }
  
    public bool MatchesWith(IMatchable other)
    {
        return false; 
    }
    public void TryMatchAndAssignPassengerToBus(BusController bus)
    {
        if (_passengers.Count == 0 || !bus.HasSwiped) return;

        var passenger = _passengers[0];
        if (!bus.TryGetAvailableSeat(passenger.ItemColor, out int segmentIndex, out int seatIndex))
            return;

        List<Vector3> originalPositions = new();
        foreach (var p in _passengers)
            originalPositions.Add(p.transform.position);

        Transform seatTransform = bus.SegmentSeats[segmentIndex][seatIndex];
        Vector3 seatPos = seatTransform.position;

        Sequence seq = DOTween.Sequence();

     

        float scaleUp = moveSettings.scaleUp;
        float scaleDuration = moveSettings.scaleDuration;
        float jumpPower = moveSettings.jumpPower;
        float jumpDuration = moveSettings.jumpDuration;
        int jumpCount = moveSettings.jumpCount;
        Ease jumpEase = moveSettings.jumpEase;
        Ease scaleEase = moveSettings.scaleEase;
        float moveDelay = moveSettings.moveDelayBetweenPassengers;

        seq.Append(passenger.transform.DOScale(scaleUp, scaleDuration).SetEase(scaleEase));
        seq.Append(passenger.transform.DOJump(seatPos, jumpPower, jumpCount, jumpDuration).SetEase(jumpEase));
        seq.Append(passenger.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.InOutSine));

        for (int i = 1; i < _passengers.Count; i++)
        {
            _passengers[i].transform.DOMove(originalPositions[i - 1], moveDelay).SetEase(Ease.OutQuad);
            _passengers[i].transform.DOLookAt(originalPositions[i - 1], 0.1f);
        }
        seq.OnComplete(() =>
        {
            passenger.transform.SetParent(seatTransform, true);
            passenger.transform.localPosition = Vector3.zero;
            passenger.transform.localRotation = Quaternion.identity;
            passenger.transform.localScale = Vector3.one;
            passenger.GetComponentInChildren<PassengerAnim>().PlayPassengerSit();
            VFXManager.Instance.PlayMergeParticle(seatTransform.position);
            bus.SeatOccupancy[segmentIndex][seatIndex] = true;

            var segmentTransform = bus.Segments[segmentIndex].transform;
            segmentTransform.DOShakeScale(
                duration: 0.3f,
                strength: new Vector3(0.25f, 0.25f, 0.25f),
                vibrato: 10,
                randomness: 90
            ).OnComplete(() =>
            {
                segmentTransform.DOScale(Vector3.one, 0.3f);
                bus.AreAllSeatsFull();
            });

            RemovePassenger(passenger);
            UpdateColorByFirstPassenger();
            TryMatchAndAssignPassengerToBus(bus);
        });
    }





    public void OnMatchedAsTarget() { }
    public void OnMatchedAsMover(GameKey targetSlot) { }
}
