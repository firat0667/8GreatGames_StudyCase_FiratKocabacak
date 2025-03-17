using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using System;
using GreatGames.CaseLib.Utility;

namespace GreatGames.CaseLib.Slinky
{
    public class SlinkyController : MonoBehaviour, IInitializable
    {
        [SerializeField] private GameObject _segmentPrefab;
        [SerializeField] private float _segmentSpacing = 0.5f;
        [SerializeField] private float _travelTime = 0.5f;
        [SerializeField] private float _delayBetweenSegments = 0.05f;
        [SerializeField] private float _springFactor = 0.15f;
        [SerializeField] private Ease _movementEase = Ease.InOutSine;

        private List<Transform> _segments;
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

        public void Initialize(Vector3 startPos, Vector3 endPos, int segmentCount, SlinkyColor color, GridManager gridManager)
        {
            if (gridManager == null)
            {
                Debug.LogError("GridManager notFound");
                return;
            }

            _gridManager = gridManager;
            _slinkyColor = SlinkyColorUtility.GetColor(color);

            transform.position = startPos;
            _segments = new List<Transform>();
            _isSelected = false;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 segmentPosition = CalculateSegmentPosition(startPos, endPos, i, segmentCount);
                GameObject segment = Instantiate(_segmentPrefab, segmentPosition, Quaternion.identity, transform);
                segment.GetComponent<Renderer>().material.color = _slinkyColor;
                _segments.Add(segment.transform);
            }
        }

        private Vector3 CalculateSegmentPosition(Vector3 startPos, Vector3 endPos, int index, int totalSegments)
        {
            float t = (float)index / (totalSegments - 1);
            Vector3 linearPosition = Vector3.Lerp(startPos, endPos, t);
            float parabola = Mathf.Sin(t * Mathf.PI) * 0.5f;
            Vector3 offset = new Vector3(0, parabola, 0);
            return linearPosition + offset;
        }

        public void OnSegmentClicked()
        {
            if (!_isSelected && !_isMoving)
            {
                GameKey emptySlotKey = _gridManager.GetFirstEmptySlot(true);

                if (emptySlotKey == null) 
                {
                    Debug.LogWarning("No available slot found for placing Slinky!");
                    return;
                }

                _isSelected = _gridManager.TryPlaceSlinky(emptySlotKey, new SlinkyData(0, 0, "Red"), true);
            }
        }

        public void MoveToTarget(Vector3 targetPosition)
        {
            if (_isMoving || _segments.Count == 0) return;
            _isMoving = true;

            Vector3 startPos = _segments[0].position;
            Vector3 midPoint = (startPos + targetPosition) / 2 + Vector3.up * (_springFactor * Vector3.Distance(startPos, targetPosition));

            for (int i = 0; i < _segments.Count; i++)
            {
                int index = i;
                float delay = i * _delayBetweenSegments;

                Ease[] movementEases = { Ease.InOutSine, Ease.OutQuad, Ease.OutBounce };
                Ease randomEase = movementEases[UnityEngine.Random.Range(0, movementEases.Length)];

                _segments[index].DOPath(new Vector3[] { midPoint, targetPosition }, _travelTime, PathType.CatmullRom)
                    .SetDelay(delay)
                    .SetEase(randomEase);

            }

            DOVirtual.DelayedCall(_travelTime + _delayBetweenSegments * _segments.Count, () =>
            {
                _isMoving = false;
                OnMovementComplete.Emit();
            });
        }

    }
}
