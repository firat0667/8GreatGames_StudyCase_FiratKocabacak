using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreatGames.CaseLib.Slinky
{
    public class SlinkyController : MonoBehaviour, IInitializable
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
        [SerializeField] private Ease _movementEase = Ease.OutElastic;
        [SerializeField] private float delayBetweenSegments = 0.02f; 
        [SerializeField] private float arcHeight = 1.5f; 
        [SerializeField] private float moveTimeMultiplier = 1.5f; 

        private Transform _slinkyParent;

        public List<Transform> Segments => _segments;
        private List<Transform> _segments = new();

        private bool _isMoving = false;
        private bool _isSelected = false;
        public BasicSignal OnMovementComplete { get; private set; }
        private GridManager _gridManager;
        public Color SlinkyColor => _slinkyColor;
        private Color _slinkyColor;
        public bool Initialized { get; set; }

        private GameObject _startSlotObject;
        private GameObject _endSlotObject;

        public Vector3 StartPosition => _startPosition;
        private Vector3 _startPosition;

        public Vector3 EndPosition => _endPosition;
        private Vector3 _endPosition;
        public GameKey SlotIndex { get; set; }
        public bool IsMoving { get; private set; } = false;
        public int SegmentCount => _segments.Count;
        public List<GameKey> OccupiedGridKeys { get; private set; } = new List<GameKey>();
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

        public void Initialize(Vector3 startPos, Vector3 endPos, GridManager gridManager, SlinkyColor color, GameObject startSlot, GameObject endSlot, Transform slinkyParent)
        {
            _startSlotObject = startSlot;
            _endSlotObject = endSlot;
            _slinkyColor = SlinkyColorUtility.GetColor(color);
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
            _gridManager.RegisterSlinky(this);
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
                    renderer.material.color = _slinkyColor;
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
        public void DestroySegments()
        {
            foreach (var segment in Segments)
            {
                if (segment != null)
                {
                    Destroy(segment.gameObject); 
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

            GameKey emptySlotKey = _gridManager.GetFirstEmptySlot(false);

            if (emptySlotKey == null)
            {
                return;
            }

            Vector3 targetPosition = _gridManager.GetSlotPosition(emptySlotKey, false);

            if (_gridManager.IsThereLongerSlinkyBlocking(this))
            {
                return;
            }

            _isSelected = true;
            MoveToTarget(targetPosition, emptySlotKey);
        }
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            foreach (var segment in _segments) 
            {
                Vector3 startPos = segment.position;
                Vector3 direction = Vector3.up; 
                float distance = 2f; 

                RaycastHit[] hits = Physics.RaycastAll(startPos, direction, distance);
                bool hasValidHit = false;

                foreach (var hit in hits)
                {
                    if (_segments.Contains(hit.collider.transform)) continue;

                    hasValidHit = true; 
                    Gizmos.color = Color.red; 
                    Gizmos.DrawSphere(hit.point, 0.2f);
                    break; 
                }

                if (!hasValidHit)
                {
                    Gizmos.color = Color.green; 
                }

                Gizmos.DrawLine(startPos, startPos + direction * distance);
            }
        }

        public void MoveToTarget(Vector3 targetPosition, GameKey newSlotKey)
        {
            if (_isMoving) return;
            _isMoving = true;

            if (newSlotKey == null)
            {
                GameKey emptySlotKey = _gridManager.GetFirstEmptySlot(false);
                if (emptySlotKey == null)
                {
                    _isMoving = false;
                    return;
                }
                newSlotKey = emptySlotKey;
                targetPosition = _gridManager.GetSlotPosition(newSlotKey, false);
              
            }
            _gridManager.TryPlaceSlinky(newSlotKey, this, false);
            SlotIndex = newSlotKey;
            OccupiedGridKeys.Clear();
            OccupiedGridKeys.Add(newSlotKey);
            foreach (Transform segment in _segments)
            {
                HingeJoint hinge = segment.GetComponent<HingeJoint>();
                if (hinge != null)
                {
                    hinge.connectedBody = null;
                    Destroy(hinge);
                }
            }

            foreach (Transform segment in _segments)
            {
                Rigidbody rb = segment.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }

            float moveTime = _moveDuration * moveTimeMultiplier;

            for (int i = 0; i < _segments.Count; i++)
            {
                int index = i;
                float delay = i * delayBetweenSegments;

                Vector3 startPos = _segments[index].position;
                Vector3 midPoint = (startPos + targetPosition) / 2 + Vector3.up * arcHeight;

                Tween moveTween = _segments[index].DOPath(new Vector3[] { midPoint, targetPosition }, moveTime, PathType.CatmullRom)
                    .SetDelay(delay)
                    .SetEase(_movementEase)
                    .SetRelative(false);
            }

            DOVirtual.DelayedCall(moveTime + delayBetweenSegments * _segments.Count, () =>
            {
                _isMoving = false;
                OnMovementComplete?.Emit();
                MatchManager.Instance.CheckForMatch();
                _gridManager.LogLowerGridSlotStates();
            });
        }

    }
}
