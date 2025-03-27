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
        private IAdvancedMatchRule _matchRule;
        private readonly SlinkyMover _slinkyMover;
        private readonly float _mergeDelay;

        public MatchProcessor(GridManager gridManager, IAdvancedMatchRule matchRule, float mergeDelay = 0.2f)
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
                 .GroupBy(item => Mathf.RoundToInt(item.SlotIndex.ToVector2Int().y))
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.SlotIndex.ToVector2Int().x).ToList());

            foreach (var row in groupedByRow)
            {
                var matchables = row.Value;

                if (_matchRule is IAdvancedMatchRule advancedRule)
                {
                    var matches = advancedRule.GetMatches(matchables);
                    if (matches.Count > 0)
                    {
                        foreach (var group in matches)
                        {
                            await HandleMatchGroupAsync(group);
                        }
                    }
                }
                else
                {
                    if (_matchRule.IsMatch(matchables))
                    {
                        await HandleMatchGroupAsync(matchables);
                    }
                }
            }
        }
        private async Task HandleMatchGroupAsync(List<IMatchable> matchGroup)
        {
            var middle = matchGroup[matchGroup.Count / 2];
            middle.OnMatchedAsTarget();

            var movers = matchGroup.Where(m => m != middle).ToList();

            foreach (var mover in movers)
            {
                if (mover is not SlinkyController slinky) continue;

                var tcs = new TaskCompletionSource<bool>();

                void OnMoveComplete()
                {
                    slinky.OnMovementComplete.Disconnect(OnMoveComplete);
                    tcs.TrySetResult(true);
                }

                slinky.OnMovementComplete.Connect(OnMoveComplete);
                mover.OnMatchedAsMover(middle.SlotIndex);

                await tcs.Task; 
            }

            if (middle is SlinkyController middleSlinky)
                middleSlinky.PlayMergeEffect();

            foreach (var item in matchGroup)
            {
                if (item is SlinkyController slinky)
                {
                    slinky.RemoveFromGrid();
                    slinky.DestroySegments();
                    slinky.gameObject.SetActive(false);
                }
            }

            await Awaiters.DelaySeconds(_mergeDelay);
            _slinkyMover.ShiftRemainingSlinkies();
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
}
