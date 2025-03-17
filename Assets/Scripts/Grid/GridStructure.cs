using System.Collections.Generic;
using UnityEngine;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Key;

namespace GreatGames.CaseLib.Grid
{
    [System.Serializable]
    public class GridStructure : IContainer
    {
        public Vector3 Offset { get; private set; }
        public GameObject SlotPrefab { get; private set; }
        private Dictionary<GameKey, GridData> _slots = new Dictionary<GameKey, GridData>();
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

        public void InitializeGrid(Transform parent)
        {
            _slots.Clear();

            for (int y = 0; y < _size.y; y++)
            {
                for (int x = 0; x < _size.x; x++)
                {
                    GameKey key = new GameKey($"Slot_{x}_{y}");
                    Vector3 position = new Vector3(x + Offset.x, Offset.y, -y + Offset.z);
                    _slots[key] = new GridData(key.Value, position); 

                    if (SlotPrefab != null)
                    {
                        GameObject slot = Object.Instantiate(SlotPrefab, position, Quaternion.identity, parent);
                        slot.name = key.ValueAsString; 
                    }
                }
            }

            OnGridUpdated.Emit(); 
        }

        public bool IsSlotEmpty(GameKey key)
        {
            return _slots.ContainsKey(key) && _slots[key].IsOccupied == false;
        }

        public bool TryGetSlot(GameKey key, out GridData slot)
        {
            return _slots.TryGetValue(key, out slot);
        }

        public Vector3 GetWorldPosition(GameKey key)
        {
            if (!_slots.ContainsKey(key))
            {
                Debug.LogWarning($"GridStructure: Slot key {key.ValueAsString} notFound!");
                return Vector3.zero;
            }

            return _slots[key].Position;
        }
    }
}
