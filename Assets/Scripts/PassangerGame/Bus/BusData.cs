using GreatGames.CaseLib.Utility;
using System.Collections.Generic;


[System.Serializable]
public class BusData : ISlotData
{
    public List<int> Slots;
    public List<ItemColor> Colors;

    int ISlotData.SlotIndex => Slots != null && Slots.Count > 0 ? Slots[0] : -1;
}
public enum DoorExitDirection { Up, Down, Left, Right }

[System.Serializable]
public class DoorData : ISlotData
{
    public int SlotIndex;
    public List<ItemColor> IncomingColors;
    public List<int> IncomingCounts;
    public DoorExitDirection EnterDirection = DoorExitDirection.Up;

    int ISlotData.SlotIndex => this.SlotIndex;
}
[System.Serializable]
public class BlockData : ISlotData
{
    public int SlotIndex;
    int ISlotData.SlotIndex => this.SlotIndex;
}
public interface ISlotData
{
    int SlotIndex { get; }
}