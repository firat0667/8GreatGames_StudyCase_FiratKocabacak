using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Slinky;
using GreatGames.CaseLib.Utility;
using UnityEngine;

namespace GreatGames.CaseLib.Grid
{
    public class GridDataContainer : IContainer
    {
        public int Index { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOccupied { get; private set; }
        public BasicSignal OnSlotStateChanged { get; private set; }

        public SlinkyController Slinky { get; private set; }

        public bool HasSlinky => Slinky != null;

        public GridDataContainer(int index, Vector3 position)
        {
            Index = index;
            Position = position;
            IsOccupied = false;
            OnSlotStateChanged = new BasicSignal();
        }

        public void SetOccupied(bool status)
        {
            if (IsOccupied == status) return; 

            IsOccupied = status;
            OnSlotStateChanged?.Emit(); 
        }
        public void Clear()
        {
            Slinky = null;
            IsOccupied = false;
            OnSlotStateChanged?.Emit();
        }

        public void CopyFrom(GridDataContainer other)
        {
            if (other == null || (Index == other.Index && Position == other.Position && IsOccupied == other.IsOccupied))
                return; 

            Index = other.Index;
            Position = other.Position;
            IsOccupied = other.IsOccupied;
        }
        public void SetSlinky(SlinkyController slinky)
        {
            Slinky = slinky;
            IsOccupied = slinky != null;
            OnSlotStateChanged?.Emit();
        }

        public void RemoveSlinky()
        {
            Slinky = null;
            IsOccupied = false;
            OnSlotStateChanged?.Emit();
        }
        public object Value
        {
            get => this;
            set
            {
                if (value is GridDataContainer data)
                    CopyFrom(data);
            }
        }
    }

    [System.Serializable]
    public class SlinkyData : IContainer
    {
        public int StartSlot;
        public int EndSlot;
        public SlinkyColor Color;

        public SlinkyData(int startSlot, int endSlot, SlinkyColor color)
        {
            StartSlot = startSlot;
            EndSlot = endSlot;
            Color = color;
        }

        public void CopyFrom(SlinkyData other)
        {
            if (other == null || (StartSlot == other.StartSlot && EndSlot == other.EndSlot && Color == other.Color))
                return;

        }
        public object Value
        {
            get => this;
            set
            {
                if (value is SlinkyData data)
                    CopyFrom(data);
            }
        }
    }
    public enum GridType
    {
        Upper,
        Lower
    }
}
