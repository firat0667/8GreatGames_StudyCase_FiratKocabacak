using UnityEngine;
using System.Collections.Generic;

public enum PassengerFormationType
{
    Radial,
    Linear,
    LShape,
}

[CreateAssetMenu(fileName = "Formation", menuName = "Game/Passenger Formation")]
public class PassengerFormationSO : ScriptableObject
{
    public PassengerFormationType FormationType = PassengerFormationType.Radial;

    public float Radius = 2f;
    [Range(0f, 360f)] public float AngleRange = 90f;
    public bool Clockwise = false;

    public float Spacing = 0.6f;

    public int BreakAfterIndex = 3;

    public List<Vector3> EvaluatePoints(int count)
    {
        return FormationType switch
        {
            PassengerFormationType.Radial => EvaluateRadial(count),
            PassengerFormationType.Linear => EvaluateLinear(count),
            PassengerFormationType.LShape => EvaluateLShape(count),
            _ => new List<Vector3>()
        };
    }

    private List<Vector3> EvaluateRadial(int count)
    {
        List<Vector3> positions = new();

        if (count <= 0) return positions;

        float angleStep = AngleRange / Mathf.Max(count - 1, 1);
        float startAngle = -AngleRange / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + i * angleStep;
            if (Clockwise) angle = -angle;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)) * Radius;
            positions.Add(pos);
        }

        return positions;
    }

    private List<Vector3> EvaluateLinear(int count)
    {
        List<Vector3> points = new();
        for (int i = 0; i < count; i++)
        {
            points.Add(Vector3.forward * i * Spacing);
        }
        return points;
    }

    private List<Vector3> EvaluateLShape(int count)
    {
        List<Vector3> points = new();
        int breakPoint = Mathf.Clamp(BreakAfterIndex, 1, count);

        for (int i = 0; i < count; i++)
        {
            if (i < breakPoint)
                points.Add(Vector3.forward * i * Spacing);
            else
                if (i < breakPoint)
                points.Add(new Vector3(0f, 0f, i * Spacing));
            else
                points.Add(new Vector3((i - breakPoint + 1) * Spacing, 0f, (breakPoint - 1) * Spacing));
        }

        return points;
    }
}
