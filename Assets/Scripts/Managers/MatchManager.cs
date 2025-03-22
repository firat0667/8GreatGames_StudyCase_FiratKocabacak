using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    private bool _isMatching = false;
    private GridManager _gridManager;
    private void Start()
    {
        _gridManager = GridManager.Instance;
    }
    public void CheckForMatch()
    {
        if (_isMatching) return;

        List<SlinkyController> slinkies = _gridManager.GetAllSlinkiesInLowerGrid();
        var groupedByRow = GroupSlinkiesByRow(slinkies);

        foreach (var row in groupedByRow)
        {
            var ordered = row.Value.OrderBy(s => s.SlotIndex.ToVector2Int().x).ToList();

            for (int i = 0; i <= ordered.Count - 3; i++)
            {
                var a = ordered[i];
                var b = ordered[i + 1];
                var c = ordered[i + 2];

                if (IsSameColor(a, b, c))
                {
                    StartCoroutine(HandleMatch(a, b, c));
                    return;
                }
            }

        }
    }
    private Dictionary<int, List<SlinkyController>> GroupSlinkiesByRow(List<SlinkyController> slinkies)
    {
        Dictionary<int, List<SlinkyController>> grouped = new();

        foreach (var slinky in slinkies)
        {
            int row = slinky.SlotIndex.ToVector2Int().y;
            if (!grouped.ContainsKey(row))
                grouped[row] = new List<SlinkyController>();

            grouped[row].Add(slinky);
        }

        return grouped;
    }

    private bool IsSameColor(SlinkyController a, SlinkyController b, SlinkyController c)
    {
        return a.SlinkyColor == b.SlinkyColor && a.SlinkyColor == c.SlinkyColor;
    }

    private IEnumerator HandleMatch(SlinkyController left, SlinkyController middle, SlinkyController right)
    {
        if (left == null || middle == null || right == null || left.IsMoving  || middle.IsMoving || right.IsMoving)
        {
            yield break;
        }

        _isMatching = true;
        Vector3 middlePos = _gridManager.GetSlotPosition(middle.SlotIndex, false);

        bool leftDone = false;
        bool rightDone = false;
        left.IsMatch = true;
        right.IsMatch = true;
        middle.IsMatch = true;

        left.OnMovementComplete.DisconnectAll();
        right.OnMovementComplete.DisconnectAll();

        left.OnMovementComplete.Connect(() => leftDone = true);
        right.OnMovementComplete.Connect(() => rightDone = true);

        GameKey leftSlot = left.SlotIndex;
        GameKey middleSlot = middle.SlotIndex;
        GameKey rightSlot = right.SlotIndex;

        left.MoveToTarget(middlePos, middle.SlotIndex);
        yield return new WaitUntil(() => leftDone);

        right.MoveToTarget(middlePos, middle.SlotIndex);
        yield return new WaitUntil(() => rightDone);
        VFXManager.Instance.PlayMergeParticle(middle.gameObject.transform);
        _gridManager.RemoveSlinkyAt(leftSlot);
        _gridManager.RemoveSlinkyAt(middleSlot);
        _gridManager.RemoveSlinkyAt(rightSlot);

        left.DestroySegments();
        middle.DestroySegments();
        right.DestroySegments();

        yield return new WaitForSeconds(0.1f);

        if (left != null) Destroy(left.gameObject);
        if (middle != null) Destroy(middle.gameObject);
        if (right != null) Destroy(right.gameObject);

        yield return new WaitForSeconds(0.2f);
        _gridManager.ShiftRemainingSlinkies();
        yield return new WaitForSeconds(0.2f);

        _isMatching = false;

        CheckForMatch();
    }
 }
