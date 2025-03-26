using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace GreatGames.CaseLib.Match
{
    public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
    {
        public bool Initialized { get; set; }
        private bool _isMatching = false;
        private MatchProcessor _matchProcessor;
        private GridManager _gridManager;

        [SerializeField] private float _mergeDelay = 0.1f;

        private void Start()
        {
            _gridManager = GridManager.Instance;
            _matchProcessor = new MatchProcessor(_gridManager, new ColorMatchRule(3));
        }

        public async void CheckForMatch()
        {
            if (_isMatching) return;

            _isMatching = true;
            _matchProcessor = new MatchProcessor(_gridManager, new ColorMatchRule(3), _mergeDelay);
            await _matchProcessor.CheckForMatchesAsync();
            _isMatching = false;

            GameManager.Instance.CheckForCompletion();
            GameManager.Instance.CheckGameState();
            CheckForMatch(); 
        }
    }
}
