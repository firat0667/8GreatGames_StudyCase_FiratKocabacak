using DG.Tweening;
using GreatGames.CaseLib.Game;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BusController : MonoBehaviour, ISlotItem, IMatchable
{
    public List<GameObject> Segments { get; private set; } = new();
    private readonly List<GameKey> _slotKeys = new();
    public List<GameKey> SlotKeys => _slotKeys;
    public List<ItemColor> SegmentColors { get; private set; } = new();
    public Direction CurrentDirection { get; private set; }
    public GameKey SlotIndex => SlotKeys != null && SlotKeys.Count > 0 ? SlotKeys[0] : null;
    public List<GameKey> OccupiedGridKeys => SlotKeys;
    public GameObject Root => gameObject;
    public ItemColor ItemColor => SegmentColors != null && SegmentColors.Count > 0 ? SegmentColors[0] : ItemColor.Red;
    public bool IsMovable => true;
    public bool IsMarkedForMatch { get; set; }
    GameKey ISlotItem.SlotIndex { get ; set ; }
    public bool IsMoving { get;  set; }
    public BusMoverSettingsSO BusMoveSettings { get; set; }

    public bool HasSwiped { get; set; } = false;

    public List<BusSeatInfo> Seats { get; private set; } = new();

    public List<List<Transform>> SegmentSeats { get; private set; } = new();
    public List<List<bool>> SeatOccupancy { get; private set; } = new();

    public bool IsAllOccupied;


    public void Initialize(List<GameKey> keys, List<ItemColor> colors, List<Direction> directions, GridManager gridManager,
                         GameObject headPrefab, GameObject midPrefab, GameObject tailPrefab)
    {
        _slotKeys.Clear();
        _slotKeys.AddRange(keys);
        SegmentColors = colors;
        Segments.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            GameObject segmentPrefab = i == 0 ? headPrefab :
                                        i == 1 ? midPrefab : tailPrefab;

            Vector3 pos = gridManager.GetSlotPosition(keys[i], true);
            Quaternion rot = GetRotationFromDirection(i < directions.Count ? directions[i] : Direction.Up);

            GameObject segment = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity);
            segment.transform.SetParent(transform, false);
            segment.transform.localPosition = pos - gridManager.GetSlotPosition(keys[0], true);
            segment.transform.localRotation = rot;

            if (i < colors.Count && segment.TryGetComponentInChildren<Renderer>(out var renderer))
            {
                renderer.material.color = ItemColorUtility.GetColor(colors[i]);
            }

            if (segment.TryGetComponent<BusSegmentClickHandler>(out var handler))
            {
                handler.Initialize(this, i);
            }
            else
            {
                var clickHandler = segment.AddComponent<BusSegmentClickHandler>();
                clickHandler.Initialize(this, i);
            }

            Segments.Add(segment);
        }

        if (directions != null && directions.Count > 0)
        {
            CurrentDirection = directions[0];
        }
        Seats.Clear();
        for (int i = 0; i < SlotKeys.Count; i++)
        {
            Seats.Add(new BusSeatInfo
            {
                SlotKey = SlotKeys[i],
                Color = SegmentColors[i],
                Capacity = (i == 1) ? 4 : 2,
                Occupied = 0
            });
        }
        SegmentSeats.Clear();
        SeatOccupancy.Clear();

        foreach (var segment in Segments)
        {
            var seatsInSegment = segment.GetComponentsInChildren<Transform>();

            List<Transform> seatList = new();
            List<bool> occupancyList = new();

            foreach (var seat in seatsInSegment)
            {
                if (seat.name.StartsWith("Seat_"))
                {
                    seatList.Add(seat);
                    occupancyList.Add(false);
                }
            }

            SegmentSeats.Add(seatList);
            SeatOccupancy.Add(occupancyList);
        }
    }

    public void UpdateSlotKeys(List<GameKey> newKeys)
    {
        _slotKeys.Clear();
        _slotKeys.AddRange(newKeys);
    }
    public void SetDirection(Direction newDir)
    {
        CurrentDirection = newDir;
    }
    public bool CanPassengerMatchBus(PassengerController passenger, List<ItemColor> busColors)
    {
        return busColors.Contains(passenger.ItemColor);
    }
    public bool TryGetAvailableSeat(ItemColor color, out int segmentIndex, out int seatIndex)
    {
        for (int i = 0; i < SlotKeys.Count; i++)
        {
            if (SegmentColors[i] != color) continue;

            for (int j = 0; j < SeatOccupancy[i].Count; j++)
            {
                if (!SeatOccupancy[i][j])
                {
                    segmentIndex = i;
                    seatIndex = j;
                    return true;
                }
            }
        }

        segmentIndex = -1;
        seatIndex = -1;
        return false;
    }
    public Quaternion GetRotationFromDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Quaternion.Euler(0, 0, 0),
            Direction.Down => Quaternion.Euler(0, 180, 0),
            Direction.Left => Quaternion.Euler(0, -90, 0),
            Direction.Right => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity
        };
    }
    public void OnMoveCompleteAfterSwipe()
    {
      if (HasSwiped) { 

         foreach (var key in OccupiedGridKeys)
         {
            if (!key.IsUpper()) continue;

            var door = DoorManager.Instance.GetDoorAt(key);
            if (door != null)
            {
              door.TryMatchAndAssignPassengerToBus(this);
            }
         }
       }
    }
    public void ApplySegmentHighlight()
    {
        for (int i = 0; i < Segments.Count; i++)
        {
            var tf = Segments[i].transform;
            tf.DOScale(1.12f, 0.15f).SetEase(Ease.OutBack);

            if (Segments[i].TryGetComponentInChildren<Renderer>(out var renderer))
            {
                Color originalColor = ItemColorUtility.GetColor(SegmentColors[i]);
                Color brightened = Color.Lerp(originalColor, Color.white, 0.4f);
                renderer.material.color = brightened;
            }
        }
    }

    public void ResetSegmentHighlight()
    {
        for (int i = 0; i < Segments.Count; i++)
        {
            var tf = Segments[i].transform;
            tf.DOScale(Vector3.one, 0.15f).SetEase(Ease.InOutSine);

            if (Segments[i].TryGetComponentInChildren<Renderer>(out var renderer))
            {
                Color originalColor = ItemColorUtility.GetColor(SegmentColors[i]);
                renderer.material.DOColor(originalColor, 0.15f);
            }
        }
    }

    public bool AreAllSeatsFull()
    {
        foreach (var occupancyList in SeatOccupancy)
        {
            foreach (var occupied in occupancyList)
            {
                if (!occupied) return false;
            }
        }
        StartFullBusSequence();
        return true;
    }
    public void StartFullBusSequence()
    {
        IsAllOccupied = true;
        Transform tailSegment = Segments[Segments.Count - 1].transform;
        VFXManager.Instance.HoleMergeSpawner(new Vector3(tailSegment.position.x, tailSegment.position.y + 1f, tailSegment.position.z));
        StartCoroutine(DestroyBusRoutine(tailSegment.position));
    }
    private IEnumerator DestroyBusRoutine(Vector3 targetPosition)
    {
        var settings = BusMoveSettings;

        for (int i = Segments.Count - 1; i >= 0; i--)
        {
            var segment = Segments[i];
            Sequence seq = DOTween.Sequence();
            seq.Join(segment.transform.DOJump(
                targetPosition,
                settings.DestroyJumpPower,
                1,
                settings.DestroyJumpDuration
            ).SetEase(settings.DestroyEase));

            seq.Join(segment.transform.DOScale(
                settings.DestroyScale,
                settings.DestroyJumpDuration
            ).SetEase(settings.DestroyScaleEase));

            yield return new WaitForSeconds(settings.DelayBetweenSegmentDestruction);

            segment.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        SlotKeys.ForEach(key => GridManager.Instance.RemoveItemAt(key));
        GridManager.Instance.RemoveItem(this);
        gameObject.SetActive(false);
        GameManager.Instance.CheckForCompletion();
    }

    public void OnSegmentClicked(Direction direction)
    {
        BusMover.TryMove(this, direction, GridManager.Instance, BusMoveSettings);
    }
    public void SetMoving(bool moving)
    {
        IsMoving = moving;
    }

    public void MoveTo(GameKey targetKey)
    {
    }

    public bool MatchesWith(IMatchable other)
    {
        return other != null && ItemColor == other.ItemColor;
    }

    public void OnMatchedAsTarget()
    {
    }

    public void OnMatchedAsMover(GameKey targetSlot)
    {
    }
}
