using UnityEngine;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Signals;

namespace GreatGames.CaseLib.Grid
{
    public class GridData : IContainer
    {
        public int Index { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOccupied { get; private set; }
        public BasicSignal OnSlotStateChanged { get; private set; }

        public GridData(int index, Vector3 position)
        {
            Index = index;
            Position = position;
            IsOccupied = false;
            OnSlotStateChanged = new BasicSignal();
        }

        public object Value
        {
            get => this;
            set
            {
                if (value is GridData data)
                {
                    Index = data.Index;
                    Position = data.Position;
                    IsOccupied = data.IsOccupied;
                }
            }
        }

        public void SetOccupied(bool status)
        {
            IsOccupied = status;
            OnSlotStateChanged.Emit();
        }
    }

    [System.Serializable]
    public class SlinkyData : IContainer
    {
        public int StartSlot { get; private set; }
        public int EndSlot { get; private set; }
        public string Color { get; private set; }

        public SlinkyData(int startSlot, int endSlot, string color)
        {
            StartSlot = startSlot;
            EndSlot = endSlot;
            Color = color;
        }

        public object Value
        {
            get => this;
            set
            {
                if (value is SlinkyData data)
                {
                    StartSlot = data.StartSlot;
                    EndSlot = data.EndSlot;
                    Color = data.Color;
                }
            }
        }
    }
}
