using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
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

        private List<SlinkyController> _slinkies = new List<SlinkyController>();

        private void OnEnable()
        {
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrids(LevelConfigSO levelData, Transform levelParent)
        {
            _levelData = levelData;

            if (_levelData == null)
            {
                return;
            }

            if (_gridParent == null)
            {
                _gridParent = new GameObject("Grids Parent").transform;
                _gridParent.SetParent(levelParent);
            }

            if (_slinkyParent == null)
            {
                _slinkyParent = new GameObject("Slinkies Parent").transform;
                _slinkyParent.SetParent(levelParent);
            }

            if (_upperGrid == null)
            {
                _upperGrid = new GridStructure(_levelData.UpperGridSize, _slinkyGridOffset, _gridPrefab, true);
            }

            if (_lowerGrid == null)
            {
                _lowerGrid = new GridStructure(_levelData.LowerGridSize, _mergeGridOffset, _gridPrefab, false);
            }


            _upperGrid.InitializeGrid(_levelData.UpperGridSize, _gridParent);
            _lowerGrid.InitializeGrid(_levelData.LowerGridSize, _gridParent);

            OnGridUpdated?.Emit();

            SpawnSlinkies();
        }
        private void SpawnSlinkies()
        {
            if (_levelData == null || _slinkyPrefab == null)
            {
                return;
            }

            foreach (var slinkyData in _levelData.Slinkies)
            {
                GridStructure startGrid = _upperGrid;
                GridStructure endGrid = _upperGrid;

                GameKey startKey = new GameKey($"U_{slinkyData.StartSlot % _levelData.UpperGridSize.x},{slinkyData.StartSlot / _levelData.UpperGridSize.x}");
                GameKey endKey = new GameKey($"U_{slinkyData.EndSlot % _levelData.UpperGridSize.x},{slinkyData.EndSlot / _levelData.UpperGridSize.x}");

                Vector3 startPos = startGrid.GetWorldPosition(startKey);
                Vector3 endPos = endGrid.GetWorldPosition(endKey);

                GameObject startSlotObject = startGrid.GetSlotObject(startKey);
                GameObject endSlotObject = endGrid.GetSlotObject(endKey);

                GameObject slinkyContainer = new GameObject($"Slinky_{startKey.ValueAsString}");
                slinkyContainer.transform.SetParent(_slinkyParent);
                slinkyContainer.transform.position = startPos;

                SlinkyController slinky = Instantiate(_slinkyPrefab, startPos, Quaternion.identity);
                slinky.transform.SetParent(slinkyContainer.transform);
                slinky.Initialize(startPos, endPos, this, slinkyData.Color, startSlotObject, endSlotObject, slinkyContainer.transform);

                startGrid.SetSlotOccupied(startKey, true);
                endGrid.SetSlotOccupied(endKey, true);
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

        public void RegisterSlinky(SlinkyController slinky)
        {
            _slinkies.Add(slinky);
        }
        public void RemoveSlinky(SlinkyController slinky)
        {
            _slinkies.Remove(slinky);
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

 
        public bool IsThereLongerSlinkyBlocking(SlinkyController slinky)
        {
            foreach (var segment in slinky.Segments)
            {
                Vector3 startPos = segment.position;
                Vector3 direction = Vector3.up;
                float distance = 2f;

                RaycastHit[] hits = Physics.RaycastAll(startPos, direction, distance);
                foreach (var hit in hits)
                {
                    SlinkyController collidingSlinky = hit.collider.GetComponentInParent<SlinkyController>();

                    if (collidingSlinky == null || collidingSlinky == slinky) continue; 

                    return true; 
                }
            }
            return false;
        }

        public GameKey GetGridKeyFromPosition(Vector3 position)
        {
            foreach (var kvp in _upperGrid.GetAllSlots())
            {
                if (Vector3.Distance(kvp.Value.Position, position) < 0.1f)
                {
                    return kvp.Key;
                }
            }

            foreach (var kvp in _lowerGrid.GetAllSlots())
            {
                if (Vector3.Distance(kvp.Value.Position, position) < 0.1f)
                {
                    return kvp.Key;  
                }
            }

            return null;
        }
        private bool IsPointBetween(Vector3 point, Vector3 start, Vector3 end)
        {
            float minX = Mathf.Min(start.x, end.x);
            float maxX = Mathf.Max(start.x, end.x);
            float minZ = Mathf.Min(start.z, end.z);
            float maxZ = Mathf.Max(start.z, end.z);

            return (point.x >= minX && point.x <= maxX) && (point.z >= minZ && point.z <= maxZ);
        }

    }

}
