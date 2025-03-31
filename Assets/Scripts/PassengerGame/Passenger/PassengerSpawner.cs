using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PassengerSpawner
{
    public static void SpawnPassengers(
        DoorData door,
        GridManager gridManager,
        GameObject passengerPrefab,
        Transform parent,
        float distanceFromDoor,
        float yOffset
    )
    {
        GameKey doorKey = gridManager.UpperGrid.CreateKeyFromIndex(door.SlotIndex);
        Vector3 doorPos = gridManager.GetSlotPosition(doorKey, true);
        Vector3 forward = GetDirectionOffset(door.EnterDirection);

        if (!door.PathLineObject.TryGetComponent<LineRenderer>(out var line) || line.positionCount < 2)
        {
            Debug.LogWarning($"LineRenderer not found for Door {door.SlotIndex}");
            return;
        }

        int totalCount = GetTotalPassengerCount(door);
        var points = EvaluatePointsFromLine(line, totalCount);
        ShiftPointsToStartFromZero(points);

        Vector3? previousPos = null;
        int counter = 0;

        foreach (var localOffset in points)
        {
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 worldOffset = localOffset.z * forward + localOffset.x * right + Vector3.up * yOffset;
            Vector3 finalPos = doorPos + forward * distanceFromDoor + worldOffset;

            Vector3 lookDir = previousPos == null ? forward : (previousPos.Value - finalPos).normalized;
            Quaternion rotation = Quaternion.LookRotation(lookDir);

            var passenger = GameObject.Instantiate(passengerPrefab, finalPos, rotation, parent);
            var ctrl = passenger.GetComponent<PassengerController>();
            var color = GetColorByIndex(door, ref counter);
            ctrl.Initialize(doorKey, color);
            ctrl.SpawnAtOffset(finalPos, color);

            var doorController = DoorManager.Instance.GetDoorAt(doorKey);

            if (doorController != null)
            {
                doorController.AddPassenger(ctrl);
            }
            previousPos = finalPos;
        }
    }

    private static List<Vector3> EvaluatePointsFromLine(LineRenderer line, int count)
    {
        List<Vector3> positions = new();
        if (line == null || line.positionCount < 2) return positions;

        float totalLength = 0f;
        List<Vector3> localPoints = new();

        for (int i = 0; i < line.positionCount; i++)
            localPoints.Add(line.GetPosition(i));

        for (int i = 1; i < localPoints.Count; i++)
            totalLength += Vector3.Distance(localPoints[i - 1], localPoints[i]);

        float spacing = totalLength / Mathf.Max(count - 1, 1);
        float distanceCovered = 0f;
        int currentIndex = 0;

        while (positions.Count < count && currentIndex < localPoints.Count - 1)
        {
            Vector3 start = localPoints[currentIndex];
            Vector3 end = localPoints[currentIndex + 1];
            float segmentLength = Vector3.Distance(start, end);

            if (distanceCovered <= segmentLength)
            {
                float t = distanceCovered / segmentLength;
                Vector3 point = Vector3.Lerp(start, end, t);
                positions.Add(point);
                distanceCovered += spacing;
            }
            else
            {
                distanceCovered -= segmentLength;
                currentIndex++;
            }
        }

        return positions;
    }

    private static void ShiftPointsToStartFromZero(List<Vector3> points)
    {
        if (points == null || points.Count == 0) return;

        Vector3 first = points[0];
        for (int i = 0; i < points.Count; i++)
            points[i] -= first;
    }

    private static int GetTotalPassengerCount(DoorData door)
    {
        int total = 0;
        foreach (var c in door.IncomingCounts)
            total += c;
        return total;
    }

    private static ItemColor GetColorByIndex(DoorData door, ref int globalIndex)
    {
        int localIndex = globalIndex;
        for (int i = 0; i < door.IncomingCounts.Count; i++)
        {
            if (localIndex < door.IncomingCounts[i])
            {
                globalIndex++; 
                return door.IncomingColors[i];
            }
            localIndex -= door.IncomingCounts[i];
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
