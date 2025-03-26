using GreatGames.CaseLib.Utility;
using System.ComponentModel;

namespace GreatGames.CaseLib.Slinky
{
    [System.Serializable]
    public class SlinkyData : IContainer
    {
        public int StartSlot;
        public int EndSlot;
        public ItemColor Color;

        public SlinkyData(int startSlot, int endSlot, ItemColor color)
        {
            StartSlot = startSlot;
            EndSlot = endSlot;
            Color = color;
        }

        public void CopyFrom(SlinkyData other)
        {
            if (other == null ||
                (StartSlot == other.StartSlot && EndSlot == other.EndSlot && Color == other.Color))
                return;

            StartSlot = other.StartSlot;
            EndSlot = other.EndSlot;
            Color = other.Color;
        }

        public void Add(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IComponent component, string name)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(IComponent component)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
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

        public ComponentCollection Components => throw new System.NotImplementedException();
    }
}