using DG.Tweening;
using GreatGames.CaseLib.DI;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Key;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : FoundationSingleton<MatchManager>, IFoundationSingleton
{
    public bool Initialized { get; set; }

    public void CheckForMatch()
    {
        GridManager.Instance.DebugLowerGridColors(); 
    }


}
