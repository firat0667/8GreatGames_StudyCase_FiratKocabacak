using System.Collections.Generic;
using UnityEngine;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Utility;

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
    ItemColor ISlotItem.ItemColor { get ; set; }
    public bool IsMoving { get; private set; }
    public BusMoverSettingsSO BusMoveSettings { get; set; }
    public void Initialize(List<GameKey> keys, List<ItemColor> colors, List<Direction> directions, GridManager gridManager,
                           GameObject headPrefab, GameObject midPrefab, GameObject tailPrefab)
    {
        _slotKeys.Clear();
        _slotKeys.AddRange(keys);
        SegmentColors = colors;

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
