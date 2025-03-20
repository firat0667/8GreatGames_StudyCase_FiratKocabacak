using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
{
    private Queue<SlinkyController> _matchQueue = new Queue<SlinkyController>();
    private bool _isMatching = false;
    private SlinkyController _targetSlinky;

    public bool Initialized { get; set; }

    public bool CheckForMatch()
    {
        if (_isMatching) return true;

        List<SlinkyController> matchedSlinkies = FindMatch();
        if (matchedSlinkies.Count == 3)
        {
            _isMatching = true;
            _targetSlinky = matchedSlinkies[1];

            AnimateMatch(matchedSlinkies);
            return true;
        }
        return false;
    }

    private List<SlinkyController> FindMatch()
    {
        List<SlinkyController> slinkies = GridManager.Instance.GetAllSlinkiesInLowerGrid();

        for (int i = 0; i <= slinkies.Count - 3; i++)
        {
            if (slinkies[i].SlinkyColor == slinkies[i + 1].SlinkyColor &&
                slinkies[i].SlinkyColor == slinkies[i + 2].SlinkyColor)
            {
                return new List<SlinkyController> { slinkies[i], slinkies[i + 1], slinkies[i + 2] };
            }
        }
        return new List<SlinkyController>();
    }

    private void AnimateMatch(List<SlinkyController> matchedSlinkies)
    {
        _matchQueue.Clear();
        _matchQueue.Enqueue(matchedSlinkies[0]);
        _matchQueue.Enqueue(matchedSlinkies[2]);

        ProcessNextMatch();
    }

    private void ProcessNextMatch()
    {
        if (_matchQueue.Count == 0)
        {
            _targetSlinky.gameObject.SetActive(false);
            GridManager.Instance.RemoveSlinky(_targetSlinky);

            ShiftSlinkiesAfterMatch();
            _isMatching = false;
            return;
        }

        SlinkyController slinky = _matchQueue.Dequeue();
        Vector3 targetPos = GridManager.Instance.GetSlotPosition(_targetSlinky.OccupiedGridKeys[0], false);

        slinky.MoveToTarget(targetPos, _targetSlinky.OccupiedGridKeys[0]);

        slinky.OnMovementComplete.Connect(() =>
        {
            GridManager.Instance.RemoveSlinky(slinky);
            slinky.gameObject.SetActive(false);

            if (_matchQueue.Count == 0)
            {
                ShiftSlinkiesAfterMatch();
            }
            else
            {
                ProcessNextMatch();
            }
        });
    }

    private void ShiftSlinkiesAfterMatch()
    {
        Debug.Log("[MATCH] ShiftSlinkiesAfterMatch çağrıldı!");
        GridManager.Instance.ShiftRemainingSlinkies();
    }
}
