using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;

namespace GreatGames.CaseLib.Slinky
{
    public class SlinkyController : MonoBehaviour, IInitializable
    {
        [Header("Segment & Physics Settings")]
        [SerializeField] private GameObject _segmentPrefab;
        [SerializeField] private float _segmentSpacing = 0.5f;
        [SerializeField] private float _springStiffness = 15f;
        [SerializeField] private float _springDamping = 0.8f;

        [Header("Movement Settings")]
        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private Ease _movementEase = Ease.OutElastic; 

        private List<Transform> _segments = new();
        private bool _isMoving = false;
        private bool _isSelected = false;
        public BasicSignal OnMovementComplete { get; private set; }
        private GridManager _gridManager;
        private Color _slinkyColor;
        public bool Initialized { get; set; }

        public void Init()
        {
            Initialized = true;
            OnMovementComplete = new BasicSignal();
        }

        public void Initialize(Vector3 startPos, Vector3 endPos, SlinkyColor color, GridManager gridManager)
        {
            if (gridManager == null)
            {
                Debug.LogError("GridManager not found!");
                return;
            }

            _gridManager = gridManager;
            _slinkyColor = SlinkyColorUtility.GetColor(color);

            transform.position = startPos;
            _segments.Clear();
            _isSelected = false;

            CreateSegments(startPos, endPos);
        }

        private void CreateSegments(Vector3 startPos, Vector3 endPos)
        {
            float distance = Vector3.Distance(startPos, endPos);
            int segmentCount = Mathf.Max(3, Mathf.CeilToInt(distance / _segmentSpacing));

            Transform prevSegment = null;
            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 segmentPosition = Vector3.Lerp(startPos, endPos, (float)i / (segmentCount - 1));
                GameObject segment = Instantiate(_segmentPrefab, segmentPosition, Quaternion.identity, transform);
                segment.GetComponentInChildren<Renderer>().material.color = _slinkyColor;
                _segments.Add(segment.transform);

                Rigidbody rb = segment.AddComponent<Rigidbody>();
                rb.mass = 0.1f;
                rb.drag = 0.2f;

                if (prevSegment != null)
                {
                    SpringJoint joint = segment.AddComponent<SpringJoint>();
                    joint.connectedBody = prevSegment.GetComponent<Rigidbody>();
                    joint.spring = _springStiffness;
                    joint.damper = _springDamping;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.anchor = Vector3.zero;
                    joint.connectedAnchor = Vector3.zero;
                }

                prevSegment = segment.transform;
            }
        }

        public void OnSegmentClicked()
        {
            if (!_isSelected && !_isMoving)
            {
                GameKey emptySlotKey = _gridManager.GetFirstEmptySlot(true);
                if (emptySlotKey != null)
                {
                    _isSelected = _gridManager.TryPlaceSlinky(emptySlotKey, new SlinkyData(0, 0, SlinkyColor.Red), true);

                }
                else
                {
                    Debug.LogWarning("No available slot found!");
                }
            }
        }

        public void MoveToTarget(Vector3 targetPosition)
        {
            if (_isMoving || _segments.Count == 0) return;
            _isMoving = true;

            foreach (Transform segment in _segments)
            {
                segment.DOMove(targetPosition, _moveDuration).SetEase(_movementEase); 
            }

            DOVirtual.DelayedCall(_moveDuration + 0.1f, () =>
            {
                _isMoving = false;
                OnMovementComplete.Emit();
            });
        }
    }
}
