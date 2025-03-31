using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;


[System.Serializable]
public class BusData : ISlotData
{
    public List<int> Slots;
    public List<ItemColor> Colors;
    public List<Direction> Directions;
    int ISlotData.SlotIndex => Slots != null && Slots.Count > 0 ? Slots[0] : -1;
}
public class BusSeatInfo
{
    public GameKey SlotKey;          
    public ItemColor Color;          
    public int Capacity;           
    public int Occupied;            
    public bool IsFull => Occupied >= Capacity;
   
}

public enum Direction { Up, Down, Left, Right,
    None
}

[System.Serializable]
public class DoorData : ISlotData
{
    public int SlotIndex;
    public List<ItemColor> IncomingColors;
    public List<int> IncomingCounts;
    public Direction EnterDirection = Direction.Up;
    public UnityEngine.GameObject PathLineObject;
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