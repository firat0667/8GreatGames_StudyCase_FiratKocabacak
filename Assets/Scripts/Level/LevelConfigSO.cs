using GreatGames.CaseLib.Slinky;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/LevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    [Header("Grid Settings")]
    public Vector2Int UpperGridSize = new(5, 5);
    public Vector2Int LowerGridSize = new(6, 1);

    public List<SlinkyData> Slinkies = new();

    private LevelController _levelController;

    public LevelController LevelController => _levelController;

    public void InitializeLevel()
    {
        Debug.Log("Initializing Level...");

        GameObject levelGO = new GameObject("Level - " + name);
        _levelController = levelGO.AddComponent<LevelController>();  
    }

    public void DestroyLevel()
    {
        if (_levelController != null)
        {
            Debug.Log("Destroying Level...");
            Destroy(_levelController.gameObject);
        }
    }
}
