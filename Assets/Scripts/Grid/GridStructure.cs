using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    [System.Serializable]
    public class GridStructure : IContainer
    {
        public Vector3 Offset { get; private set; }
        public GameObject SlotPrefab { get; private set; }

        private Dictionary<GameKey, GridDataContainer> _slots = new();
        private Queue<GameKey> _emptySlots = new();
        private Dictionary<GameKey, GameObject> _slotObjects = new(); // ✅ Slot objelerini saklar

        private Vector2Int _size;
        private bool _isUpperGrid;

        public int SlotCount => _size.x * _size.y;
        public BasicSignal OnGridUpdated { get; private set; }
        public object Value { get => this; set { } }
        public GridType Type => _isUpperGrid ? GridType.Upper : GridType.Lower;

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

            string prefix = _isUpperGrid ? "U_" : "L_";

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

        public void SetSlotOccupied(GameKey key, SlinkyController slinky)
        {
            if (!_slots.ContainsKey(key)) return;

            _slots[key].SetOccupied(true);
            _slots[key].SetSlinky(slinky);

            OnGridUpdated.Emit();
        }

        public void ClearSlot(GameKey key)
        {
            if (!_slots.ContainsKey(key)) return;

            _slots[key].SetOccupied(false);
            _slots[key].RemoveSlinky();

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

        public Dictionary<GameKey, GridDataContainer> GetAllSlots()
        {
            return _slots;
        }

        public void RemoveSlinky(GameKey key)
        {
            if (_slots.TryGetValue(key, out GridDataContainer slot))
            {
                slot.RemoveSlinky();
            }
        }

        public void PlaceSlinky(SlinkyController slinky, GameKey key)
        {
            if (_slots.ContainsKey(key))
            {
                _slots[key].SetSlinky(slinky); 
            }
        }
    }
}
