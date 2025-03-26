using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Slinky
{
    public class SlinkyController : MonoBehaviour, IInitializable, ISlotItem
    {
        [Header("Segment & Physics Settings")]
        [SerializeField] private GameObject _segmentPrefab;
        [SerializeField] private int segmentsPerUnit = 5;
        [SerializeField] private float heightMultiplierFactor = 0.5f;
        [SerializeField] private float horizontalTightness = 0.8f;
        [SerializeField] private float springForce = 1000f;
        [SerializeField] private float dampingFactor = 80f;

        [Header("Movement Settings")]
        [SerializeField] private float _moveDuration = 0.5f;
        public float MoveDurationValue => _moveDuration;

        [SerializeField] private float _shiftingDuration = 0.2f;
        public float ShiftingDuration => _shiftingDuration;

        public Ease MovementEase => _movementEase;
        [SerializeField] private Ease _movementEase = Ease.OutElastic;
        public float DelayBetweenSegments => delayBetweenSegments;
        [SerializeField] private float delayBetweenSegments = 0.02f;
        public float ArcHeight => arcHeight;
        [SerializeField] private float arcHeight = 1.5f;
        public float MoveTimeMultiplierValue => moveTimeMultiplier;
        [SerializeField] private float moveTimeMultiplier = 1.5f;

        private Transform _slinkyParent;

        public List<Transform> Segments => _segments;
        private List<Transform> _segments = new();
        public bool IsMoving=>_isMoving;
        private bool _isMoving = false;
        public bool IsMatch { get;  set; }
        private bool _isSelected = false;
        public BasicSignal OnMovementComplete { get; private set; }
        private GridManager _gridManager;
       
        public bool Initialized { get; set; }

        private GameObject _startSlotObject;
        private GameObject _endSlotObject;

        public Vector3 StartPosition => _startPosition;
        private Vector3 _startPosition;

        public Vector3 EndPosition => _endPosition;
        private Vector3 _endPosition;

        public GameKey SlotIndex { get; set; }
        public int SegmentCount => _segments.Count;
        public List<GameKey> OccupiedGridKeys { get; private set; } = new List<GameKey>();

        private SlinkyMover _slinkyMover;

        public GameObject Root => gameObject;

        private ItemColor _itemColor;
        public ItemColor ItemColor
        {
            get => _itemColor;
            set => _itemColor = value;
        }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            Initialized = true;
            OnMovementComplete = new BasicSignal();
            _gridManager = GridManager.Instance;
        }

        public void Initialize(Vector3 startPos, Vector3 endPos, GridManager gridManager, ItemColor color, GameObject startSlot, GameObject endSlot, Transform slinkyParent)
        {
            _startSlotObject = startSlot;
            _endSlotObject = endSlot;
            _itemColor = color;
            _slinkyParent = slinkyParent;
            _segments.Clear();
            _gridManager = gridManager;
            transform.SetParent(_slinkyParent);
            transform.position = startPos;

            _startPosition = startPos;
            _endPosition = endPos;
            OccupiedGridKeys.Clear();
            OccupiedGridKeys.Add(gridManager.GetGridKeyFromPosition(startPos));
            OccupiedGridKeys.Add(gridManager.GetGridKeyFromPosition(endPos));
            CreateSegments(startPos, endPos, transform);
           
        }

        private void CreateSegments(Vector3 startPos, Vector3 endPos, Transform parent)
        {
            float totalDistance = Vector3.Distance(startPos, endPos);
            int segmentCount = Mathf.Max(5, Mathf.RoundToInt(totalDistance * segmentsPerUnit));
            float segmentSpacing = (totalDistance * horizontalTightness) / (segmentCount - 1);
            float heightMultiplier = totalDistance * heightMultiplierFactor;

            GameObject previousSegment = _startSlotObject;

            for (int i = 0; i < segmentCount; i++)
            {
                float t = i / (float)(segmentCount - 1);

                float x = Mathf.Lerp(startPos.x, endPos.x, t);
                float z = Mathf.Lerp(startPos.z, endPos.z, t);

                float height = -4 * heightMultiplier * (t - 0.5f) * (t - 0.5f) + heightMultiplier;
                float y = startPos.y + height;

                Vector3 position = new Vector3(x, y, z);
                GameObject segment = Instantiate(_segmentPrefab, position, Quaternion.identity);
                segment.transform.localScale *= 1.0f;
                segment.transform.SetParent(parent);
                segment.AddComponent<MouseInteractable>();
                Renderer renderer = segment.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = SlinkyColorUtility.GetColor(_itemColor);
                }

                Rigidbody rb = segment.AddComponent<Rigidbody>();
                rb.mass = 2f;
                rb.drag = 0.1f;
                rb.angularDrag = 2f;
                rb.useGravity = false;

                HingeJoint hinge = segment.AddComponent<HingeJoint>();
                hinge.connectedBody = previousSegment.GetComponent<Rigidbody>();
                hinge.useSpring = true;

                JointSpring spring = new JointSpring();
                spring.spring = springForce;
                spring.damper = dampingFactor;
                hinge.spring = spring;

                previousSegment = segment;
                _segments.Add(segment.transform);
            }

            HingeJoint lastHinge = _segments[_segments.Count - 1].gameObject.AddComponent<HingeJoint>();
            lastHinge.connectedBody = _endSlotObject.GetComponent<Rigidbody>();
        }
        public void SetIsMoving(bool value) => _isMoving = value;

        public void DestroySegments()
        {
            foreach (var segment in Segments)
            {
                if (segment != null)
                {
                    segment.gameObject.SetActive(false);
                }
            }

            Segments.Clear();
        }
        public void OnSegmentClicked()
        {
            if (_isSelected || _isMoving)
            {
                return;
            }
            if (IsThereLongerSlinkyBlocking(this))
            {
                return;
            }
            GameKey emptySlotKey = _gridManager.GetFirstColorEmptySlotOrNextToMatch(this);

            if (emptySlotKey == null)
            {
                return;
            }
            Vector3 targetPosition = _gridManager.GetSlotPosition(emptySlotKey, false);
            if (_gridManager.IsSlotOccupied(emptySlotKey))
            {
                bool shiftSuccess = _slinkyMover.ShiftUntilFit(emptySlotKey,this);
                if (!shiftSuccess)
                {
                    return;
                }
            }
            _isSelected = true;
            MoveTo(emptySlotKey);
        }
        public void MoveTo(GameKey targetKey)
        {
            SlinkyMover.Move(this, targetKey);
        }
        public bool IsThereLongerSlinkyBlocking(SlinkyController slinky)
        {
            foreach (var segment in slinky.Segments)
            {
                RaycastHit[] hits = Physics.RaycastAll(segment.position, Vector3.up, 5f);
                foreach (var hit in hits)
                {
                    var other = hit.collider.GetComponentInParent<SlinkyController>();
                    if (other != null && other != slinky)
                        return true;
                }
            }
            return false;
        }
        public bool MatchesWith(ISlotItem other)
        {
            if (other is SlinkyController otherSlinky)
                return _itemColor == otherSlinky._itemColor;

            return false;
        }

        //void OnDrawGizmos()
        //{
        //    if (!Application.isPlaying) return;

        //    foreach (var segment in _segments)
        //    {
        //        Vector3 startPos = segment.position;
        //        Vector3 direction = Vector3.up;
        //        float distance = 2f;

        //        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, distance);
        //        bool hasValidHit = false;

        //        foreach (var hit in hits)
        //        {
        //            if (_segments.Contains(hit.collider.transform)) continue;

        //            hasValidHit = true;
        //            Gizmos.color = Color.red;
        //            Gizmos.DrawSphere(hit.point, 0.2f);
        //            break;
        //        }

        //        if (!hasValidHit)
        //        {
        //            Gizmos.color = Color.green;
        //        }

        //        Gizmos.DrawLine(startPos, startPos + direction * distance);
        //    }
        // }
    }
}
