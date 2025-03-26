using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LowerGridSlotHandler
{
    private readonly GridStructure _lowerGrid;

    public LowerGridSlotHandler(GridStructure grid)
    {
        _lowerGrid = grid;
    }

    public GameKey GetBestSlotFor(ISlotItem item)
    {
        var slots = _lowerGrid.GetAllSlots()
            .OrderBy(kvp => kvp.Key.ToVector2Int().x)
            .ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            var currentSlot = slots[i];

            if (!currentSlot.Value.HasItem)
                continue;

            if (currentSlot.Value.TryGetItem(out var existingItem) && existingItem.MatchesWith(item))
            {
                int rightIndex = i - 1;

                if (rightIndex>=0)
                {
                    var rightSlot = slots[rightIndex];
                    if(rightSlot.Value.HasItem)
                    {
                        if (item is SlinkyController slinky)
                        {
                            var mover = new SlinkyMover(_lowerGrid, GridManager.Instance);
                            bool shifted = mover.ShiftUntilFit(rightSlot.Key, slinky);

                            if (shifted)
                            {
                                return rightSlot.Key;
                            }
                        }
                    }

                    else
                    {
                        return rightSlot.Key;
                    }

                }
            }
        }
        return GetIdealReverseSlot(item);
    }

    public GameKey GetIdealReverseSlot(ISlotItem item)
    {
        var slots = _lowerGrid.GetAllSlots()
            .OrderByDescending(kvp => kvp.Key.ToVector2Int().x)
            .ToList();
        return GetFirstEmpty(slots);
    }

    private GameKey GetFirstEmpty(List<KeyValuePair<GameKey, GridDataContainer>> slots)
    {
        return slots.FirstOrDefault(s => !s.Value.HasItem).Key;
    }
}
