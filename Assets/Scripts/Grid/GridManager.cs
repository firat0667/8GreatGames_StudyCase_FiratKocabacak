using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    public class GridManager : FoundationSingleton<GridManager>, IFoundationSingleton, IInitializable
    {
        [Header("Grid Configurations")]
        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private SlinkyController _slinkyPrefab; 

        [Header("Grid Offsets")]
        [SerializeField] private Vector3 _slinkyGridOffset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 _mergeGridOffset = new Vector3(0, -1, 0);

        private Transform _gridParent;
        private Transform _slinkyParent;

        private GridStructure _upperGrid;
        private GridStructure _lowerGrid;

        public BasicSignal OnGridUpdated { get; private set; }
        public bool Initialized { get; set; }

        private LevelConfigSO _levelData;

        private void OnEnable()
        {
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrids(LevelConfigSO levelData, Transform levelParent)
        {
            _levelData = levelData;

            if (_levelData == null)
            {
                Debug.LogError("ERROR: _levelData is NULL!");
                return;
            }

            if (_gridParent == null)
            {
                _gridParent = new GameObject("Grid Parent").transform;
                _gridParent.SetParent(levelParent);
            }

            if (_slinkyParent == null)
            {
                _slinkyParent = new GameObject("Slinky Parent").transform;
                _slinkyParent.SetParent(levelParent);
            }

            if (_upperGrid == null)
            {
                _upperGrid = new GridStructure(_levelData.UpperGridSize, _slinkyGridOffset, _gridPrefab);
            }

            if (_lowerGrid == null)
            {
                _lowerGrid = new GridStructure(_levelData.LowerGridSize, _mergeGridOffset, _gridPrefab);
            }

            _upperGrid.InitializeGrid(_levelData.UpperGridSize, _gridParent);
            _lowerGrid.InitializeGrid(_levelData.LowerGridSize, _gridParent);

            Debug.Log("Grids initialized successfully!");
            OnGridUpdated?.Emit();

            SpawnSlinkies();
        }
        private bool IsValidSlot(int slotIndex, bool isUpperGrid)
        {
            int maxSlotCount = isUpperGrid ? _upperGrid.SlotCount : _lowerGrid.SlotCount;
            return slotIndex >= 0 && slotIndex < maxSlotCount;
        }
        private void SpawnSlinkies()
        {
            if (_levelData == null || _slinkyPrefab == null)
            {
                Debug.LogError("ERROR: LevelData or SlinkyPrefab is NULL!");
                return;
            }

            foreach (var slinkyData in _levelData.Slinkies)
            {
                if (!IsValidSlot(slinkyData.StartSlot, true) || !IsValidSlot(slinkyData.EndSlot, true))
                {
                    Debug.LogError($"Invalid slot positions for slinky: {slinkyData.StartSlot}, {slinkyData.EndSlot}");
                    continue;
                }

                GameKey startKey = new GameKey(slinkyData.StartSlot.ToString());
                GameKey endKey = new GameKey(slinkyData.EndSlot.ToString());

                Vector3 startPos = _upperGrid.GetWorldPosition(startKey);
                Vector3 endPos = _upperGrid.GetWorldPosition(endKey);

                Debug.Log($"Spawning Slinky at -> Start: {startPos}, End: {endPos}");

                SlinkyController slinky = Instantiate(_slinkyPrefab, startPos, Quaternion.identity);
                slinky.transform.SetParent(_slinkyParent);
                slinky.Initialize(startPos, endPos, slinkyData.Color,this);
                _upperGrid.SetSlotOccupied(startKey, true);
                _upperGrid.SetSlotOccupied(endKey, true);
            }
        }


        public bool TryPlaceSlinky(GameKey slotKey, SlinkyData slinky, bool isUpperGrid)
        {
            GridStructure targetGrid = isUpperGrid ? _upperGrid : _lowerGrid;

            if (targetGrid.TryGetSlot(slotKey, out GridDataContainer slot) && slot.IsOccupied == false)
            {
                slot.SetOccupied(true);
                targetGrid.SetSlotOccupied(slotKey, true);
                OnGridUpdated?.Emit();
                return true;
            }

            return false;
        }

        public bool IsSlotEmpty(GameKey key, bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.IsSlotEmpty(key) : _lowerGrid.IsSlotEmpty(key);
        }

        public Vector3 GetSlotPosition(GameKey key, bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.GetWorldPosition(key) : _lowerGrid.GetWorldPosition(key);
        }

        public GameKey GetFirstEmptySlot(bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.GetFirstEmptySlot() : _lowerGrid.GetFirstEmptySlot();
        }
    }
}
