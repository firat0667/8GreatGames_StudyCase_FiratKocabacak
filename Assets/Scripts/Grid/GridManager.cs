using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    public class GridManager : MonoBehaviour, IFoundationSingleton, IInitializable
    {
        [Header("Grid Configurations")]
        [SerializeField] private GridStructure _upperGrid;
        [SerializeField] private GridStructure _lowerGrid;
        [SerializeField] private GameObject _slotPrefab;

        [Header("Grid Offsets")] 
        [SerializeField] private Vector3 _slinkyGridOffset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 _mergeGridOffset = new Vector3(0, -1, 0);

        private Transform _gridParent;
        private Transform _slinkyParent;

        public BasicSignal OnGridUpdated { get; private set; }
        public bool Initialized { get; set; }

        public void Init()
        {
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrids(Vector2Int upperSize, Vector2Int lowerSize, Transform levelParent)
        {
            if (upperSize.x <= 0 || upperSize.y <= 0 || lowerSize.x <= 0 || lowerSize.y <= 0)
            {
                Debug.LogError("Grid size should be bigger than zero");
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

            _upperGrid ??= new GridStructure(upperSize, _slinkyGridOffset, _slotPrefab);
            _lowerGrid ??= new GridStructure(lowerSize, _mergeGridOffset, _slotPrefab);

            _upperGrid.InitializeGrid(_gridParent);
            _lowerGrid.InitializeGrid(_gridParent);

            OnGridUpdated?.Emit();
        }

        public bool TryPlaceSlinky(GameKey slotKey, SlinkyData slinky, bool isUpperGrid)
        {
            GridStructure targetGrid = isUpperGrid ? _upperGrid : _lowerGrid;

            if (!targetGrid.IsSlotEmpty(slotKey)) return false;

            if (targetGrid.TryGetSlot(slotKey, out GridData slot))
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
