using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Grid;

public static class BusMover
{
    public static void TryMove(
    BusController bus,
    Direction direction,
    GridManager gridManager,
    BusMoverSettingsSO settings
)
    {
        var currentKeys = new List<GameKey>(bus.OccupiedGridKeys);

        if (currentKeys.Count != 3)
            return;

        GameKey newHead = currentKeys[0].GetAdjacent(direction);
        if (!gridManager.UpperGrid.IsSlotEmpty(newHead))
            return;

        var newKeys = new List<GameKey> { newHead, currentKeys[0], currentKeys[1] };

        bus.SetDirection(direction);

        AnimateSegments(bus, currentKeys, newKeys, gridManager, () =>
        {
            foreach (var key in currentKeys)
                gridManager.UpperGrid.ClearSlot(key);

            for (int i = 0; i < newKeys.Count; i++)
                gridManager.UpperGrid.PlaceMultiSlotItem(newKeys[i], bus, force: true);

            bus.UpdateSlotKeys(newKeys);
            bus.SetMoving(false);
        }, settings);

        bus.SetMoving(true);
    }

    private static void AnimateSegments(
     BusController bus,
     List<GameKey> prevKeys,
     List<GameKey> newKeys,
     GridManager gridManager,
     System.Action onComplete,
     BusMoverSettingsSO settings
 )
    {
        int completed = 0;

        for (int i = 0; i < bus.Segments.Count; i++)
        {
            GameObject segment = bus.Segments[i];
            GameKey targetKey = (i == 0) ? newKeys[0] : prevKeys[i - 1];
            Vector3 worldTarget = gridManager.GetSlotPosition(targetKey, true);

            Quaternion targetRotation;
            if (i == 0)
            {
                targetRotation = bus.GetRotationFromDirection(bus.CurrentDirection);
            }
            else
            {
                Vector3 dir = bus.Segments[i - 1].transform.position - segment.transform.position;
                if (dir != Vector3.zero)
                    targetRotation = Quaternion.LookRotation(dir);
                else
                    targetRotation = segment.transform.rotation;
            }

            segment.transform.DORotateQuaternion(targetRotation, settings.RotationSpeed)
                .SetEase(settings.RotationEase);

            segment.transform.DOMove(worldTarget, settings.MoveDuration)
                .SetEase(settings.MoveEase)
                .OnComplete(() =>
                {
                    completed++;
                    if (completed == bus.Segments.Count)
                    {
                        onComplete?.Invoke();
                        bus.OnMoveCompleteAfterSwipe();
                    }
                });
        }
    }
}
