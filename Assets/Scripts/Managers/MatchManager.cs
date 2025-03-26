//using GreatGames.CaseLib.Grid;
//using GreatGames.CaseLib.Key;
//using GreatGames.CaseLib.Patterns;
//using GreatGames.CaseLib.Slinky;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
//{
//    public bool Initialized { get; set; }
//    private bool _isMatching = false;
//    private GridManager _gridManager;
//    [SerializeField] private float _mergeBetweenTimer = 0.1f;
//    private void Start()
//    {
//        _gridManager = GridManager.Instance;
//    }
//    public void CheckForMatch()
//    {
//        if (_isMatching) return;

//        List<ISlotItem> slinkies = _gridManager.GetAllItemsInLowerGrid();
//        var groupedByRow = GroupItemsByRow(slinkies);

//        foreach (var row in groupedByRow)
//        {
//            var ordered = row.Value.OrderBy(s => s.SlotIndex.ToVector2Int().x).ToList();

//            for (int i = 0; i <= ordered.Count - 3; i++)
//            {
//                var a = ordered[i];
//                var b = ordered[i + 1];
//                var c = ordered[i + 2];

//                if (IsSameColor(a, b, c))
//                {
//                    StartCoroutine(HandleMatch(a, b, c));
//                    return;
//                }
//            }

//        }
//    }
//    private Dictionary<int, List<ISlotItem>> GroupItemsByRow(List<ISlotItem> items)
//    {
//        Dictionary<int, List<ISlotItem>> grouped = new();

//        foreach (var slinky in items)
//        {
//            int row = slinky.SlotIndex.ToVector2Int().y;
//            if (!grouped.ContainsKey(row))
//                grouped[row] = new List<ISlotItem>();

//            grouped[row].Add(slinky);
//        }

//        return grouped;
//    }

//    private bool IsSameColor(ISlotItem a, ISlotItem b, ISlotItem c)
//    {
//        return a.ItemColor == b.ItemColor && a.ItemColor == c.ItemColor;
//    }
//    public bool CheckAnyAvailableMatch()
//    {
//        var allSlinkies = GridManager.Instance.GetAllItemsInLowerGrid();

//        for (int i = 1; i < allSlinkies.Count - 1; i++)
//        {
//            var left = allSlinkies[i - 1];
//            var middle = allSlinkies[i];
//            var right = allSlinkies[i + 1];

//            if (IsSameColor(left, middle, right))
//            {
//                return true;
//            }
//        }

//        return false;
//    }
//    private IEnumerator HandleMatch(SlinkyController left, SlinkyController middle, SlinkyController right)
//    {
//        if (left == null || middle == null || right == null 
//            || left.IsMoving  || middle.IsMoving || right.IsMoving)
//        {
//            yield break;
//        }

//        _isMatching = true;
//        Vector3 middlePos = _gridManager.GetSlotPosition(middle.SlotIndex, false);

//        bool leftDone = false;
//        bool rightDone = false;
//        left.IsMatch = true;
//        right.IsMatch = true;
//        middle.IsMatch = true;

//        left.OnMovementComplete.DisconnectAll();
//        right.OnMovementComplete.DisconnectAll();

//        left.OnMovementComplete.Connect(() => leftDone = true);
//        right.OnMovementComplete.Connect(() => rightDone = true);

//        GameKey leftSlot = left.SlotIndex;
//        GameKey middleSlot = middle.SlotIndex;
//        GameKey rightSlot = right.SlotIndex;


//        left.MoveTo(middle.SlotIndex);
//        yield return new WaitUntil(() => leftDone);

//        right.MoveTo(middle.SlotIndex);
//        yield return new WaitUntil(() => rightDone);

//        VFXManager.Instance.PlayMergeParticle(middlePos);

//        _gridManager.RemoveSlinkyAt(leftSlot);
//        _gridManager.RemoveSlinkyAt(middleSlot);
//        _gridManager.RemoveSlinkyAt(rightSlot);

//        left.DestroySegments();
//        middle.DestroySegments();
//        right.DestroySegments();

//        yield return new WaitForSeconds(_mergeBetweenTimer);

//        if (left != null)left.gameObject.SetActive(false);
//        if (middle != null) middle.gameObject.SetActive(false);
//        if (right != null) right.gameObject.SetActive(false);

//        yield return new WaitForSeconds(_mergeBetweenTimer);
//        _gridManager.ShiftRemainingSlinkies();
//        yield return new WaitForSeconds(_mergeBetweenTimer);

//        _isMatching = false;
//        GameManager.Instance.CheckForCompletion();
//        GameManager.Instance.CheckGameState();
//        CheckForMatch();
//    }
// }
