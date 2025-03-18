using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
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

        private Queue<GameKey> _emptySlots = new Queue<GameKey>();

        private Dictionary<GameKey, GridDataContainer> _slots = new Dictionary<GameKey, GridDataContainer>();
        private Vector2Int _size;

        public int SlotCount => _size.x * _size.y;
        public BasicSignal OnGridUpdated { get; private set; }

        public object Value { get => this; set { } }

        public GridStructure(Vector2Int size, Vector3 offset, GameObject slotPrefab)
        {
            _size = size;
            Offset = offset;
            SlotPrefab = slotPrefab;
            OnGridUpdated = new BasicSignal();
        }

        public void InitializeGrid(Vector2Int size, Transform parent)
        {
            _size = size;
            _slots.Clear();

            if (SlotPrefab == null)
            {
                Debug.LogError("ERROR: GridConfig -> slotPrefab is NULL!");
                return;
            }

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    GameKey slotKey = new GameKey($"{x},{y}");

                    // ✅ Doğru pozisyon hesapla!
                    Vector3 position = new Vector3(x + Offset.x, Offset.y, -y + Offset.z);
                    _slots[slotKey] = new GridDataContainer(x + y * _size.x, position);

                    GameObject slot = Object.Instantiate(SlotPrefab, position, Quaternion.identity, parent);
                    slot.name = $"Slot_{x}_{y}";

                    // 🔍 Log ekleyerek hangi slotların oluşturulduğunu kontrol et!
                    Debug.Log($"[GRID INIT] Created Slot {slot.name} at {position} with Key: {slotKey.ValueAsString}");
                }
            }
        }

        public void SetSlotOccupied(GameKey key, bool occupied)
        {
            if (!_slots.ContainsKey(key)) return;

            _slots[key].SetOccupied(occupied);

            if (occupied)
            {
                if (_emptySlots.Contains(key))
                    _emptySlots = new Queue<GameKey>(_emptySlots.Where(k => k != key));
            }
            else
            {
                if (!_emptySlots.Contains(key))
                    _emptySlots.Enqueue(key);
            }

            OnGridUpdated.Emit();
        }


        public bool IsSlotEmpty(GameKey key)
        {
            return _slots.ContainsKey(key) && _slots[key].IsOccupied == false;
        }

        public bool TryGetSlot(GameKey key, out GridDataContainer slot)
        {
            if (!_slots.TryGetValue(key, out slot))
            {
                Debug.LogWarning($"Slot key {key.ValueAsString} not found!");
                return false;
            }
            return true;
        }

        public Vector3 GetWorldPosition(GameKey key)
        {
            if (!_slots.ContainsKey(key))
            {
                Debug.LogError($"❌ [ERROR] GridStructure: Slot key {key.ValueAsString} bulunamadı!");
                return Vector3.zero;
            }

            Debug.Log($"✅ Slot key {key.ValueAsString} bulundu, Pozisyon: {_slots[key].Position}");
            return _slots[key].Position;

        }



        public GameKey GetFirstEmptySlot()
        {
            while (_emptySlots.Count > 0)
            {
                GameKey key = _emptySlots.Peek();
                if (_slots[key].IsOccupied == false)
                    return key;
                else
                    _emptySlots.Dequeue();
            }
            return null;
        }
    }
}
