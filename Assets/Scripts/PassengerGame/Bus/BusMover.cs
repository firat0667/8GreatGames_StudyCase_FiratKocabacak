using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Grid;

public static class BusMover
{
    public static void TryMove(BusController bus, Direction direction, GridManager gridManager)
    {
        var currentKeys = new List<GameKey>(bus.OccupiedGridKeys);

        if (currentKeys.Count != 3)
        {
            Debug.LogWarning("Bus segment sayısı 3 değil!");
            return;
        }


        GameKey newHead = currentKeys[0].GetAdjacent(direction);


        if (!gridManager.UpperGrid.IsSlotEmpty(newHead))
        {
            return;
        }

        var newKeys = new List<GameKey> { newHead, currentKeys[0], currentKeys[1] };

        foreach (var key in currentKeys)
            gridManager.UpperGrid.ClearSlot(key);
        for (int i = 0; i < newKeys.Count; i++)
            gridManager.UpperGrid.PlaceMultiSlotItem(newKeys[i], bus, force: true);
        bus.UpdateSlotKeys(newKeys);
        bus.SetDirection(direction);

        AnimateSegments(bus, newKeys, gridManager);
    }

    private static void AnimateSegments(BusController bus, List<GameKey> keys, GridManager gridManager)
    {
        Vector3 basePos = gridManager.GetSlotPosition(keys[0], true);

        for (int i = 0; i < bus.Segments.Count; i++)
        {
            Vector3 localTarget = gridManager.GetSlotPosition(keys[i], true) - basePos;
            float delay = i * 0.1f; 

            bus.Segments[i].transform.DOLocalMove(localTarget, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);

            Debug.Log($"[BusMover] Segment {i} -> {keys[i].ValueAsString}");
        }
    }
}
