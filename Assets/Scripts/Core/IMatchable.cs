using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Utility;
using System.Collections.Generic;
using System.Linq;

public interface IMatchable
{
    GameKey SlotIndex { get; }
    ItemColor ItemColor { get; }
    bool IsMovable { get; }
    bool IsMarkedForMatch { get; set; }
    bool MatchesWith(IMatchable other);
    void OnMatchedAsTarget();
    void OnMatchedAsMover(GameKey targetSlot);
}
public interface IMatchRule
{
    bool IsMatch(List<IMatchable> sequence);
}
public class ColorMatchRule : IMatchRule
{
    private int _requiredCount;

    public ColorMatchRule(int requiredCount = 3)
    {
        _requiredCount = requiredCount;
    }

    public bool IsMatch(List<IMatchable> sequence)
    {
        return GetMatches(sequence).Any();
    }
    public List<List<IMatchable>> GetMatches(List<IMatchable> sequence)
    {
        List<List<IMatchable>> result = new();
        int i = 0;

        while (i <= sequence.Count - _requiredCount)
        {
            var currentColor = sequence[i].ItemColor;
            List<IMatchable> temp = new() { sequence[i] };

            int j = i + 1;
            while (j < sequence.Count && sequence[j].ItemColor == currentColor)
            {
                temp.Add(sequence[j]);
                j++;
            }

            if (temp.Count >= _requiredCount)
                result.Add(temp);

            i = j;
        }

        return result;
    }
}
public static class MatchableExtensions
{
    public static bool TryFindMatch(this List<IMatchable> sequence, IMatchable target, out IMatchable matchedWith)
    {
        matchedWith = null;

        foreach (var item in sequence)
        {
            if (item == target) continue;

            if (target.MatchesWith(item))
            {
                matchedWith = item;
                return true;
            }
        }

        return false;
    }
}

//List<IMatchable> matchableList = ...;
//IMatchable myItem = ...;

//if (matchableList.TryFindMatch(myItem, out var matchedItem))
//{
//    // matched 
//    Debug.Log($"Match found with: {matchedItem.SlotIndex}");
//}
//else
//{
//    // not match
//}
