using System.Collections;
using UnityEngine;
using GreatGames.CaseLib.Patterns;
using GreatGames.CaseLib.Signals;
using GreatGames.CaseLib.Grid;
using GreatGames.CaseLib.Level;

public class LevelManager : FoundationSingleton<LevelManager>, IFoundationSingleton
{
    public LevelDataSO ProgressData;
    [SerializeField] private CameraPointer _mainCamera;

    public BasicSignal OnLevelLoaded { get; private set; } = new BasicSignal();
    public BasicSignal OnLevelUnloaded { get; private set; } = new BasicSignal();

    public int CurrentLevelIndex => ProgressData.CurrentLevelIndex;
    public int CurrentLevelCount => ProgressData.CurrentLevelCount + 1;

    public bool Initialized { get ; set; }

    public new void Init()
    {
        Debug.Log("LevelManager Initialized!");
        StartCoroutine(LoadLevelRoutine());
    }

    public void LoadNextLevel()
    {
        DestroyOldLevel();
        ProgressData.CurrentLevelIndex = (ProgressData.CurrentLevelIndex + 1) % ProgressData.Levels.Count;
        ProgressData.CurrentLevelCount++;
        StartCoroutine(LoadLevelRoutine());
    }

    public void RestartLevel()
    {
        DestroyOldLevel();
        StartCoroutine(LoadLevelRoutine());
    }

    private IEnumerator LoadLevelRoutine()
    {
        yield return new WaitForSeconds(1);
        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        if (ProgressData.CurrentLevelIndex >= ProgressData.Levels.Count)
        {
            ProgressData.CurrentLevelIndex = 0;
        }

        LevelConfigSO currentLevel = GetCurrentLevel();

        if (currentLevel == null)
        {
            Debug.LogError("LoadCurrentLevel: GetCurrentLevel() NULL döndü!");
            return;
        }

        currentLevel.InitializeLevel();  

        if (currentLevel.LevelController == null)
        {
            Debug.LogError("LoadCurrentLevel: LevelController NULL!");
            return;
        }

        LevelController levelInstance = currentLevel.LevelController;
        levelInstance.Initialize();

        if (GridManager.Instance == null)
        {
            Debug.LogError("LoadCurrentLevel: GridManager.Instance NULL! Sahneye ekli olduğundan emin ol.");
            return;
        }

        Debug.Log("Calling InitializeGrids with: " + currentLevel.UpperGridSize + " " + currentLevel.LowerGridSize);

        GridManager.Instance.InitializeGrids(currentLevel, levelInstance.transform);
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
