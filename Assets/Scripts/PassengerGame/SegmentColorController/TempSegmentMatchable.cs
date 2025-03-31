using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;

public class TempSegmentMatchable : IMatchable
{
    public GameKey SlotIndex { get; }
    public ItemColor ItemColor { get; }
    public bool IsMovable => false;
    public bool IsMarkedForMatch { get; set; }

    public TempSegmentMatchable(GameKey slotIndex, ItemColor color)
    {
        SlotIndex = slotIndex;
        ItemColor = color;
    }

    public bool MatchesWith(IMatchable other)
    {
        return other.ItemColor == ItemColor;
    }

    public void OnMatchedAsTarget() { }
    public void OnMatchedAsMover(GameKey targetSlot) { }
}
