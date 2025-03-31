using GreatGames.CaseLib.Definitions;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using UnityEngine;

public static class DoorBuilder
{
    public static GameObject Build(DoorData data, GridManager gridManager, GameObject prefab, Transform parent)
    {
        GameKey key = gridManager.UpperGrid.CreateKeyFromIndex(data.SlotIndex);
        Vector3 centerPos = gridManager.GetSlotPosition(key, true);
        Vector3 offset = GetDirectionOffset(data.EnterDirection) * 0.5f;
        Quaternion rotation = GetRotationFromDirection(data.EnterDirection);
        Vector3 finalPos = centerPos + offset;

        var doorGO = Object.Instantiate(prefab, finalPos, rotation, parent);
        ComponentUtils.SetNameBySlotType(doorGO, SlotType.Door, key);
        SetDoorColors(doorGO, data.IncomingColors);
        var controller = doorGO.AddComponent<DoorController>();
        DoorManager.Instance.RegisterDoor(controller);
        controller.Initialize(key);
        return doorGO;
    }

    private static Vector3 GetDirectionOffset(Direction dir) => dir switch
    {
        Direction.Up => Vector3.forward,
        Direction.Down => Vector3.back,
        Direction.Left => Vector3.left,
        Direction.Right => Vector3.right,
        _ => Vector3.forward
    };

    private static Quaternion GetRotationFromDirection(Direction dir) => dir switch
    {
        Direction.Up => Quaternion.Euler(0, 0, 0),
        Direction.Down => Quaternion.Euler(0, 180, 0),
        Direction.Left => Quaternion.Euler(0, -90, 0),
        Direction.Right => Quaternion.Euler(0, 90, 0),
        _ => Quaternion.identity
    };

    private static void SetDoorColors(GameObject go, List<ItemColor> colors)
    {
        if (colors.Count == 0) return;
        if (go.TryGetComponent<Renderer>(out var r) || go.TryGetComponentInChildren<Renderer>(out r))
            r.material.color = ItemColorUtility.GetColor(colors[0]);

        for (int i = 0; i < colors.Count && i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            if (child.TryGetComponent<Renderer>(out var cr))
                cr.material.color = ItemColorUtility.GetColor(colors[i]);
        }
    }
}
