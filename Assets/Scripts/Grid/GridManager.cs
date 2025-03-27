using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace GreatGames.CaseLib.Grid
{
    public partial class GridManager : FoundationSingleton<GridManager>, IFoundationSingleton, IInitializable
    {
        private LowerGridSlotHandler _slotHandler;
        public BasicSignal OnGridUpdated { get; private set; } = new BasicSignal();

        private GridBuilder _gridBuilder;
        private GridStructure _upperGrid;
        private GridStructure _lowerGrid;

        [SerializeField] private GameObject _gridPrefab;

        public LevelConfigSO LevelData => _levelData;
        [SerializeField] private LevelConfigSO _levelData;

        public GridStructure LowerGrid => _lowerGrid;
        public GridStructure UpperGrid => _upperGrid;

        private readonly List<ISlotItem> _slotItems = new();
        public IReadOnlyList<ISlotItem> AllSlotItems => _slotItems;

        public bool Initialized { get; set; }

        [SerializeField] private Vector3 _upperGridOffset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 _lowerGridOffset = new Vector3(0, -1.5f, 0);


        private void Awake()
        {
            _slotHandler = new LowerGridSlotHandler(_lowerGrid);
        }

        public void InitializeGrids(LevelConfigSO levelData, Transform levelParent)
        {
            _levelData = levelData;
            _gridBuilder = new GridBuilder();

            (_upperGrid, _lowerGrid) = _gridBuilder.Build(levelData, _gridPrefab, levelParent, _upperGridOffset, _lowerGridOffset);
            _slotHandler = new LowerGridSlotHandler(_lowerGrid);

            OnGridUpdated?.Emit();
        }


        public bool IsLowerGridFull()
        {
            return _lowerGrid.GetAllSlots().All(slot => slot.Value.IsOccupied);
        }

        public GameKey GetGridKeyFromPosition(Vector3 position)
        {
            foreach (var kvp in _upperGrid.GetAllSlots())
            {
                if (Vector3.Distance(kvp.Value.Position, position) < 0.1f)
                    return kvp.Key;
            }

            foreach (var kvp in _lowerGrid.GetAllSlots())
            {
                if (Vector3.Distance(kvp.Value.Position, position) < 0.1f)
                    return kvp.Key;
            }

            return null;
        }

        public GameKey GetFirstColorEmptySlotOrNextToMatch(ISlotItem item)
        {
                return _slotHandler.GetBestSlotFor(item);

        }



        public Vector3 GetSlotPosition(GameKey key, bool isUpperGrid)
        {
            return isUpperGrid ? _upperGrid.GetWorldPosition(key) : _lowerGrid.GetWorldPosition(key);
        }

        public bool IsSlotOccupied(GameKey key)
        {
            if (key == null) return false;

            if (key.IsUpper())
                return _upperGrid.TryGetContainer(key, out var slot) && slot.IsOccupied;

            return _lowerGrid.TryGetContainer(key, out var slot2) && slot2.IsOccupied;
        }

        public void RemoveItemAt(GameKey key)
        {
            if (key.IsLower())
            {
                if (_lowerGrid.TryGetContainer(key, out var slot) && slot.HasItem)
                {
                    slot.RemoveItem();
                }
            }
            else if (key.IsUpper())
            {
                if (_upperGrid.TryGetContainer(key, out var slot) && slot.HasItem)
                {
                    slot.RemoveItem();
                }
            }
        }
        public GameKey GetFirstEmptySlotOnLeftOf(int currentX, bool isUpperGrid)
        {
            var grid = isUpperGrid ? _upperGrid : _lowerGrid;

            return grid.GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied && kvp.Key.ToVector2Int().x > currentX)
                .OrderByDescending(kvp => kvp.Key.ToVector2Int().x)
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }

        public List<GameKey> GetEmptyLowerGridSlots()
        {
            return _lowerGrid.GetAllSlots()
                .Where(kvp => !kvp.Value.IsOccupied)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public GameKey GetFirstEmptySlot(bool isUpperGrid)
        {
            var grid = isUpperGrid ? _upperGrid : _lowerGrid;
            return grid.GetAllSlots()
                       .Where(s => !s.Value.IsOccupied)
                       .OrderByDescending(s => s.Key.ToVector2Int().x)
                       .Select(s => s.Key)
                       .FirstOrDefault();
        }

        public bool PlaceItem<T>(GameKey key, T item, bool isUpperGrid, bool force = false) where T : ISlotItem
        {
            var grid = isUpperGrid ? _upperGrid : _lowerGrid;
            return grid.PlaceItem(key, item, force);
        }

        public void RemoveItem(ISlotItem item)
        {
            _slotItems.Remove(item);
        }

        public void RegisterItem(ISlotItem item)
        {
            if (!_slotItems.Contains(item))
                _slotItems.Add(item);
        }
        public List<ISlotItem> GetAllItemsInLowerGrid()
        {
            var result = _slotItems
                .Where(s => s.OccupiedGridKeys.Count > 0 && s.OccupiedGridKeys.All(k => k != null && k.IsLower()))
                .ToList();


            return result;
        }

        public void UpdateItemSlot(ISlotItem item, GameKey newKey)
        {
            foreach (var key in item.OccupiedGridKeys)
            {
                if (key.IsLower())
                    _lowerGrid.ClearSlot(key);
                else
                    _upperGrid.ClearSlot(key);
            }
            item.OccupiedGridKeys.Clear();
            item.OccupiedGridKeys.Add(newKey);

            if (newKey.IsLower())
                _lowerGrid.PlaceItem(newKey, item);
            else
                _upperGrid.PlaceItem(newKey, item);

            OnGridUpdated?.Emit();
        }
        public void ClearAll()
        {
            foreach (var item in _slotItems)
            {
                if (item.Root != null)
                Destroy(item.Root);
            }
            _slotItems.Clear();
        }
        public List<ISlotItem> GetAllItems()
        {
            return _slotItems;
        }

    }
}
