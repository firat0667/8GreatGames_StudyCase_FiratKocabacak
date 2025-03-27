using GreatGames.CaseLib.Utility;
using System.Collections.Generic;


[System.Serializable]
public class BusData
{
    public List<int> Slots = new List<int>();
    public List<ItemColor> Colors = new List<ItemColor>();
}

[System.Serializable]
public class DoorData
{
    public int SlotIndex;
    public List<ItemColor> IncomingColors;
    public List<int> IncomingCounts;
}
[System.Serializable]
public class BlockData
{
    public int SlotIndex;
}