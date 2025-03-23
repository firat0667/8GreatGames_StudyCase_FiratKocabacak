using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.UI;
using UnityEngine;
public enum GameState
{
    LevelDone,
    LevelFailed,
    Pause,
}
public class GameManager : FoundationSingleton<GameManager>, IFoundationSingleton
{

    public bool Initialized { get; set; }
    private bool _levelCompleted;
    public void TriggerLevelDone()
    {
        VFXManager.Instance.PlayLevelDoneParticle();
        UIManager.Instance.ShowLevelDonePanel();
    }

    public void TriggerLevelFailed()
    {
        UIManager.Instance.ShowLevelFailedPanel();
    }
    public void CheckGameState()
    {
        bool isFull = GridManager.Instance.IsLowerGridFull();
        bool hasMatch = MatchManager.Instance.CheckAnyAvailableMatch();

        if (isFull && !hasMatch)
        {
           Instance.TriggerLevelFailed();
        }
    }
    public void CheckForCompletion()
    {
        if (_levelCompleted) return;

        bool allSlinkiesCleared = GridManager.Instance.GetAllSlinkies().Count == 0;
        if (allSlinkiesCleared)
        {
            _levelCompleted = true;
            UIManager.Instance.ShowLevelDonePanel();
        }
    }
 }

