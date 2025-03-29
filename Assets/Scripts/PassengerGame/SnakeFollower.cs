using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class SnakeSwipeFollower : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int segmentCount = 5;
    public float moveTime = 0.2f;
    public Ease moveEase = Ease.Linear;

    private List<Transform> segments = new();
    private Vector3 startMousePos;
    private bool isMoving = false;
    private int activeIndex = -1;

    void Start()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject seg = Instantiate(segmentPrefab, new Vector3(-i, 0, 0), Quaternion.identity);
            segments.Add(seg.transform);
        }
    }

    void Update()
    {
        if (isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(startMousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int index = segments.FindIndex(s => s.gameObject == hit.transform.gameObject);
                if (index != -1)
                {
                    activeIndex = index;
                }
            }
        }

        if (Input.GetMouseButton(0) && activeIndex != -1)
        {
            Vector3 delta = Input.mousePosition - startMousePos;

            if (delta.magnitude > 50f)
            {
                Vector3 dir = GetSwipeDirection(delta);
                StartCoroutine(MoveFromEndNearestTo(activeIndex, dir));
                startMousePos = Input.mousePosition;
            }
        }
    }

    Vector3 GetSwipeDirection(Vector3 delta)
    {
        delta.Normalize();
        return Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? (delta.x > 0 ? Vector3.right : Vector3.left)
            : (delta.y > 0 ? Vector3.forward : Vector3.back);
    }

    System.Collections.IEnumerator MoveFromEndNearestTo(int selectedIndex, Vector3 dir)
    {
        isMoving = true;

        Vector3[] oldPositions = new Vector3[segments.Count];
        for (int i = 0; i < segments.Count; i++)
            oldPositions[i] = segments[i].position;

        bool closerToHead = selectedIndex < segments.Count / 2;

        if (closerToHead)
        {
            // Head moves forward
            Vector3 nextHeadPos = segments[0].position + dir;
            segments[0].DOMove(nextHeadPos, moveTime).SetEase(moveEase);

            for (int i = 1; i < segments.Count; i++)
            {
                Vector3 targetPos = oldPositions[i - 1];
                Vector3 currentDir = (segments[i].position - targetPos).normalized;
                segments[i].DOMove(targetPos, moveTime).SetEase(moveEase).OnStart(() =>
                {
                    segments[i].forward = -currentDir;
                });
            }
        }
        else
        {
            // Tail moves forward
            Vector3 nextTailPos = segments[^1].position + dir;
            segments[^1].DOMove(nextTailPos, moveTime).SetEase(moveEase);

            for (int i = segments.Count - 2; i >= 0; i--)
            {
                Vector3 targetPos = oldPositions[i + 1];
                Vector3 currentDir = (segments[i].position - targetPos).normalized;
                segments[i].DOMove(targetPos, moveTime).SetEase(moveEase).OnStart(() =>
                {
                    segments[i].forward = -currentDir;
                });
            }
        }

        yield return new WaitForSeconds(moveTime);
        isMoving = false;
    }
}