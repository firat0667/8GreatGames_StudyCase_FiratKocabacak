using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
{
    private List<SlinkyController> _slinkies = new List<SlinkyController>();

    public bool Initialized { get; set; }


    public void RegisterSlinky(SlinkyController slinky)
    {
        _slinkies.Add(slinky);
    }

    public void CheckForMatch()
    {
        List<SlinkyController> matchedSlinkies = FindMatch();

        if (matchedSlinkies.Count >= 3)
        {
            foreach (var slinky in matchedSlinkies)
            {
                slinky.gameObject.SetActive(false);
                _slinkies.Remove(slinky);
            }

            ShiftRemainingSlinkies();
        }
    }

    private List<SlinkyController> FindMatch()
    {
        List<SlinkyController> matched = new List<SlinkyController>();

        for (int i = 0; i < _slinkies.Count - 2; i++)
        {
            if (_slinkies[i].SlinkyColor == _slinkies[i + 1].SlinkyColor && _slinkies[i].SlinkyColor == _slinkies[i + 2].SlinkyColor)
            {
                matched.Add(_slinkies[i]);
                matched.Add(_slinkies[i + 1]);
                matched.Add(_slinkies[i + 2]);
            }
        }

        return matched;
    }

    private void ShiftRemainingSlinkies()
    {
        foreach (var slinky in _slinkies)
        {
            GameKey newSlotKey = GridManager.Instance.GetFirstEmptySlot(false);
            if (newSlotKey != null)
            {
                Vector3 targetPosition = GridManager.Instance.GetSlotPosition(newSlotKey, false);
                slinky.MoveToTarget(targetPosition, newSlotKey);
            }
        }
    }

}
