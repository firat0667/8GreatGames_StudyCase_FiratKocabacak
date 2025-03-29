using UnityEngine;

[ExecuteInEditMode]
public class PassengerFormationPreview : MonoBehaviour
{
    public PassengerFormationSO Formation;
    public int PassengerCount = 6;
    public Vector3 Forward = Vector3.forward;

    private void OnDrawGizmos()
    {
        if (Formation == null) return;

        Gizmos.color = Color.green;
        Vector3 center = transform.position;

        var points = Formation.EvaluatePoints(PassengerCount);
        foreach (var p in points)
        {
            Vector3 worldPos = center + Quaternion.LookRotation(Forward) * p;
            Gizmos.DrawSphere(worldPos, 0.1f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, Formation.Radius);
    }
}
