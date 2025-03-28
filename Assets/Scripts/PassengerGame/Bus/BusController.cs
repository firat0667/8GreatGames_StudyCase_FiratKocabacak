using System.Collections.Generic;
using UnityEngine;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Utility;

public class BusController : MonoBehaviour
{
    public List<GameObject> Segments { get; private set; } = new();
    public List<GameKey> SlotKeys { get; private set; } = new();
    public List<ItemColor> SegmentColors { get; private set; } = new();

    public void Initialize(List<GameKey> keys, List<ItemColor> colors, List<Direction> directions, GridManager gridManager,
                           GameObject headPrefab, GameObject midPrefab, GameObject tailPrefab)
    {
        SlotKeys = keys;
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

            Segments.Add(segment);
        }
    }

    private Quaternion GetRotationFromDirection(Direction dir)
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
}
