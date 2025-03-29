using GreatGames.CaseLib.Definitions;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Signals;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    public class GridDataContainer : IContainer
    {
        public int Index { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOccupied { get; private set; }
        public BasicSignal OnSlotStateChanged { get; private set; }

        public ISlotItem Item { get; private set; }
        public bool HasItem => Item != null; 

        public object Value { get; set; }
        private SlotType _slotType;
        public GridDataContainer(int index, Vector3 position)
        {
            Index = index;
            Position = position;
            IsOccupied = false;
            OnSlotStateChanged = new BasicSignal();
        }
        public void SetItem(ISlotItem item)
        {
            Item = item;
            IsOccupied = item != null;
            OnSlotStateChanged?.Emit();
        }

        public void RemoveItem()
        {
            Item = null;
            IsOccupied = false;
            OnSlotStateChanged?.Emit();
        }

        public void SetOccupied(bool status)
        {
            if (IsOccupied == status) return;
            IsOccupied = status;
            OnSlotStateChanged?.Emit();
        }
        public bool TryGetItem(out ISlotItem item)
        {
            item = Item;
            return item != null;
        }
        public void SetSlotType(SlotType type)
        {
            _slotType = type;
        }

        public SlotType GetSlotType()
        {
            return _slotType;
        }

    }
}
