using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }
    private bool _isMatching = false;
    private GridManager _gridManager;
    private AnimationQueue animationQueue = new AnimationQueue();
    private void Start()
    {
        _gridManager = GridManager.Instance;
    }
    public void CheckForMatch()
    {
        Debug.Log("✅ MatchManager.CheckForMatch() çağrıldı.");
        if (_isMatching) return;

        List<SlinkyController> slinkies = _gridManager.GetAllSlinkiesInLowerGrid();
        Debug.Log($"Alt griddeki slinky sayısı: {slinkies.Count}");
        foreach (var s in slinkies)
        {
            string colorText = s?.SlinkyColor.ToString() ?? "null";
            Debug.Log($"Slinky: Color = {colorText}, Slot = {s.SlotIndex?.ValueAsString ?? "null"}");
        }
        var groupedByRow = GroupSlinkiesByRow(slinkies);

        foreach (var row in groupedByRow)
        {
            var ordered = row.Value.OrderBy(s => s.SlotIndex.ToVector2Int().x).ToList();

            Debug.Log($"🧪 Row {row.Key} içinde {ordered.Count} slinky var:");
            foreach (var s in ordered)
            {
                Debug.Log($"🧪  → {s.SlotIndex.ValueAsString} [{s.SlinkyColor}]");
            }

            for (int i = 0; i <= ordered.Count - 3; i++)
            {
                var a = ordered[i];
                var b = ordered[i + 1];
                var c = ordered[i + 2];

                Debug.Log($"🧪 Kontrol: {a.SlotIndex.ValueAsString}, {b.SlotIndex.ValueAsString}, {c.SlotIndex.ValueAsString}");

                if (IsSameColor(a, b, c))
                {
                    Debug.Log("🎯 Eşleşme bulundu!");
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
        if (left == null || middle == null || right == null)
        {
            Debug.LogWarning("❌ HandleMatch içinde null slinky referansı!");
            yield break;
        }

        _isMatching = true;
        Vector3 middlePos = _gridManager.GetSlotPosition(middle.SlotIndex, false);

        bool leftDone = false;
        bool rightDone = false;

        // Eventleri önce temizle, sonra bağla
        left.OnMovementComplete.DisconnectAll();
        right.OnMovementComplete.DisconnectAll();

        left.OnMovementComplete.Connect(() => leftDone = true);
        right.OnMovementComplete.Connect(() => rightDone = true);

        GameKey leftSlot = left.SlotIndex;
        GameKey middleSlot = middle.SlotIndex;
        GameKey rightSlot = right.SlotIndex;

        // Sırasıyla hedefe taşı
        left.MoveToTarget(middlePos, middle.SlotIndex);
        yield return new WaitUntil(() => leftDone);

        right.MoveToTarget(middlePos, middle.SlotIndex);
        yield return new WaitUntil(() => rightDone);

        _gridManager.RemoveSlinkyAt(leftSlot);
        _gridManager.RemoveSlinkyAt(middleSlot);
        _gridManager.RemoveSlinkyAt(rightSlot);

        // 🔧 Ardından segmentleri yok et
        left.DestroySegments();
        middle.DestroySegments();
        right.DestroySegments();

        yield return new WaitForSeconds(0.1f);

        // ⛔ GameObject'leri en son destroy et
        if (left != null) Destroy(left.gameObject);
        if (middle != null) Destroy(middle.gameObject);
        if (right != null) Destroy(right.gameObject);

        yield return new WaitForSeconds(0.1f);

        _gridManager.ShiftRemainingSlinkies();

        yield return new WaitForSeconds(0.2f);

        _isMatching = false;

        CheckForMatch();
    }



}
public class AnimationQueue
{
    private Queue<IEnumerator> _animationQueue = new Queue<IEnumerator>();
    private bool _isAnimating = false;

    // Method to add animations to the queue
    public void AddToQueue(IEnumerator animation)
    {
        _animationQueue.Enqueue(animation);
        if (!_isAnimating)
        {
            ProcessQueue();
        }
    }

    private void ProcessQueue()
    {
        if (_animationQueue.Count > 0)
        {
            _isAnimating = true;
            // Start the next animation in the queue
            CoroutineRunner.Instance.StartCoroutine(_animationQueue.Dequeue());
        }
        else
        {
            _isAnimating = false;
        }
    }

    // Method to signal completion and start the next animation
    public void OnAnimationComplete()
    {
        ProcessQueue();
    }
    public class CoroutineRunner : MonoBehaviour
    {
        // Singleton instance
        public static CoroutineRunner Instance { get; private set; }

        private void Awake()
        {
            // Ensure only one instance of CoroutineRunner exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Method to start a coroutine from any other class
        public void StartCoroutineFromOtherClass(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}