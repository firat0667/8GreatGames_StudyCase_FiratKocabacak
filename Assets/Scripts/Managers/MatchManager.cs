using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Patterns;
using System.Linq;
using UnityEngine;
namespace GreatGames.CaseLib.Match
{
    public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
    {
        public bool Initialized { get; set; }
        private bool _isMatching = false;
        private MatchProcessor _matchProcessor;
        private GridManager _gridManager;
        [SerializeField] private int _matchCount = 3;

        [SerializeField] private float _mergeDelay = 0.1f;

        private IAdvancedMatchRule _matchRule;

        private void Start()
        {
            _gridManager = GridManager.Instance;
             _matchRule = new ColorMatchRule(_matchCount);
            _matchProcessor = new MatchProcessor(_gridManager, _matchRule, _mergeDelay);

        }

        public async void CheckForMatch()
        {
            if (_isMatching) return;

            _isMatching = true;
             _matchRule = new ColorMatchRule(_matchCount);
            _matchProcessor = new MatchProcessor(_gridManager, _matchRule, _mergeDelay);

            await _matchProcessor.CheckForMatchesAsync();
            _isMatching = false;
            GameManager.Instance.CheckForCompletion();
            GameManager.Instance.CheckGameState();
        }
        public bool HasAnyMatchAvailable()
        {
            var allItems = GridManager.Instance.GetAllItemsInLowerGrid()
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
                        return true;
                }
                else
                {
                    if (_matchRule.IsMatch(matchables))
                        return true;
                }
            }

            return false;
        }
    }
}
