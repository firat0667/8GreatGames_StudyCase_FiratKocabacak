using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using UnityEngine;

public static class PassengerSpawner
{
    public static void SpawnPassengers(DoorData door, GridManager gridManager, GameObject passengerPrefab, Transform parent)
    {
        GameKey doorKey = gridManager.UpperGrid.CreateKeyFromIndex(door.SlotIndex);
        Vector3 doorPos = gridManager.GetSlotPosition(doorKey, true);
        Vector3 forward = GetDirectionOffset(door.EnterDirection);
        Vector3 center = doorPos + forward * 1.5f;

        var formation = door.Formation;
        if (formation == null)
        {
            formation = ScriptableObject.CreateInstance<PassengerFormationSO>();
            formation.FormationType = PassengerFormationType.Linear;
        }

        int totalCount = GetTotalPassengerCount(door);
        var points = formation.EvaluatePoints(totalCount);

        int counter = 0;
        foreach (var point in points)
        {
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 worldOffset = point.z * forward + point.x * right;
            Vector3 finalPos = center + worldOffset;
            Debug.DrawLine(center, finalPos, Color.yellow, 5f);
            Debug.Log($"Passenger Pos: {finalPos}");
            var passenger = GameObject.Instantiate(passengerPrefab, finalPos, Quaternion.LookRotation(forward), parent);
            var ctrl = passenger.GetComponent<PassengerController>();
            var color = GetColorByIndex(door, ref counter);
            ctrl.SpawnAtOffset(finalPos, color);
        }
    }

    private static int GetTotalPassengerCount(DoorData door)
    {
        int total = 0;
        foreach (var c in door.IncomingCounts)
            total += c;
        return total;
    }

    private static ItemColor GetColorByIndex(DoorData door, ref int index)
    {
        for (int i = 0; i < door.IncomingCounts.Count; i++)
        {
            if (index < door.IncomingCounts[i])
            {
                index++;
                return door.IncomingColors[i];
            }
            index -= door.IncomingCounts[i];
        }
        return ItemColor.Red;
    }

    private static Vector3 GetDirectionOffset(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.forward,
            Direction.Down => Vector3.back,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.forward
        };
    }
}
