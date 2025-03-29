using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
using System.Collections.Generic;
using UnityEngine;
using GreatGames.CaseLib.Definitions;
using static UnityEditor.Progress;

namespace GreatGames.CaseLib.Grid
{
    [System.Serializable]
    public class GridStructure : IContainer
    {
        public Vector3 Offset { get; private set; }
        public GameObject SlotPrefab { get; private set; }

        private Dictionary<GameKey, GridDataContainer> _slots = new();
        private Queue<GameKey> _emptySlots = new();
        private Dictionary<GameKey, GameObject> _slotObjects = new(); 

        public  Vector2Int Size=>_size;
        private Vector2Int _size;

        private bool _isUpperGrid;

        public BasicSignal OnGridUpdated { get; private set; }
        public object Value { get => this; set { } }
        public GridStructure(Vector2Int size, Vector3 offset, GameObject slotPrefab, bool isUpperGrid)
        {
            _size = size;
            Offset = offset;
            SlotPrefab = slotPrefab;
            OnGridUpdated = new BasicSignal();
            _isUpperGrid = isUpperGrid;
        }

        public void InitializeGrid(Vector2Int size, Transform parent)
        {
            _size = size;
            _slots.Clear();
            _slotObjects.Clear();

            string prefix = GameKeyExtensions.GetPrefix(_isUpperGrid);

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    int correctedX = !_isUpperGrid ? (size.x - 1 - x) : x;

                    GameKey slotKey = new GameKey($"{prefix}{correctedX},{y}");
                    Vector3 position = new Vector3(correctedX + Offset.x, Offset.y, -y + Offset.z);

                    GridDataContainer slotData = new GridDataContainer(correctedX + y * _size.x, position);
                    _slots[slotKey] = slotData;
                    _emptySlots.Enqueue(slotKey);

                    GameObject slotObject = Object.Instantiate(SlotPrefab, position, Quaternion.identity, parent);
                    slotObject.name = $"Grid_{prefix}{correctedX},{y}";
                    _slotObjects[slotKey] = slotObject;
                }
            }
        }


        public void ClearSlot(GameKey key)
        {;
            if (!_slots.ContainsKey(key)) return;

            _slots[key].SetOccupied(false);
            _slots[key].RemoveItem();

            if (!_emptySlots.Contains(key))
            {
                _emptySlots.Enqueue(key);
            }

            OnGridUpdated.Emit();
        }


        public bool IsSlotEmpty(GameKey key)
        {
            return _slots.ContainsKey(key) && !_slots[key].IsOccupied;
        }

        public bool TryGetContainer(GameKey key, out GridDataContainer container)
        {
            if (!_slots.TryGetValue(key, out container))
            {
                Debug.LogWarning($"Slot key {key.ValueAsString} not found!");
                return false;
            }
            return true;
        }
        public bool PlaceItem<T>(GameKey key, T item, bool force = false) where T : ISlotItem
        {
            if (!_slots.TryGetValue(key, out var container)) return false;

            if (!force && container.IsOccupied)
                return false;

            container.SetOccupied(true);
            container.SetItem(item);

            item.OccupiedGridKeys.Clear();
            item.OccupiedGridKeys.Add(key);
            item.SlotIndex = key;
            OnGridUpdated.Emit(); 

            return true;
        }
        public bool PlaceMultiSlotItem<T>(GameKey key, T item, bool force = false) where T : ISlotItem
        {
            if (!_slots.TryGetValue(key, out var container)) return false;

            if (!force && container.IsOccupied)
                return false;

            container.SetOccupied(true);
            container.SetItem(item);
            item.SlotIndex = key;
            OnGridUpdated.Emit();

            return true;
        }

        public void SetSlotType(GameKey key, SlotType type)
        {
            if (_slots.TryGetValue(key, out var container))
            {
                container.SetSlotType(type);
            }
        }


        public GameObject GetSlotObject(GameKey key)
        {
            if (_slotObjects.TryGetValue(key, out GameObject slotObject))
            {
                return slotObject;
            }

            return null;
        }

        public Vector3 GetWorldPosition(GameKey key)
        {
            if (!_slots.ContainsKey(key))
            {
                return Vector3.zero;
            }

            return _slots[key].Position;
        }

        public GameKey GetFirstEmptySlot()
        {
            while (_emptySlots.Count > 0)
            {
                GameKey key = _emptySlots.Peek();
                if (!_slots[key].IsOccupied)
                    return key;
                else
                    _emptySlots.Dequeue();
            }
            return null;
        }
        public GameKey CreateKeyFromIndex(int index)
        {
            int x = index % _size.x;
            int y = index / _size.x;

            int correctedX = !_isUpperGrid ? (_size.x - 1 - x) : x;
            string prefix = GameKeyExtensions.GetPrefix(_isUpperGrid);
            return new GameKey($"{prefix}{correctedX},{y}");
        }


        public Dictionary<GameKey, GridDataContainer> GetAllSlots()
        {
            return _slots;
        }


    }
}
