using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlinkyCreator : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject startAnchor;
    public GameObject endAnchor;

    public int segmentsPerUnit = 5;  
    public float heightMultiplier = 2f;
    public float horizontalTightness = 0.8f;

    private List<GameObject> spheres = new List<GameObject>();

    void Start()
    {
        CreateSlinky();
    }

    void CreateSlinky()
    {
        GameObject previousSphere = startAnchor;

        // 🔥 Toplam mesafeyi hesapla
        float totalDistance = Vector3.Distance(startAnchor.transform.position, endAnchor.transform.position);
        int segmentCount = Mathf.Max(3, Mathf.RoundToInt(totalDistance * segmentsPerUnit));  
        float segmentSpacing = (totalDistance * horizontalTightness) / (segmentCount - 1);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);

            float x = Mathf.Lerp(startAnchor.transform.position.x, endAnchor.transform.position.x, t);
            float z = Mathf.Lerp(startAnchor.transform.position.z, endAnchor.transform.position.z, t);

            float height = -4 * heightMultiplier * (t - 0.5f) * (t - 0.5f) + heightMultiplier;
            float y = startAnchor.transform.position.y + height;

            Vector3 position = new Vector3(x, y, z);
            GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
            sphere.transform.localScale *= 1f;

            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.drag = 1f;
            rb.angularDrag = 0.1f;
            rb.useGravity = false;

            HingeJoint hinge = sphere.AddComponent<HingeJoint>();
            hinge.useLimits = true;

            if (previousSphere != null)
            {
                hinge.connectedBody = previousSphere.GetComponent<Rigidbody>();
            }

            sphere.AddComponent<MouseInteractable>();

            previousSphere = sphere;
            spheres.Add(sphere);
        }

        HingeJoint lastHinge = spheres[spheres.Count - 1].AddComponent<HingeJoint>();
        lastHinge.connectedBody = endAnchor.GetComponent<Rigidbody>();
    }
}
