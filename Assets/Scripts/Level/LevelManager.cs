using GreatGames.CaseLib.Definitions;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Level;
using GreatGames.CaseLib.Managers;
using GreatGames.CaseLib.Passenger;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.UI;
using System.Collections;
using UnityEngine;


public class LevelManager : FoundationSingleton<LevelManager>, IFoundationSingleton
{
    public LevelDataSO ProgressData;
    [SerializeField] private CameraPointer _mainCamera;

    public BasicSignal OnLevelLoaded { get; private set; } = new BasicSignal();
    public BasicSignal OnLevelUnloaded { get; private set; } = new BasicSignal();

    public int CurrentLevelIndex => ProgressData.CurrentLevelIndex;
    public int CurrentLevelCount => ProgressData.CurrentLevelCount + 1;

    public bool Initialized { get ; set; }

    public GameType GameType;

    private void Awake()
    {
        StartCoroutine(LoadLevelRoutine());
    }

    public void LoadNextLevel()
    {
        DestroyOldLevel();
        UIManager.Instance.ClearAllPanels();
        ProgressData.CurrentLevelIndex = (ProgressData.CurrentLevelIndex + 1) % ProgressData.Levels.Count;
        ProgressData.CurrentLevelCount++;
        StartCoroutine(LoadLevelRoutine());
    }

    public void RestartLevel()
    {
        DestroyOldLevel();
        UIManager.Instance.ClearAllPanels();
        GridManager.Instance.ClearAll();
        StartCoroutine(LoadLevelRoutine());
    }

    private IEnumerator LoadLevelRoutine()
    {
        yield return new WaitForSeconds(1);
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        GameManager.Instance.ResetLevelState();
        if (ProgressData.CurrentLevelIndex >= ProgressData.Levels.Count)
        {
            ProgressData.CurrentLevelIndex = 0;
        }

        LevelConfigSO currentLevel = GetCurrentLevel();

        if (currentLevel == null)
        {;
            return;
        }

        currentLevel.InitializeLevel();  

        if (currentLevel.LevelController == null)
        {
            return;
        }

        LevelController levelInstance = currentLevel.LevelController;
        levelInstance.Initialize();

        if (GridManager.Instance == null)
        {
            return;
        }


        GridManager.Instance.InitializeGrids(currentLevel, levelInstance.transform);
        if(GameType==GameType.SlinkyGame)
        {
            SlinkyManager.Instance.SpawnSlinkies(currentLevel.Slinkies);
        }

        else
        {
            PassengerGameBuilder.Instance.BuildLevel();
        }
      

        _mainCamera.AdjustCameraByLevelData(currentLevel);
        OnLevelLoaded.Emit();
    }



    private LevelConfigSO GetCurrentLevel()
    {
        if (ProgressData.CurrentLevelIndex < ProgressData.Levels.Count)
        {
            return ProgressData.Levels[ProgressData.CurrentLevelIndex];
        }
        else
        {
            Debug.LogError("Level index out of range.");
            return null;
        }
    }

    private void DestroyOldLevel()
    {
        LevelConfigSO currentLevel = GetCurrentLevel();
        if (currentLevel != null && currentLevel.LevelController != null)
        {
            currentLevel.LevelController.DestroyLevel();
        }
    }
}
