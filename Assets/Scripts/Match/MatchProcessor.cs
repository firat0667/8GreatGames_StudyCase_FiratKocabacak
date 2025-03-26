using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

namespace GreatGames.CaseLib.Match
{
    public class MatchProcessor
    {
        private readonly GridManager _gridManager;
        private readonly IMatchRule _matchRule;
        private readonly SlinkyMover _slinkyMover;
        private readonly float _mergeDelay;

        public MatchProcessor(GridManager gridManager, IMatchRule matchRule, float mergeDelay = 0.2f)
        {
            _gridManager = gridManager;
            _matchRule = matchRule;
            _slinkyMover = new SlinkyMover(_gridManager.LowerGrid, _gridManager);
            _mergeDelay = mergeDelay;
        }

        public async Task CheckForMatchesAsync()
        {
            var allItems = _gridManager.GetAllItemsInLowerGrid()
                .OfType<IMatchable>()
                .Where(item => item != null && item.SlotIndex != null)
                .ToList();


            var groupedByRow = allItems
                .Where(item => item.SlotIndex != null) 
                .GroupBy(item => item.SlotIndex.ToVector2Int().y)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.SlotIndex.ToVector2Int().x).ToList());


            foreach (var row in groupedByRow)
            {
                var matchables = row.Value;

                if (_matchRule is IAdvancedMatchRule advancedRule)
                {
                    var matches = advancedRule.GetMatches(matchables);
                    if (matches.Count > 0)
                    {
                        Debug.Log($"{matches.Count} MATCH GROUPS IN ROW {row.Key}");

                        foreach (var group in matches)
                        {
                            await HandleMatchGroupAsync(group);
                        }
                    }
                    else
                    {
                        Debug.Log($" NO MATCH in row {row.Key}");
                    }
                }
                else
                {
                    if (_matchRule.IsMatch(matchables))
                    {
                        Debug.Log($"MATCH FOUND IN ROW {row.Key}");
                        await HandleMatchGroupAsync(matchables);
                    }
                    else
                    {
                        Debug.Log($"NO MATCH in row {row.Key}");
                    }
                }
            }
        }

        private async Task HandleMatchGroupAsync(List<IMatchable> matchGroup)
        {
            var middle = matchGroup[matchGroup.Count / 2];
            middle.OnMatchedAsTarget();

            foreach (var mover in matchGroup.Where(m => m != middle))
            {
                mover.OnMatchedAsMover(middle.SlotIndex);
                await Awaiters.Until(() => !mover.IsMarkedForMatch);
            }

            if (middle is SlinkyController middleSlinky)
                middleSlinky.PlayMergeEffect();

            foreach (var item in matchGroup)
            {
                if (item is SlinkyController slinky)
                {
                    slinky.RemoveFromGrid();
                    slinky.DestroySegments();
                }
            }

            await Awaiters.DelaySeconds(_mergeDelay);
            _slinkyMover.ShiftRemainingSlinkies();
        }
    }

    public interface IAdvancedMatchRule : IMatchRule
    {
        List<List<IMatchable>> GetMatches(List<IMatchable> sequence);
    }

    public static class Awaiters
    {
        public static Task DelaySeconds(float seconds) => Task.Delay(System.TimeSpan.FromSeconds(seconds));

        public static async Task Until(System.Func<bool> condition)
        {
            while (!condition())
                await Task.Yield();
        }
    }
}
