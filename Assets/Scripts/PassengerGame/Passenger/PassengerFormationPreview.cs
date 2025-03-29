using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PassengerLinePreview : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public int PassengerCount = 6;
    public Vector3 Forward = Vector3.forward;
    public float LookDistance = 0.5f;

    private void OnDrawGizmos()
    {
        if (LineRenderer == null || LineRenderer.positionCount < 2)
            return;

        var points = EvaluatePointsFromLine(LineRenderer, PassengerCount);
        ShiftPointsToStartFromZero(points);

        Vector3 origin = transform.position;
        Vector3? previousPos = null;

        foreach (var offset in points)
        {
            Vector3 right = Vector3.Cross(Vector3.up, Forward);
            Vector3 worldOffset = offset.z * Forward + offset.x * right;
            Vector3 worldPos = origin + worldOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(worldPos, 0.1f);

            Vector3 lookDir = previousPos == null ? Forward : (previousPos.Value - worldPos).normalized;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(worldPos, lookDir * LookDistance);

            previousPos = worldPos;
        }
    }

    private List<Vector3> EvaluatePointsFromLine(LineRenderer line, int count)
    {
        List<Vector3> positions = new();
        if (line == null || line.positionCount < 2) return positions;

        float totalLength = 0f;
        List<Vector3> localPoints = new();

        for (int i = 0; i < line.positionCount; i++)
            localPoints.Add(line.GetPosition(i)); // local space

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

    private void ShiftPointsToStartFromZero(List<Vector3> points)
    {
        if (points == null || points.Count == 0) return;

        Vector3 first = points[0];
        for (int i = 0; i < points.Count; i++)
            points[i] -= first;
    }
}
